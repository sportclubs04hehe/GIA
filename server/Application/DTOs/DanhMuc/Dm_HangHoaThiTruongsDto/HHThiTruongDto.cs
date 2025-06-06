using Core.Entities.Domain.DanhMuc.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto
{
    public class HHThiTruongDto : BaseDto
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        public Loai LoaiMatHang { get; set; }
        
        // Thêm thuộc tính này
        public Guid? MatHangChaId { get; set; }
        public string? TenMatHangCha { get; set; }

        // Thuộc tính cho hàng hóa
        public string? DacTinh { get; set; }
        public Guid? DonViTinhId { get; set; }
        public string? TenDonViTinh { get; set; }
    }
}
