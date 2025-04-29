using Application.DTOs.DanhMuc.HangHoasDto;
using Core.Helpers;

namespace Application.ServiceInterface.IDanhMuc
{
    public interface IHangHoaService
    {
        /// <summary>
        /// Lấy thông tin hàng hóa theo ID.
        /// </summary>
        Task<HangHoaDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Lấy thông tin hàng hóa theo mã mặt hàng.
        /// </summary>
        Task<HangHoaDto> GetByMaMatHangAsync(string maMatHang);

        /// <summary>
        /// Lấy danh sách hàng hóa có phân trang.
        /// </summary>
        Task<PagedList<HangHoaDto>> GetAllAsync(PaginationParams paginationParams);

        /// <summary>
        /// Lấy danh sách hàng hóa theo nhóm hàng hóa.
        /// </summary>
        Task<PagedList<HangHoaDto>> GetByNhomHangHoaAsync(Guid nhomHangHoaId, PaginationParams paginationParams);

        /// <summary>
        /// Tìm kiếm hàng hóa theo từ khóa.
        /// </summary>
        Task<PagedList<HangHoaDto>> SearchAsync(SearchParams searchParams);

        /// <summary>
        /// Lọc và sắp xếp danh sách hàng hóa theo nhiều tiêu chí.
        /// </summary>
        Task<PagedList<HangHoaDto>> GetWithFilterAsync(SpecificationParams specParams);

        /// <summary>
        /// Thêm mới hàng hóa.
        /// </summary>
        Task<HangHoaDto> AddAsync(HangHoaCreateDto createDto);

        /// <summary>
        /// Thêm mới nhiều hàng hóa.
        /// </summary>
        Task<(bool IsSuccess, List<HangHoaDto> Data, List<string> Errors)> CreateManyAsync(List<HangHoaCreateDto> dtos);

        /// <summary>
        /// Cập nhật thông tin hàng hóa.
        /// </summary>
        Task<(bool IsSuccess, string ErrorMessage)> UpdateAsync(HangHoaUpdateDto updateDto);

        /// <summary>
        /// Xóa hàng hóa theo ID.
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Kiểm tra sự tồn tại của hàng hóa theo ID.
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Kiểm tra sự tồn tại của hàng hóa theo mã mặt hàng.
        /// </summary>
        Task<bool> ExistsByMaMatHangAsync(string maMatHang, Guid excludeId);

        /// <summary>
        /// Kiểm tra tính hợp lệ của hàng hóa trước khi thêm mới hoặc cập nhật.
        /// </summary>
        Task<(bool IsValid, string ErrorMessage)> ValidateHangHoaAsync(HangHoaDto hangHoaDto, bool isUpdate = false);

        /// <summary>
        /// Đếm số lượng hàng hóa trong hệ thống.
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Lấy danh sách hàng hóa đang có hiệu lực.
        /// </summary>
        Task<PagedList<HangHoaDto>> GetActiveHangHoaAsync(PaginationParams paginationParams);

        Task<(bool IsValid, string ErrorMessage)> ValidateCreateHangHoaAsync(HangHoaCreateDto createDto);
        Task<(bool IsValid, string ErrorMessage)> ValidateUpdateHangHoaAsync(HangHoaUpdateDto updateDto);
    }
}
