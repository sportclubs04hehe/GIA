using Core.Entities.Domain.DanhMuc;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.Generic;
using Microsoft.EntityFrameworkCore;
using Core.Helpers;
using System.Linq.Expressions;

namespace Infrastructure.Data.Repository
{
    public class NhomHangHoaRepository : GenericRepository<Dm_NhomHangHoa>, INhomHangHoaRepository
    {
        private readonly StoreContext _storeContext;

        public NhomHangHoaRepository(StoreContext context) : base(context)
        {
            _storeContext = context;
        }

        public async Task<List<Dm_NhomHangHoa>> GetRootNodesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsDelete && x.NhomChaId == null)
                .OrderBy(x => x.MaNhom)
                .ToListAsync();
        }

        protected override IQueryable<Dm_NhomHangHoa> IncludeRelations(IQueryable<Dm_NhomHangHoa> query)
        {
            return query.Include(x => x.NhomCha);
        }

        public async Task<bool> IsMaNhomExistsInSameParentAsync(string maNhom, Guid? parentId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(x => !x.IsDelete && x.MaNhom == maNhom && x.NhomChaId == parentId);
            
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
                
            return await query.AnyAsync();
        }

        public async Task<List<Dm_NhomHangHoa>> GetChildGroupsAsync(Guid parentId)
        {
            return await _dbSet
                .Where(x => !x.IsDelete && x.NhomChaId == parentId)
                .ToListAsync();
        }

        public async Task<List<Dm_NhomHangHoa>> GetAllDescendantsAsync(Guid parentId)
        {
            var result = new List<Dm_NhomHangHoa>();
            var directChildren = await GetChildGroupsAsync(parentId);
            
            if (!directChildren.Any())
                return result;
                
            result.AddRange(directChildren);
            
            foreach (var child in directChildren)
            {
                var descendants = await GetAllDescendantsAsync(child.Id);
                result.AddRange(descendants);
            }
            
            return result;
        }

        public async Task<PagedList<Dm_HangHoa>> GetAllProductsInGroupAsync(Guid groupId, PaginationParams paginationParams)
        {
            // Lấy tất cả nhóm con (bao gồm nhóm hiện tại)
            var allGroupIds = new List<Guid> { groupId };
            var childGroups = await GetAllDescendantsAsync(groupId);
            allGroupIds.AddRange(childGroups.Select(g => g.Id));
            
            // Lấy hàng hóa từ tất cả nhóm
            var query = _storeContext.HangHoas
                .Where(p => !p.IsDelete && allGroupIds.Contains(p.NhomHangHoaId.Value))
                .OrderByDescending(p => p.CreatedDate);
                
            return await PagedList<Dm_HangHoa>.CreateAsync(
                query,
                paginationParams.PageIndex,
                paginationParams.PageSize);
        }

        public async Task<bool> DeleteGroupWithRelatedEntitiesAsync(Guid groupId)
        {
            using var transaction = await _storeContext.Database.BeginTransactionAsync();
            
            try
            {
                // 1. Xác định tất cả các nhóm con
                var allChildGroups = await GetAllDescendantsAsync(groupId);
                var allGroupIds = new List<Guid> { groupId };
                allGroupIds.AddRange(allChildGroups.Select(g => g.Id));
                
                // 2. Đánh dấu xóa tất cả hàng hóa trong các nhóm
                var relatedProducts = await _storeContext.HangHoas
                    .Where(p => !p.IsDelete && allGroupIds.Contains(p.NhomHangHoaId.Value))
                    .ToListAsync();
                    
                foreach (var product in relatedProducts)
                {
                    product.IsDelete = true;
                    product.ModifiedDate = DateTime.UtcNow;
                }
                
                // 3. Đánh dấu xóa tất cả nhóm con (từ sâu đến nông)
                foreach (var childGroup in allChildGroups.OrderByDescending(g => g.NhomCon.Count))
                {
                    childGroup.IsDelete = true;
                    childGroup.ModifiedDate = DateTime.UtcNow;
                }
                
                // 4. Cuối cùng đánh dấu xóa nhóm cha
                var mainGroup = await _dbSet.FindAsync(groupId);
                if (mainGroup != null)
                {
                    mainGroup.IsDelete = true;
                    mainGroup.ModifiedDate = DateTime.UtcNow;
                }
                
                await _storeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
