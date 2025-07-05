using Core.Entities.Domain.DanhMuc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IRepository.INghiepVu
{
    public interface IGiaDichVuRepository
    {
        Task<(List<Dm_HangHoaThiTruong> HangHoa, Dictionary<Guid, decimal> GiaKyTruoc)>
        GetHangHoaVaGiaKyTruocAsync(Guid nhomHangHoaId, DateTime? ngayNhap);
    }
}
