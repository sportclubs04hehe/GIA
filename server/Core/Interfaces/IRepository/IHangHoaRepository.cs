using Core.Entities.Domain;
using Core.Helpers;
using Core.Specifications;

namespace Core.Interfaces.IRepository
{
    /// <summary>
    /// Interface cho repository HangHoa với đầy đủ các thao tác truy cập dữ liệu.
    /// </summary>
    public interface IHangHoaRepository
    {
        Task<HangHoa> GetByIdAsync(Guid id);
        Task<HangHoa> GetByMaMatHangAsync(string maMatHang);
        Task<PagedList<HangHoa>> GetAllAsync(PaginationParams paginationParams);
        Task<PagedList<HangHoa>> GetActiveHangHoaAsync(PaginationParams paginationParams);
        Task<PagedList<HangHoa>> GetByNhomHangHoaAsync(Guid nhomHangHoaId, PaginationParams paginationParams);
        Task<PagedList<HangHoa>> GetWithFilterAsync(SpecificationParams specParams);
        Task<PagedList<HangHoa>> SearchAsync(SearchParams searchParams);
        Task<HangHoa> AddAsync(HangHoa hangHoa);
        Task<bool> UpdateAsync(HangHoa hangHoa);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByMaMatHangAsync(string maMatHang);
        Task<int> CountAsync();
    }
}
