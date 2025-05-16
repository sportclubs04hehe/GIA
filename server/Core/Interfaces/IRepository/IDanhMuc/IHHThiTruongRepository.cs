using Core.Entities.Domain.DanhMuc;
using Core.Interfaces.IGeneric;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.IRepository.IDanhMuc
{
    public interface IHHThiTruongRepository : IGenericRepository<Dm_HangHoaThiTruong>
    {
        Task<List<Dm_HangHoaThiTruong>> GetAllParentCategoriesAsync();
        Task<List<Dm_HangHoaThiTruong>> GetHierarchicalCategoriesAsync();
        Task<bool> DeleteWithChildrenAsync(Guid id);
        Task<Dm_HangHoaThiTruong?> GetWithChildrenAsync(Guid id);
        
        // Add this new method to check for duplicate code
        Task<bool> ExistsByMaInSameLevelAsync(string ma, Guid? parentId, Guid? exceptId = null);

        // Add this method to the existing interface
        Task<IEnumerable<Dm_HangHoaThiTruong>> AddRangeWithValidationAsync(IEnumerable<Dm_HangHoaThiTruong> entities);
    }
}
