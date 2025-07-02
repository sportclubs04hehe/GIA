using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.NghiepVu;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Interfaces.IRepository.INghiepVu
{
    public interface IThuThapGiaThiTruongRepository : IGenericRepository<ThuThapGiaThiTruong>
    {
        // Lấy thông tin chi tiết của một phiếu thu thập giá bao gồm danh sách chi tiết giá
        Task<ThuThapGiaThiTruong> GetThuThapGiaThiTruongWithDetailsAsync(Guid id);
        
        // Tạo mới phiếu thu thập giá và danh sách chi tiết giá
        Task<ThuThapGiaThiTruong> CreateThuThapGiaVaChiTietAsync(
            ThuThapGiaThiTruong thuThapGia,
            IEnumerable<ThuThapGiaChiTiet> chiTietGia);
        
        // Cập nhật phiếu thu thập giá và danh sách chi tiết giá
        Task<bool> UpdateThuThapGiaVaChiTietAsync(
            ThuThapGiaThiTruong thuThapGia,
            IEnumerable<ThuThapGiaChiTiet> chiTietGia);
            
        // Tính toán tỷ lệ tăng giảm giá cho tất cả chi tiết giá
        Task<bool> TinhToanTyLeTangGiamAsync(Guid thuThapGiaThiTruongId);
    }
}
