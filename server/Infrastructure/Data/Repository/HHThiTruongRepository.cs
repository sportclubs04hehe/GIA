using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Data.Repository
{
    public class HHThiTruongRepository : GenericRepository<Dm_HangHoaThiTruong>, IHHThiTruongRepository
    {
        public HHThiTruongRepository(StoreContext context) : base(context)
        {
        }

        // Override to include related entities
        protected override IQueryable<Dm_HangHoaThiTruong> IncludeRelations(IQueryable<Dm_HangHoaThiTruong> query)
        {
            return query
                .Include(x => x.MatHangCha)
                .Include(x => x.DonViTinh);
        }

        // Get all parent categories (only groups with no parent)
        public async Task<List<Dm_HangHoaThiTruong>> GetAllParentCategoriesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsDelete && x.LoaiMatHang == LoaiMatHangEnum.Nhom && x.MatHangChaId == null)
                .OrderBy(x => x.Ten)
                .AsNoTracking()
                .ToListAsync();
        }

        // Get a specific category/product with its direct children
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

        // Delete an item and all its children recursively
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
                        : CombineOr(combinedExpression, expression);
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

        public async Task<PagedList<Dm_HangHoaThiTruong>> GetChildrenByParentIdPagedAsync(Guid parentId, PaginationParams paginationParams)
        {
            var query = _dbSet
                .Where(x => !x.IsDelete && x.MatHangChaId == parentId)
                .Include(x => x.DonViTinh)
                .AsNoTracking();

            // Áp dụng sắp xếp
            query = string.IsNullOrEmpty(paginationParams.OrderBy)
                ? query.OrderBy(x => x.Ten)
                : query.OrderByProperty(paginationParams.OrderBy, paginationParams.SortDescending);

            return await PagedList<Dm_HangHoaThiTruong>.CreateAsync(
                query, 
                paginationParams.PageIndex, 
                paginationParams.PageSize);
        }

        public async Task<List<(Dm_HangHoaThiTruong Category, bool HasChildren)>> GetAllCategoriesWithChildInfoAsync()
        {
            // Sử dụng cách tiếp cận hiệu quả hơn, tránh các truy vấn N+1
            var categories = await _dbSet
                .Where(x => !x.IsDelete && x.LoaiMatHang == LoaiMatHangEnum.Nhom)
                .Include(x => x.MatHangCha) // Vẫn giữ lại Join với mặt hàng cha để lấy tên
                // Bỏ .Include(x => x.DonViTinh) vì mặt hàng nhóm không có đơn vị tính
                .OrderBy(x => x.Ten)
                .AsNoTracking()
                .ToListAsync();
            
            // Lấy tất cả ID của các nhóm có con trong một truy vấn duy nhất
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

        /// <summary>
        /// Lấy đường dẫn từ gốc đến node theo ID
        /// </summary>
        public async Task<List<Guid>> GetPathToRootAsync(Guid nodeId)
        {
            var path = new List<Guid>();
            var currentNodeId = nodeId;

            // Cache lưu trữ các node đã truy vấn để tránh truy vấn trùng lặp
            var nodeCache = new Dictionary<Guid, Guid?>();

            // Lấy đường dẫn từ node hiện tại lên đến gốc
            while (true)
            {
                // Kiểm tra trong cache trước khi truy vấn database
                if (nodeCache.ContainsKey(currentNodeId))
                {
                    path.Add(currentNodeId);

                    if (!nodeCache[currentNodeId].HasValue)
                        break; // Đã đến node gốc

                    currentNodeId = nodeCache[currentNodeId].Value;
                }
                else
                {
                    // Truy vấn chỉ lấy thông tin ID và parent ID để giảm tải dữ liệu
                    var node = await _dbSet
                        .AsNoTracking()
                        .Where(x => x.Id == currentNodeId && !x.IsDelete)
                        .Select(x => new { x.Id, x.MatHangChaId })
                        .FirstOrDefaultAsync();

                    if (node == null)
                        break;

                    // Lưu vào cache
                    nodeCache[node.Id] = node.MatHangChaId;

                    path.Add(node.Id);

                    if (!node.MatHangChaId.HasValue)
                        break; // Đã đến node gốc

                    currentNodeId = node.MatHangChaId.Value;
                }
            }

            // Đảo ngược để có thứ tự từ gốc đến node
            path.Reverse();

            return path;
        }

        /// <summary>
        /// Lấy các node gốc với các node con cần thiết theo đường dẫn, bao gồm tất cả anh em của node mới
        /// </summary>
        public async Task<List<Dm_HangHoaThiTruong>> GetRootNodesWithRequiredChildrenAsync(List<Guid> pathIds, Guid? newItemId = null)
        {
            if (pathIds == null || !pathIds.Any())
                return new List<Dm_HangHoaThiTruong>();

            // Lấy id của node gốc
            var rootId = pathIds.First();

            // Tìm node gốc trong cây
            var rootNodes = await _dbSet
                .Where(x => x.Id == rootId && !x.IsDelete)
                .Include(x => x.DonViTinh)
                .ToListAsync();

            if (!rootNodes.Any())
                return new List<Dm_HangHoaThiTruong>();

            // Xây dựng cây với chỉ các node cần thiết theo đường dẫn
            var result = new List<Dm_HangHoaThiTruong>(rootNodes);

            // Lưu trữ ID của node cha chứa node mới để sau này tải tất cả con của nó
            Guid? parentIdOfNewItem = null;

            // Duyệt qua từng cấp trong đường dẫn để xây dựng cây
            for (int i = 0; i < pathIds.Count - 1; i++)
            {
                var currentId = pathIds[i];
                var nextId = pathIds[i + 1];

                // Tìm node hiện tại trong kết quả
                var currentNode = FindNodeById(result, currentId);
                if (currentNode == null)
                    continue;

                // Nếu đây là node cuối cùng trong đường dẫn, lưu lại ID để tải tất cả con
                if (i == pathIds.Count - 2)
                {
                    parentIdOfNewItem = currentId;
                }

                // Tìm tất cả các node con trực tiếp của node hiện tại trong đường dẫn
                var childNodes = await _dbSet
                    .Where(x => x.MatHangChaId == currentId && !x.IsDelete && x.Id == nextId)
                    .Include(x => x.DonViTinh)
                    .ToListAsync();

                // Đảm bảo chỉ thêm mới node chưa có
                foreach (var child in childNodes)
                {
                    if (currentNode.MatHangCon == null)
                        currentNode.MatHangCon = new List<Dm_HangHoaThiTruong>();

                    if (!currentNode.MatHangCon.Any(x => x.Id == child.Id))
                        currentNode.MatHangCon.Add(child);
                }
            }

            // Lấy node cha cuối cùng trong đường dẫn
            var lastId = pathIds.Last();
            var lastNode = FindNodeById(result, lastId);

            if (lastNode != null)
            {
                // Tải tất cả các node con của node cha cuối cùng
                var childrenOfLastNode = await _dbSet
                    .Where(x => x.MatHangChaId == lastId && !x.IsDelete)
                    .Include(x => x.DonViTinh)
                    .ToListAsync();

                if (lastNode.MatHangCon == null)
                    lastNode.MatHangCon = new List<Dm_HangHoaThiTruong>();

                foreach (var child in childrenOfLastNode)
                {
                    if (!lastNode.MatHangCon.Any(x => x.Id == child.Id))
                        lastNode.MatHangCon.Add(child);
                }

                // Đánh dấu node mới nếu có
                if (newItemId.HasValue && lastNode.MatHangCon.Any(x => x.Id == newItemId))
                {
                    // Không cần làm gì thêm vì tất cả con đã được tải
                    // Và controller sẽ tự động chọn newItemId
                }
            }

            return result;
        }

        // Helper method to find a node in the tree by ID
        private Dm_HangHoaThiTruong FindNodeById(List<Dm_HangHoaThiTruong> nodes, Guid id)
        {
            foreach (var node in nodes)
            {
                if (node.Id == id)
                    return node;

                if (node.MatHangCon != null && node.MatHangCon.Any())
                {
                    var found = FindNodeById(node.MatHangCon.ToList(), id);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        // nhiều mã tồn tại trong cùng một nhóm
        public async Task<List<string>> GetExistingCodesInSameLevelAsync(List<string> codes, Guid? parentId)
        {
            if (codes == null || !codes.Any())
                return new List<string>();

            // Lọc và chuẩn hóa danh sách các mã không rỗng
            var validCodes = codes.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();

            // Nếu không có mã hợp lệ, trả về danh sách rỗng
            if (!validCodes.Any())
                return new List<string>();

            // Lấy danh sách mã đã tồn tại trong cùng cấp
            return await _dbSet
                .AsNoTracking()
                .Where(x => !x.IsDelete && x.MatHangChaId == parentId && validCodes.Contains(x.Ma))
                .Select(x => x.Ma)
                .ToListAsync();
        }

        // mã tồn tại trong cùng một nhóm
        public async Task<bool> ExistsByMaInSameLevelAsync(string ma, Guid? parentId, Guid? exceptId = null)
        {
            // Query items at the same level (same parent) with the same code
            var query = _dbSet
                .AsNoTracking()
                .Where(x => !x.IsDelete)
                .Where(x => x.Ma == ma)
                .Where(x => x.MatHangChaId == parentId);

            // If we're updating an existing item, exclude it from the check
            if (exceptId.HasValue)
            {
                query = query.Where(x => x.Id != exceptId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
