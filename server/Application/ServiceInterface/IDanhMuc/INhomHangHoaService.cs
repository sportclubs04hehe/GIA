using Application.DTOs.DanhMuc.NhomHangHoasDto;
using Core.Entities.Domain.Enum;
using Core.Helpers;

namespace Application.ServiceInterface.IDanhMuc
{
        public interface INhomHangHoaService
        {
            /// <summary>
            /// Lấy thông tin nhóm hàng hóa theo ID.
            /// </summary>
            Task<NhomHangHoaDto> GetByIdAsync(Guid id);

            /// <summary>
            /// Lấy thông tin nhóm hàng hóa theo mã nhóm.
            /// </summary>
            Task<NhomHangHoaDto> GetByMaNhomAsync(string maNhom);

            /// <summary>
            /// Lấy tất cả nhóm hàng hóa có phân trang.
            /// </summary>
            Task<PagedList<NhomHangHoaDto>> GetAllAsync(PaginationParams paginationParams);

            /// <summary>
            /// Thêm mới một nhóm hàng hóa.
            /// </summary>
            Task<NhomHangHoaDto> AddAsync(CreateNhomHangHoaDto createNhomHangHoaDto);

            /// <summary>
            /// Cập nhật thông tin một nhóm hàng hóa.
            /// </summary>
            Task<bool> UpdateAsync(UpdateNhomHangHoaDto updateNhomHangHoaDto);

            /// <summary>
            /// Xóa một nhóm hàng hóa theo ID.
            /// </summary>
            Task<bool> DeleteAsync(Guid id);

            /// <summary>
            /// Lấy danh sách nhóm con trực tiếp của một nhóm hàng hóa.
            /// </summary>
            Task<List<NhomHangHoaDto>> GetChildGroupsAsync(Guid parentId);

            /// <summary>
            /// Lấy tất cả các nhóm gốc (không có nhóm cha).
            /// </summary>
            Task<List<NhomHangHoaDto>> GetRootGroupsAsync();

            /// <summary>
            /// Tìm kiếm nhóm hàng hóa theo tên hoặc mã nhóm có phân trang.
            /// </summary>
            Task<PagedList<NhomHangHoaDto>> SearchAsync(SearchParams searchParams);

            /// <summary>
            /// Lọc, sắp xếp và phân trang nhóm hàng hóa.
            /// </summary>
            Task<PagedList<NhomHangHoaDto>> GetWithFilterAsync(SpecificationParams specParams);

            /// <summary>
            /// Kiểm tra sự tồn tại của nhóm hàng hóa theo ID.
            /// </summary>
            Task<bool> ExistsAsync(Guid id);

            /// <summary>
            /// Kiểm tra sự tồn tại của nhóm hàng hóa theo mã nhóm.
            /// </summary>
            Task<bool> ExistsByMaNhomAsync(string maNhom);

            /// <summary>
            /// Lấy cấu trúc phân cấp đầy đủ của nhóm hàng hóa.
            /// </summary>
            Task<List<NhomHangHoaDto>> GetHierarchyAsync();

            // Thêm các phương thức mới
            Task<List<NhomHangHoaDto>> GetByLoaiNhomAsync(LoaiNhom loaiNhom);
            Task<List<NhomHangHoaDto>> GetChildGroupsByLoaiNhomAsync(Guid parentId, LoaiNhom loaiNhom);
            Task<bool> CanBeParentAsync(Guid parentId, LoaiNhom childLoaiNhom);
            Task<HierarchyDto> GetHierarchyWithLoaiNhomAsync();
            Task<bool> IsParentOfAnyGroupAsync(Guid id);
            Task<bool> HasProductsAsync(Guid id);

    }
}
