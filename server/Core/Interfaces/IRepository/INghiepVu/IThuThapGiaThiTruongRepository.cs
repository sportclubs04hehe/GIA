using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.NghiepVu;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using System.Threading.Tasks;

namespace Core.Interfaces.IRepository.INghiepVu
{
    public interface IThuThapGiaThiTruongRepository : IGenericRepository<ThuThapGiaThiTruong>
    {
        Task<PagedList<ThuThapGiaThiTruong>> SearchAsync(SearchParams searchParams);
        Task<List<Dm_HangHoaThiTruong>> GetHierarchicalDataWithPreviousPricesAsync(
           Guid parentId,
           DateTime ngayThuThap,
           Guid loaiGiaId);
        // Thêm method này
        Task<Dictionary<Guid, decimal?>> GetPreviousPricesAsync(List<Guid> hangHoaIds, DateTime ngayThuThap, Guid loaiGiaId);
    }
}
