using Core.Entities.Domain.DanhMuc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.IRepository.INghiepVu
{
    public interface IGiaDichVuRepository
    {
        Task<(List<Dm_HangHoaThiTruong> HangHoa, Dictionary<Guid, decimal> GiaKyTruoc)> 
            GetHangHoaVaGiaKyTruocAsync(Guid nhomHangHoaId, DateTime? ngayNhap);
            
        Task<List<Dm_HangHoaThiTruong>> SearchMatHangAsync(Guid nhomHangHoaId, string searchTerm, int maxResults = 50);
    }
}
