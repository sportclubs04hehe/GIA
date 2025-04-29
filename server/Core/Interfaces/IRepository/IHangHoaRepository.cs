using Core.Entities.Domain;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using Core.Specifications;

namespace Core.Interfaces.IRepository
{
    /// <summary>
    /// Interface cho repository HangHoa với đầy đủ các thao tác truy cập dữ liệu.
    /// </summary>
    public interface IHangHoaRepository : IRepository<HangHoa>
    {
        Task<HangHoa> GetByMaMatHangAsync(string maMatHang);
        Task<PagedList<HangHoa>> GetActiveHangHoaAsync(PaginationParams paginationParams);
        Task<PagedList<HangHoa>> GetByNhomHangHoaAsync(Guid nhomHangHoaId, PaginationParams paginationParams);
        Task<PagedList<HangHoa>> GetWithFilterAsync(SpecificationParams specParams);
        Task<bool> ExistsByMaMatHangAsync(string maMatHang, Guid excludeId);
        Task<int> CountAsync();
        IQueryable<HangHoa> SearchQuery(SearchParams p);
    }
}
