using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class TongHopDuLieuThemMoiTT29Dto
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string GhiChu { get; set; }
        public string DacTinh { get; set; }
        public int LoaiMatHang { get; set; }
        public Guid? MatHangChaId { get; set; }
        public int Level { get; set; }
        public Guid? DonViTinhId { get; set; }
        public string? TenDonViTinh { get; set; }

        // Thêm trường giá kỳ trước
        public decimal? GiaBinhQuanKyTruoc { get; set; }

        public List<HHThiTruongTreeNodeDto> MatHangCon { get; set; } = new List<HHThiTruongTreeNodeDto>();
    }
}
