using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Infrastructure.Data.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Infrastructure.Data.DanhMuc.Repository
{
    public class HHThiTruongRepository : GenericRepository<Dm_HangHoaThiTruong>, IHHThiTruongRepository
    {
        private readonly ExpressionBuilder<Dm_HangHoaThiTruong> _expressionBuilder = new ExpressionBuilder<Dm_HangHoaThiTruong>();
        public HHThiTruongRepository(StoreContext context) : base(context)
        {
        }

        // Ghi đè để bao gồm các thực thể liên quan
        protected override IQueryable<Dm_HangHoaThiTruong> IncludeRelations(IQueryable<Dm_HangHoaThiTruong> query)
        {
            return query
                .Include(x => x.MatHangCha)
                .Include(x => x.DonViTinh);
        }

        // Lấy tất cả các danh mục cha (chỉ các nhóm không có cha)
        public async Task<List<Dm_HangHoaThiTruong>> GetAllParentCategoriesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsDelete && x.LoaiMatHang == Loai.Cha && x.MatHangChaId == null)
                .OrderBy(x => x.Ten)
                .AsNoTracking()
                .ToListAsync();
        }
        // Nhận một danh mục/sản phẩm cụ thể với các sản phẩm con trực tiếp của nó
        public async Task<Dm_HangHoaThiTruong?> GetWithChildrenAsync(Guid id)
        {
            return await _dbSet
                .Where(x => x.Id == id && !x.IsDelete)
                .Include(x => x.MatHangCon.Where(c => !c.IsDelete))
                .Include(x => x.DonViTinh)
                .Include(x => x.MatHangCha)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Xóa một mặt hàng và tất cả các mặt hàng con của nó
        /// </summary>
        /// <param name="id">ID mặt hàng cần xóa</param>
        /// <param name="useExternalTransaction">Có sử dụng transaction bên ngoài hay không</param>
        public async Task<bool> DeleteWithChildrenAsync(Guid id, bool useExternalTransaction = false)
        {
            // Chỉ tạo transaction mới khi không sử dụng transaction bên ngoài
            if (!useExternalTransaction)
            {
                using var transaction = await BeginTransactionAsync();
                try
                {
                    var result = await DeleteWithChildrenInternalAsync(id);
                    await transaction.CommitAsync();
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            else
            {
                // Sử dụng transaction bên ngoài, không tạo mới
                return await DeleteWithChildrenInternalAsync(id);
            }
        }

        /// <summary>
        /// Thực hiện xóa mặt hàng và con mà không quản lý transaction
        /// </summary>
        private async Task<bool> DeleteWithChildrenInternalAsync(Guid id)
        {
            var entity = await GetWithChildrenAsync(id);
            if (entity == null) return false;

            await DeleteEntityWithChildren(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // Xóa một mục và tất cả các mục con của nó theo cách đệ quy
        public async Task<bool> DeleteWithChildrenAsync(Guid id)
        {
            using var transaction = await BeginTransactionAsync();
            try
            {
                var entity = await GetWithChildrenAsync(id);
                if (entity == null) return false;

                await DeleteEntityWithChildren(entity);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Recursive helper method for deletion
        private async Task DeleteEntityWithChildren(Dm_HangHoaThiTruong entity)
        {
            // First, recursively delete all children
            if (entity.MatHangCon != null)
            {
                foreach (var child in entity.MatHangCon.Where(x => !x.IsDelete))
                {
                    // Load children of this child if not already loaded
                    if (child.MatHangCon == null || !child.MatHangCon.Any())
                    {
                        var fullChild = await GetWithChildrenAsync(child.Id);
                        if (fullChild != null)
                        {
                            await DeleteEntityWithChildren(fullChild);
                        }
                    }
                    else
                    {
                        await DeleteEntityWithChildren(child);
                    }
                }
            }

            // Then mark the entity itself as deleted
            entity.IsDelete = true;
            entity.ModifiedDate = DateTime.UtcNow;
        }

        // Add this method to the existing repository class
        public async Task<IEnumerable<Dm_HangHoaThiTruong>> AddRangeWithValidationAsync(IEnumerable<Dm_HangHoaThiTruong> entities)
        {
            using var transaction = await BeginTransactionAsync();
            try
            {
                // Group entities by parent to validate code uniqueness within same level
                var groupedByParent = entities.GroupBy(e => e.MatHangChaId);

                foreach (var group in groupedByParent)
                {
                    var parentId = group.Key;
                    var entitiesInGroup = group.ToList();

                    // Check for duplicate codes within the current batch
                    var codeGroups = entitiesInGroup.GroupBy(e => e.Ma).Where(g => g.Count() > 1);
                    if (codeGroups.Any())
                    {
                        var duplicateCodes = string.Join(", ", codeGroups.Select(g => $"'{g.Key}'"));
                        throw new ArgumentException($"Các mã {duplicateCodes} bị trùng lặp trong cùng một nhóm");
                    }

                    // Check for existing codes in the database at the same level
                    var codes = entitiesInGroup.Select(e => e.Ma).ToList();
                    var existingCodes = await _dbSet
                        .Where(x => !x.IsDelete && x.MatHangChaId == parentId && codes.Contains(x.Ma))
                        .Select(x => x.Ma)
                        .ToListAsync();

                    if (existingCodes.Any())
                    {
                        throw new ArgumentException($"Các mã '{string.Join("', '", existingCodes)}' đã tồn tại trong cùng nhóm hàng hóa");
                    }
                }

                var result = await base.AddRangeAsync(entities);
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedList<Dm_HangHoaThiTruong>> SearchAllPagedAsync(
            string searchTerm,
            PaginationParams paginationParams,
            params Expression<Func<Dm_HangHoaThiTruong, string>>[] searchFields)
        {
            var query = _dbSet.AsNoTracking()
                             .Where(x => !x.IsDelete);

            if (!string.IsNullOrWhiteSpace(searchTerm) && searchFields.Length > 0)
            {
                var term = searchTerm.ToLower();

                Expression<Func<Dm_HangHoaThiTruong, bool>> combinedExpression = null;

                foreach (var selector in searchFields)
                {
                    var memberExpression = selector.Body as MemberExpression;
                    if (memberExpression == null)
                        continue;

                    string propertyName = memberExpression.Member.Name;

                    var parameter = Expression.Parameter(typeof(Dm_HangHoaThiTruong), "x");
                    var property = Expression.Property(parameter, propertyName);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    if (toLowerMethod == null)
                        continue;

                    var toLower = Expression.Call(property, toLowerMethod);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    if (containsMethod == null)
                        continue;

                    var contains = Expression.Call(toLower, containsMethod, Expression.Constant(term));
                    var expression = Expression.Lambda<Func<Dm_HangHoaThiTruong, bool>>(contains, parameter);

                    combinedExpression = combinedExpression == null
                        ? expression
                        : _expressionBuilder.CombineOr(combinedExpression, expression);
                }

                if (combinedExpression != null)
                    query = query.Where(combinedExpression);
            }

            // Áp dụng sắp xếp
            query = string.IsNullOrEmpty(paginationParams.OrderBy)
                ? query.OrderBy(x => x.Ten)
                : query.OrderByProperty(paginationParams.OrderBy, paginationParams.SortDescending);

            // Trả về kết quả có phân trang
            return await PagedList<Dm_HangHoaThiTruong>.CreateAsync(
                query.Include(x => x.DonViTinh),
                paginationParams.PageIndex,
                paginationParams.PageSize);
        }

        public async Task<List<Dm_HangHoaThiTruong>> GetRootItemsForSearchAsync(
            HashSet<Guid> parentIds, List<Guid> matchingItemIds)
        {
            // Lấy tất cả nodes cần thiết
            var allNodes = await _dbSet
                .AsNoTracking()
                .Where(x => !x.IsDelete && (parentIds.Contains(x.Id) || matchingItemIds.Contains(x.Id)))
                .Include(x => x.DonViTinh)
                .ToListAsync();

            // Xây dựng map cho việc tìm nhanh
            var nodeMap = allNodes.ToDictionary(x => x.Id);

            // Danh sách các nodes gốc cần trả về
            var rootNodes = new List<Dm_HangHoaThiTruong>();

            // Tìm và thêm các nodes gốc
            foreach (var node in allNodes)
            {
                if (!node.MatHangChaId.HasValue || !nodeMap.ContainsKey(node.MatHangChaId.Value))
                {
                    // Đây là node gốc
                    rootNodes.Add(node);
                }
                else
                {
                    // Đây là node con, thêm vào node cha
                    var parent = nodeMap[node.MatHangChaId.Value];
                    if (parent.MatHangCon == null)
                        parent.MatHangCon = new List<Dm_HangHoaThiTruong>();

                    parent.MatHangCon.Add(node);
                }
            }

            return rootNodes;
        }

        public async Task<PagedList<Dm_HangHoaThiTruong>> GetChildrenByParentIdPagedAsync(
            Guid parentId, PaginationParams paginationParams)
        {
            var query = _dbSet
                .Where(x => !x.IsDelete && x.MatHangChaId == parentId)
                .Include(x => x.DonViTinh)
                .AsNoTracking();

            // Trường hợp sắp xếp theo mã dạng số
            if (string.IsNullOrEmpty(paginationParams.OrderBy) || paginationParams.OrderBy.ToLower() == "ma")
            {
                // Tải dữ liệu về bộ nhớ trước khi sắp xếp
                var allItems = await query.ToListAsync();

                // Sắp xếp trên bộ nhớ sử dụng KeySelector trực tiếp
                var comparer = new MixedCodeComparer();

                var orderedItems = paginationParams.SortDescending
                    ? allItems.OrderByDescending(x => x.Ma, comparer)
                    : allItems.OrderBy(x => x.Ma, comparer);

                // Tính toán số lượng phần tử để phân trang
                int totalCount = allItems.Count;
                var paginatedItems = orderedItems
                    .Skip((paginationParams.PageIndex - 1) * paginationParams.PageSize)
                    .Take(paginationParams.PageSize)
                    .ToList();

                // Sử dụng constructor trực tiếp 
                return new PagedList<Dm_HangHoaThiTruong>(
                    paginatedItems,
                    totalCount,
                    paginationParams.PageIndex,
                    paginationParams.PageSize);
            }
            else
            {
                // Sử dụng sắp xếp thông thường cho các trường khác
                query = query.OrderByProperty(paginationParams.OrderBy, paginationParams.SortDescending);

                // Sử dụng phân trang bình thường trên database
                return await PagedList<Dm_HangHoaThiTruong>.CreateAsync(
                    query,
                    paginationParams.PageIndex,
                    paginationParams.PageSize);
            }
        }

        // Thêm một lớp so sánh mã số
        private class NumericCode : IComparable<NumericCode>, IComparable
        {
            private readonly double[] _parts;

            public NumericCode(string code)
            {
                if (string.IsNullOrEmpty(code))
                {
                    _parts = new double[] { 0 };
                    return;
                }

                // Tách chuỗi theo dấu chấm và các ký tự không phải số
                _parts = System.Text.RegularExpressions.Regex.Split(code, @"\.|\D+")
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(part => double.TryParse(part, out var num) ? num : 0)
                    .ToArray();
            }

            public int CompareTo(NumericCode other)
            {
                if (other == null)
                    return 1;

                int minLength = Math.Min(_parts.Length, other._parts.Length);

                // So sánh từng phần số
                for (int i = 0; i < minLength; i++)
                {
                    int comparison = _parts[i].CompareTo(other._parts[i]);
                    if (comparison != 0)
                        return comparison;
                }

                // Nếu các phần đều bằng nhau, chuỗi ngắn hơn sẽ đứng trước
                return _parts.Length.CompareTo(other._parts.Length);
            }

            public int CompareTo(object obj)
            {
                if (obj is NumericCode other)
                    return CompareTo(other);

                throw new ArgumentException("Object must be of type NumericCode", nameof(obj));
            }
        }

        private class MixedCodeComparer : IComparer<string>
        {
            private readonly StringComparer _viComparer = StringComparer.Create(new CultureInfo("vi-VN"), ignoreCase: false);

            public int Compare(string x, string y)
            {
                var isXNumeric = IsNumericLike(x);
                var isYNumeric = IsNumericLike(y);

                // Nếu cả hai đều là dạng số thì dùng NumericCode
                if (isXNumeric && isYNumeric)
                {
                    return new NumericCode(x).CompareTo(new NumericCode(y));
                }

                // Nếu một cái là số, một cái là chữ, thì số đứng trước
                if (isXNumeric && !isYNumeric) return -1;
                if (!isXNumeric && isYNumeric) return 1;

                // Nếu cả hai là chữ thì dùng so sánh tiếng Việt
                return _viComparer.Compare(x, y);
            }

            private bool IsNumericLike(string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return false;

                // Ví dụ: 1, 1.2, 1001.4.5
                return Regex.IsMatch(input, @"^\d+(\.\d+)*$");
            }
        }

        public async Task<List<(Dm_HangHoaThiTruong Category, bool HasChildren)>> GetAllCategoriesWithChildInfoAsync()
        {
            // Sử dụng cách tiếp cận hiệu quả hơn, tránh các truy vấn N+1
            var categories = await _dbSet
                .Where(x => !x.IsDelete && x.LoaiMatHang == Loai.Cha)
                .Include(x => x.MatHangCha)
                .OrderBy(x => x.Ten)
                .AsNoTracking()
                .ToListAsync();

            var categoryIdsWithChildren = await _dbSet
                .Where(x => !x.IsDelete && x.MatHangChaId.HasValue)
                .Select(x => x.MatHangChaId.Value)
                .Distinct()
                .ToListAsync();

            // Tạo một HashSet cho việc kiểm tra hiệu quả
            var categoryIdsWithChildrenSet = new HashSet<Guid>(categoryIdsWithChildren);

            // Ghép các kết quả lại
            return categories
                .Select(c => (c, categoryIdsWithChildrenSet.Contains(c.Id)))
                .ToList();
        }

        public async Task<Dm_HangHoaThiTruong> GetByIdWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDelete)
                .Include(x => x.DonViTinh)
                .Include(x => x.MatHangCha)
                .FirstOrDefaultAsync();
        }

        // nhiều mã tồn tại trong cùng một nhóm
        public async Task<List<string>> GetExistingCodesInSameLevelAsync(List<string> codes, Guid? parentId)
        {
            if (codes == null || !codes.Any())
                return new List<string>();

            var validCodes = codes.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();

            if (!validCodes.Any())
                return new List<string>();

            return await _dbSet
                .AsNoTracking()
                .Where(x => !x.IsDelete && x.MatHangChaId == parentId && validCodes.Contains(x.Ma))
                .Select(x => x.Ma)
                .ToListAsync();
        }

        // mã tồn tại trong cùng một nhóm
        public async Task<bool> ExistsByMaInSameLevelAsync(string ma, Guid? parentId, Guid? exceptId = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(x => !x.IsDelete)
                .Where(x => x.Ma == ma)
                .Where(x => x.MatHangChaId == parentId);

            if (exceptId.HasValue)
            {
                query = query.Where(x => x.Id != exceptId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<List<Dm_HangHoaThiTruong>> GetHierarchicalPathAsync(Guid itemId)
        {
            // Start with the target item
            var targetItem = await _dbSet
                .AsNoTracking()
                .Include(x => x.DonViTinh)
                .FirstOrDefaultAsync(x => x.Id == itemId && !x.IsDelete);

            if (targetItem == null)
                return new List<Dm_HangHoaThiTruong>();

            // Build the path from child to parents
            var path = new List<Dm_HangHoaThiTruong> { targetItem };
            var currentId = targetItem.MatHangChaId;

            // Walk up the hierarchy until we reach the root
            while (currentId.HasValue)
            {
                var parentItem = await _dbSet
                    .AsNoTracking()
                    .Include(x => x.DonViTinh)
                    .FirstOrDefaultAsync(x => x.Id == currentId.Value && !x.IsDelete);

                if (parentItem == null)
                    break;

                path.Add(parentItem);
                currentId = parentItem.MatHangChaId;
            }

            // Reverse to get root->leaf order
            path.Reverse();
            return path;
        }

    }
}
