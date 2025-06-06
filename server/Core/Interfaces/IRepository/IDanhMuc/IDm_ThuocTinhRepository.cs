using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IGeneric;

namespace Core.Interfaces.IRepository.IDanhMuc
{
    public interface IDm_ThuocTinhRepository: IGenericRepository<Dm_ThuocTinh>
    {
        Task<List<Dm_ThuocTinh>> GetAllParentCategoriesAsync();
    }
}
