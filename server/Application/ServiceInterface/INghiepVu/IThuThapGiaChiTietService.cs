using Application.DTOs.NghiepVu.ThuThapGiaChiTiet;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterface.INghiepVu
{
    public interface IThuThapGiaChiTietService
    {
        // Lấy danh sách chi tiết giá của một phiếu thu thập giá
        Task<IEnumerable<ThuThapGiaChiTietDto>> GetByThuThapGiaIdAsync(Guid thuThapGiaId);
        
        // Lấy thông tin chi tiết giá của một mặt hàng
        Task<ThuThapGiaChiTietDto> GetByIdAsync(Guid id);
        
        // Lấy lịch sử giá của một mặt hàng
        Task<IEnumerable<ThuThapGiaChiTietDto>> GetLichSuGiaByHangHoaAsync(Guid hangHoaId);
        
        // Tạo mới một chi tiết giá
        Task<ThuThapGiaChiTietDto> CreateAsync(ThuThapGiaChiTietCreateDto chiTietGiaDto);
        
        // Cập nhật một chi tiết giá
        Task<bool> UpdateAsync(ThuThapGiaChiTietUpdateDto chiTietGiaDto);
        
        // Xóa một chi tiết giá
        Task<bool> DeleteAsync(Guid id);
        
        // Lưu nhiều chi tiết giá cùng lúc
        Task<bool> SaveManyAsync(IEnumerable<ThuThapGiaChiTietCreateDto> chiTietGiaDto);
        
        // Tính toán tỷ lệ tăng giảm cho các chi tiết giá
        Task<bool> TinhToanTyLeTangGiamAsync(Guid thuThapGiaId);
    }
}
