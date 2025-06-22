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
        Task<Dictionary<Guid, decimal?>> GetPreviousPricesAsync(List<Guid> hangHoaIds, DateTime ngayThuThap, Guid loaiGiaId);
        Task<bool> ExistsForDateAndPriceTypeAsync(Guid hangHoaId, DateTime ngayThuThap, Guid loaiGiaId);
        Task<List<ThuThapGiaThiTruong>> BulkAddAsync(List<ThuThapGiaThiTruong> entities);
        Task<Dictionary<Guid, bool>> CheckExistenceForBulkAsync(List<Guid> hangHoaIds, DateTime ngayThuThap, Guid loaiGiaId);
    }
}
