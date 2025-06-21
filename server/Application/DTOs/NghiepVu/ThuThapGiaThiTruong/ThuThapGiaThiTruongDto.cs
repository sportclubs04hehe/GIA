using Core.Entities.Domain.DanhMuc.Enum;

namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class ThuThapGiaThiTruongDto : BaseDto
    {
        public DateTime NgayThuThap { get; set; }
        public Guid HangHoaId { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public Guid LoaiGiaId { get; set; }
        public string TenLoaiGia { get; set; }

        // Add these fields
        public decimal? GiaPhoBienKyBaoCao { get; set; }
        public decimal? GiaBinhQuanKyNay { get; set; }
        public string? NguonThongTin { get; set; }
        public string? GhiChu { get; set; }
        public decimal? GiaBinhQuanKyTruoc { get; set; }
        public decimal? MucTangGiam { get; set; }
        public decimal? TyLeTangGiam { get; set; }
    }
}
