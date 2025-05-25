using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Interfaces.IRepository.IDanhMuc
{
    public interface IHHThiTruongRepository : IGenericRepository<Dm_HangHoaThiTruong>
    {
        Task<List<Dm_HangHoaThiTruong>> GetAllParentCategoriesAsync();
        Task<Dm_HangHoaThiTruong?> GetWithChildrenAsync(Guid id);
        Task<bool> DeleteWithChildrenAsync(Guid id);
        Task<bool> ExistsByMaInSameLevelAsync(string ma, Guid? parentId, Guid? exceptId = null);
        Task<IEnumerable<Dm_HangHoaThiTruong>> AddRangeWithValidationAsync(IEnumerable<Dm_HangHoaThiTruong> entities);
        Task<PagedList<Dm_HangHoaThiTruong>> GetChildrenByParentIdPagedAsync(Guid parentId, PaginationParams paginationParams);
        
        // Thêm phương thức thiếu
        Task<List<Dm_HangHoaThiTruong>> SearchAllAsync(string searchTerm, params Expression<Func<Dm_HangHoaThiTruong, string>>[] searchFields);
        Task<List<Dm_HangHoaThiTruong>> GetRootItemsForSearchAsync(HashSet<Guid> parentIds, List<Guid> matchingItemIds);
        Task<List<(Dm_HangHoaThiTruong Category, bool HasChildren)>> GetAllCategoriesWithChildInfoAsync();
        Task<Dm_HangHoaThiTruong> GetByIdWithRelationsAsync(Guid id);
        Task<List<Dm_HangHoaThiTruong>> GetRootNodesWithRequiredChildrenAsync(List<Guid> pathIds, Guid? newItemId = null);
    }
}
