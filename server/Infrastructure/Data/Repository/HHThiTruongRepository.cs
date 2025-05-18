using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
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

        // Get hierarchical tree structure starting from root categories
        public async Task<List<Dm_HangHoaThiTruong>> GetHierarchicalCategoriesAsync()
        {
            // Start with root categories and include children recursively
            var rootCategories = await _dbSet
                .Where(x => !x.IsDelete && x.MatHangChaId == null)
                .Include(x => x.MatHangCon.Where(c => !c.IsDelete))
                .ThenInclude(x => x.MatHangCon.Where(c => !c.IsDelete))
                .ThenInclude(x => x.MatHangCon.Where(c => !c.IsDelete))
                .OrderBy(x => x.Ten)
                .AsNoTracking()
                .ToListAsync();

            return rootCategories;
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

        public async Task<bool> ExistsByMaInSameLevelAsync(string ma, Guid? parentId, Guid? exceptId = null)
        {
            // Query items at the same level (same parent) with the same code
            var query = _dbSet
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

        public async Task<List<Dm_HangHoaThiTruong>> GetChildrenByParentIdAsync(Guid parentId)
        {
            return await _dbSet
                .Where(x => !x.IsDelete && x.MatHangChaId == parentId)
                .Include(x => x.DonViTinh)
                .OrderBy(x => x.Ten)
                .AsNoTracking()
                .ToListAsync();
        }

        // Tìm kiếm không phân trang
        public async Task<List<Dm_HangHoaThiTruong>> SearchAllAsync(
            string searchTerm, 
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
                    // Kiểm tra null cho phương thức ToLower
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    if (toLowerMethod == null)
                        continue;

                    var toLower = Expression.Call(property, toLowerMethod);

                    // Kiểm tra null cho phương thức Contains
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
            
            return await query
                .Include(x => x.DonViTinh)
                .OrderBy(x => x.Ten)
                .ToListAsync();
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
    }
}
