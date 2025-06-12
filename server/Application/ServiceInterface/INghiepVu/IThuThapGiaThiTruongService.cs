using Application.DTOs.NghiepVu.ThuThapGiaThiTruong;
using Core.Helpers;

namespace Application.ServiceInterface.INghiepVu
{
    public interface IThuThapGiaThiTruongService
    {
        // Create
        Task<ThuThapGiaThiTruongDto> CreateAsync(ThuThapGiaThiTruongCreateDto createDto);
        
        // Read
        Task<ThuThapGiaThiTruongDto> GetByIdAsync(Guid id);
        Task<PagedList<ThuThapGiaThiTruongDto>> GetAllAsync(PaginationParams paginationParams);
        Task<PagedList<ThuThapGiaThiTruongDto>> SearchAsync(SearchParams searchParams);
        
        // Update
        Task<bool> UpdateAsync(ThuThapGiaThiTruongUpdateDto updateDto);
        
        // Delete
        Task<bool> DeleteAsync(Guid id);
        
        // Check if exists
        Task<bool> ExistsAsync(Guid id);
    }
}
