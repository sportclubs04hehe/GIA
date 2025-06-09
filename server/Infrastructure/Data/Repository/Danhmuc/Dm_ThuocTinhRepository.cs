using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Infrastructure.Data.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Data.Repository.Danhmuc
{
    public class Dm_ThuocTinhRepository : GenericRepository<Dm_ThuocTinh>, IDm_ThuocTinhRepository
    {
        private readonly ExpressionBuilder<Dm_ThuocTinh> _expressionBuilder = new ExpressionBuilder<Dm_ThuocTinh>();

        public Dm_ThuocTinhRepository(StoreContext context) : base(context)
        {
        }

        // Lấy tất cả các thuộc tính cha (không có cha)
        public async Task<List<Dm_ThuocTinh>> GetAllParentCategoriesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsDelete && x.Loai == Loai.Cha && x.ThuocTinhChaId == null)
                .OrderBy(x => x.Ten)
                .AsNoTracking()
                .ToListAsync();
        }

        // Lấy thuộc tính kèm các thuộc tính con
        public async Task<Dm_ThuocTinh?> GetWithChildrenAsync(Guid id)
        {
            return await _dbSet
                .Where(x => x.Id == id && !x.IsDelete)
                .Include(x => x.ThuocTinhCon.Where(c => !c.IsDelete))
                .Include(x => x.ThuocTinhCha)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }

        // Xóa thuộc tính và tất cả thuộc tính con
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

        // Phương thức đệ quy hỗ trợ xóa
        private async Task DeleteEntityWithChildren(Dm_ThuocTinh entity)
        {
            // Xóa tất cả các con trước
            if (entity.ThuocTinhCon != null)
            {
                foreach (var child in entity.ThuocTinhCon.Where(x => !x.IsDelete))
                {
                    // Tải con của con này nếu chưa được tải
                    if (child.ThuocTinhCon == null || !child.ThuocTinhCon.Any())
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

            // Sau đó đánh dấu thực thể là đã xóa
            entity.IsDelete = true;
            entity.ModifiedDate = DateTime.UtcNow;
        }

        // Kiểm tra mã đã tồn tại ở cùng cấp không
        public async Task<bool> ExistsByMaInSameLevelAsync(string ma, Guid? parentId, Guid? exceptId = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(x => !x.IsDelete)
                .Where(x => x.Ma == ma)
                .Where(x => x.ThuocTinhChaId == parentId);

            if (exceptId.HasValue)
            {
                query = query.Where(x => x.Id != exceptId.Value);
            }

            return await query.AnyAsync();
        }

        // Thêm nhiều thuộc tính với xác thực mã trùng lặp
        public async Task<IEnumerable<Dm_ThuocTinh>> AddRangeWithValidationAsync(IEnumerable<Dm_ThuocTinh> entities)
        {
            using var transaction = await BeginTransactionAsync();
            try
            {
                // Nhóm các thực thể theo cha để xác thực tính duy nhất của mã trong cùng cấp
                var groupedByParent = entities.GroupBy(e => e.ThuocTinhChaId);
                
                foreach (var group in groupedByParent)
                {
                    var parentId = group.Key;
                    var entitiesInGroup = group.ToList();
                    
                    // Kiểm tra mã trùng lặp trong lô hiện tại
                    var codeGroups = entitiesInGroup.GroupBy(e => e.Ma).Where(g => g.Count() > 1);
                    if (codeGroups.Any())
                    {
                        var duplicateCodes = string.Join(", ", codeGroups.Select(g => $"'{g.Key}'"));
                        throw new ArgumentException($"Các mã {duplicateCodes} bị trùng lặp trong cùng một nhóm");
                    }
                    
                    // Kiểm tra mã đã tồn tại trong cơ sở dữ liệu ở cùng cấp
                    var codes = entitiesInGroup.Select(e => e.Ma).ToList();
                    var existingCodes = await _dbSet
                        .Where(x => !x.IsDelete && x.ThuocTinhChaId == parentId && codes.Contains(x.Ma))
                        .Select(x => x.Ma)
                        .ToListAsync();
                        
                    if (existingCodes.Any())
                    {
                        throw new ArgumentException($"Các mã '{string.Join("', '", existingCodes)}' đã tồn tại trong cùng nhóm thuộc tính");
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

        // Lấy danh sách con phân trang theo ID cha
        public async Task<PagedList<Dm_ThuocTinh>> GetChildrenByParentIdPagedAsync(Guid parentId, PaginationParams paginationParams)
        {
            var query = _dbSet
                .Where(x => !x.IsDelete && x.ThuocTinhChaId == parentId)
                .AsNoTracking();

            // Áp dụng sắp xếp
            query = string.IsNullOrEmpty(paginationParams.OrderBy)
                ? query.OrderBy(x => x.Ten)
                : query.OrderByProperty(paginationParams.OrderBy, paginationParams.SortDescending);

            return await PagedList<Dm_ThuocTinh>.CreateAsync(
                query,
                paginationParams.PageIndex, 
                paginationParams.PageSize);
        }

        // Tìm kiếm tất cả với phân trang
        public async Task<PagedList<Dm_ThuocTinh>> SearchAllPagedAsync(
            string searchTerm,
            PaginationParams paginationParams,
            params Expression<Func<Dm_ThuocTinh, string>>[] searchFields)
        {
            var query = _dbSet.AsNoTracking()
                             .Where(x => !x.IsDelete);

            if (!string.IsNullOrWhiteSpace(searchTerm) && searchFields.Length > 0)
            {
                var term = searchTerm.ToLower();
                Expression<Func<Dm_ThuocTinh, bool>> combinedExpression = null;

                foreach (var selector in searchFields)
                {
                    var memberExpression = selector.Body as MemberExpression;
                    if (memberExpression == null)
                        continue;

                    string propertyName = memberExpression.Member.Name;
                    var parameter = Expression.Parameter(typeof(Dm_ThuocTinh), "x");
                    var property = Expression.Property(parameter, propertyName);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    if (toLowerMethod == null)
                        continue;

                    var toLower = Expression.Call(property, toLowerMethod);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    if (containsMethod == null)
                        continue;

                    var contains = Expression.Call(toLower, containsMethod, Expression.Constant(term));
                    var expression = Expression.Lambda<Func<Dm_ThuocTinh, bool>>(contains, parameter);

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
            return await PagedList<Dm_ThuocTinh>.CreateAsync(
                query,
                paginationParams.PageIndex,
                paginationParams.PageSize);
        }

        // Lấy các node gốc cho tìm kiếm
        public async Task<List<Dm_ThuocTinh>> GetRootItemsForSearchAsync(
            HashSet<Guid> parentIds, List<Guid> matchingItemIds)
        {
            // Lấy tất cả nodes cần thiết
            var allNodes = await _dbSet
                .AsNoTracking()
                .Where(x => !x.IsDelete && (parentIds.Contains(x.Id) || matchingItemIds.Contains(x.Id)))
                .ToListAsync();
            
            // Xây dựng map cho việc tìm nhanh
            var nodeMap = allNodes.ToDictionary(x => x.Id);
            
            // Danh sách các nodes gốc cần trả về
            var rootNodes = new List<Dm_ThuocTinh>();
            
            // Tìm và thêm các nodes gốc
            foreach (var node in allNodes)
            {
                if (!node.ThuocTinhChaId.HasValue || !nodeMap.ContainsKey(node.ThuocTinhChaId.Value))
                {
                    // Đây là node gốc
                    rootNodes.Add(node);
                }
                else
                {
                    // Đây là node con, thêm vào node cha
                    var parent = nodeMap[node.ThuocTinhChaId.Value];
                    if (parent.ThuocTinhCon == null)
                        parent.ThuocTinhCon = new List<Dm_ThuocTinh>();
                        
                    parent.ThuocTinhCon.Add(node);
                }
            }
            
            return rootNodes;
        }

        // Lấy tất cả thuộc tính với thông tin có con hay không
        public async Task<List<(Dm_ThuocTinh Category, bool HasChildren)>> GetAllCategoriesWithChildInfoAsync()
        {
            // Lấy tất cả các thuộc tính
            var categories = await _dbSet
                .Where(x => !x.IsDelete && x.Loai == Loai.Cha)
                .Include(x => x.ThuocTinhCha)
                .OrderBy(x => x.Ten)
                .AsNoTracking()
                .ToListAsync();
            
            // Lấy danh sách ID các thuộc tính có con
            var categoryIdsWithChildren = await _dbSet
                .Where(x => !x.IsDelete && x.ThuocTinhChaId.HasValue)
                .Select(x => x.ThuocTinhChaId.Value)
                .Distinct()
                .ToListAsync();
            
            // Tạo HashSet cho kiểm tra hiệu quả
            var categoryIdsWithChildrenSet = new HashSet<Guid>(categoryIdsWithChildren);
            
            // Kết hợp kết quả
            return categories
                .Select(c => (c, categoryIdsWithChildrenSet.Contains(c.Id)))
                .ToList();
        }

        // Lấy thuộc tính theo ID với các quan hệ
        public async Task<Dm_ThuocTinh> GetByIdWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDelete)
                .Include(x => x.ThuocTinhCha)
                .FirstOrDefaultAsync();
        }

        // Lấy đường dẫn từ node tới gốc
        public async Task<List<Guid>> GetPathToRootAsync(Guid nodeId)
        {
            var path = new List<Guid>();
            var currentNodeId = nodeId;
            var nodeCache = new Dictionary<Guid, Guid?>();

            while (true)
            {
                if (nodeCache.ContainsKey(currentNodeId))
                {
                    path.Add(currentNodeId);
                    if (!nodeCache[currentNodeId].HasValue)
                        break; // Đến node gốc

                    currentNodeId = nodeCache[currentNodeId].Value;
                }
                else
                {
                    var node = await _dbSet
                        .AsNoTracking()
                        .Where(x => x.Id == currentNodeId && !x.IsDelete)
                        .Select(x => new { x.Id, x.ThuocTinhChaId })
                        .FirstOrDefaultAsync();

                    if (node == null)
                        break;

                    nodeCache[node.Id] = node.ThuocTinhChaId;
                    path.Add(node.Id);

                    if (!node.ThuocTinhChaId.HasValue)
                        break; // Đến node gốc

                    currentNodeId = node.ThuocTinhChaId.Value;
                }
            }

            path.Reverse();
            return path;
        }

        // Lấy các node gốc với các node con theo đường dẫn
        public async Task<List<Dm_ThuocTinh>> GetRootNodesWithRequiredChildrenAsync(List<Guid> pathIds, Guid? newItemId = null)
        {
            if (pathIds == null || !pathIds.Any())
                return new List<Dm_ThuocTinh>();

            // Lấy ID node gốc
            var rootId = pathIds.First();

            // Tìm node gốc
            var rootNodes = await _dbSet
                .Where(x => x.Id == rootId && !x.IsDelete)
                .ToListAsync();

            if (!rootNodes.Any())
                return new List<Dm_ThuocTinh>();

            var result = new List<Dm_ThuocTinh>(rootNodes);

            // Duyệt từng cấp trong đường dẫn để xây dựng cây
            for (int i = 0; i < pathIds.Count - 1; i++)
            {
                var currentId = pathIds[i];
                var nextId = pathIds[i + 1];

                var currentNode = FindNodeById(result, currentId);
                if (currentNode == null)
                    continue;

                // Tìm node con trực tiếp trong đường dẫn
                var childNodes = await _dbSet
                    .Where(x => x.ThuocTinhChaId == currentId && !x.IsDelete && x.Id == nextId)
                    .ToListAsync();

                foreach (var child in childNodes)
                {
                    if (currentNode.ThuocTinhCon == null)
                        currentNode.ThuocTinhCon = new List<Dm_ThuocTinh>();

                    if (!currentNode.ThuocTinhCon.Any(x => x.Id == child.Id))
                        currentNode.ThuocTinhCon.Add(child);
                }
            }

            // Tải tất cả các node con của node cuối cùng
            var lastId = pathIds.Last();
            var lastNode = FindNodeById(result, lastId);

            if (lastNode != null)
            {
                var childrenOfLastNode = await _dbSet
                    .Where(x => x.ThuocTinhChaId == lastId && !x.IsDelete)
                    .ToListAsync();

                if (lastNode.ThuocTinhCon == null)
                    lastNode.ThuocTinhCon = new List<Dm_ThuocTinh>();

                foreach (var child in childrenOfLastNode)
                {
                    if (!lastNode.ThuocTinhCon.Any(x => x.Id == child.Id))
                        lastNode.ThuocTinhCon.Add(child);
                }
            }

            return result;
        }

        // Phương thức hỗ trợ tìm node theo ID trong cây
        private Dm_ThuocTinh FindNodeById(List<Dm_ThuocTinh> nodes, Guid id)
        {
            foreach (var node in nodes)
            {
                if (node.Id == id)
                    return node;

                if (node.ThuocTinhCon != null && node.ThuocTinhCon.Any())
                {
                    var found = FindNodeById(node.ThuocTinhCon.ToList(), id);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        // Lấy danh sách mã đã tồn tại ở cùng cấp
        public async Task<List<string>> GetExistingCodesInSameLevelAsync(List<string> codes, Guid? parentId)
        {
            if (codes == null || !codes.Any())
                return new List<string>();

            var validCodes = codes.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();

            if (!validCodes.Any())
                return new List<string>();

            return await _dbSet
                .AsNoTracking()
                .Where(x => !x.IsDelete && x.ThuocTinhChaId == parentId && validCodes.Contains(x.Ma))
                .Select(x => x.Ma)
                .ToListAsync();
        }
    }
}
