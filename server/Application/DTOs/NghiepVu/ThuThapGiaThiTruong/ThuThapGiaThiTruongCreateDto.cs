using Core.Entities.Domain.DanhMuc.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class ThuThapGiaThiTruongCreateDto : BaseDto
    {
        [Required(ErrorMessage = "Ngày thu thập là bắt buộc")]
        public DateTime NgayThuThap { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Hàng hóa là bắt buộc")]
        public Guid HangHoaId { get; set; }

        [Required(ErrorMessage = "Loại giá là bắt buộc")]
        public Guid LoaiGiaId { get; set; }
        public decimal? GiaPhoBienKyBaoCao { get; set; }
        public decimal? GiaBinhQuanKyNay { get; set; }
        public string? NguonThongTin { get; set; }
        public string? GhiChu { get; set; }
    }
}
