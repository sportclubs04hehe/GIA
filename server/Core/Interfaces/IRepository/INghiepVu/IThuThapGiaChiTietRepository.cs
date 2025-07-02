using Core.Entities.Domain.NghiepVu;
using Core.Interfaces.IGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IRepository.INghiepVu
{
    public interface IThuThapGiaChiTietRepository : IGenericRepository<ThuThapGiaChiTiet>
    {
        // Lấy tất cả chi tiết giá của một phiếu thu thập giá
        Task<List<ThuThapGiaChiTiet>> GetByThuThapGiaIdAsync(Guid thuThapGiaId);

        // Lấy lịch sử giá của một mặt hàng
        Task<List<ThuThapGiaChiTiet>> GetLichSuGiaByHangHoaAsync(Guid hangHoaId);
    }
}
