using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using Core.Specifications;

namespace Core.Interfaces.IRepository.IDanhMuc
{
    /// <summary>
    /// Interface cho repository HangHoa với đầy đủ các thao tác truy cập dữ liệu.
    /// </summary>
    public interface IHangHoaRepository : IGenericRepository<Dm_HangHoa>
    {
        Task<Dm_HangHoa> GetByMaMatHangAsync(string maMatHang);
        Task<PagedList<Dm_HangHoa>> GetActiveHangHoaAsync(PaginationParams paginationParams);
        Task<PagedList<Dm_HangHoa>> GetByNhomHangHoaAsync(Guid nhomHangHoaId, PaginationParams paginationParams);
        Task<PagedList<Dm_HangHoa>> GetWithFilterAsync(SpecificationParams specParams);
        Task<bool> ExistsByMaMatHangAsync(
            string maMatHang,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);
        Task<int> CountAsync();
        Task<PagedList<Dm_HangHoa>> SearchQuery(SearchParams p);
    }
}
