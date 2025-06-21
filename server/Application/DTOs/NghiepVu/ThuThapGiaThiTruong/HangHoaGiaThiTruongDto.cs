using Core.Entities.Domain.DanhMuc.Enum;

namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class HangHoaGiaThiTruongDto
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public Loai LoaiMatHang { get; set; }
        public int Level { get; set; }
        public string DacTinh { get; set; }
        public string TenDonViTinh { get; set; }
        public decimal? GiaBinhQuanKyTruoc { get; set; }
        public List<HangHoaGiaThiTruongDto> MatHangCon { get; set; } = new List<HangHoaGiaThiTruongDto>();
    }
}
