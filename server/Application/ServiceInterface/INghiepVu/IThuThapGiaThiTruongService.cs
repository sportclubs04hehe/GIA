using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.DTOs.DanhMuc.Dm_LoaiGia;
using Application.DTOs.NghiepVu.ThuThapGiaChiTiet;
using Application.DTOs.NghiepVu.ThuThapGiaThiTruong;
using Core.Helpers;
namespace Application.ServiceInterface.INghiepVu
{
    public interface IThuThapGiaThiTruongService
    {
        // Lấy danh sách các loại giá (để hiển thị trong dropdown chọn)
        Task<IEnumerable<LoaiGiaDto>> GetLoaiGiaAsync();
        
        // Lấy thông tin của một phiếu thu thập giá đã có
        Task<ThuThapGiaThiTruongDto> GetByIdAsync(Guid id);
        
        // Lấy thông tin chi tiết của một phiếu thu thập giá bao gồm danh sách chi tiết giá
        Task<ThuThapGiaThiTruongDto> GetWithDetailsAsync(Guid id);
        
        // Tạo mới phiếu thu thập giá và danh sách chi tiết giá
        Task<ThuThapGiaThiTruongDto> CreateThuThapGiaVaChiTietAsync(
            ThuThapGiaThiTruongCreateDto thuThapGiaDto,
            IEnumerable<ThuThapGiaChiTietCreateDto> chiTietGiaDto);
        
        // Cập nhật phiếu thu thập giá và danh sách chi tiết giá
        Task<bool> UpdateThuThapGiaVaChiTietAsync(
            ThuThapGiaThiTruongUpdateDto thuThapGiaDto,
            IEnumerable<ThuThapGiaChiTietUpdateDto> chiTietGiaDto);
        
        // Xóa phiếu thu thập giá
        Task<bool> DeleteAsync(Guid id);
        
        // Lấy danh sách phiếu thu thập giá có phân trang, tìm kiếm
        Task<PagedList<ThuThapGiaThiTruongDto>> GetAllAsync(PaginationParams paginationParams);
        
        // Tìm kiếm phiếu thu thập giá
        Task<PagedList<ThuThapGiaThiTruongDto>> SearchAsync(SearchParams searchParams);
        Task<List<HHThiTruongTreeNodeDto>> GetAllChildrenRecursiveAsync(Guid parentId, DateTime? ngayNhap = null);

    }
}
