using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;

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
    }
}
