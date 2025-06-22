using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class ThuThapGiaThiTruongBulkCreateDto
    {
        [Required(ErrorMessage = "Ngày thu thập là bắt buộc")]
        public DateTime NgayThuThap { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Loại giá là bắt buộc")]
        public Guid LoaiGiaId { get; set; }

        public string? NguonThongTin { get; set; }

        [Required(ErrorMessage = "Danh sách giá hàng hóa là bắt buộc")]
        public List<HangHoaGiaCreateDto> DanhSachGiaHangHoa { get; set; } = new List<HangHoaGiaCreateDto>();
    }

    public class HangHoaGiaCreateDto
    {
        [Required(ErrorMessage = "Hàng hóa là bắt buộc")]
        public Guid HangHoaId { get; set; }

        public decimal? GiaPhoBienKyBaoCao { get; set; }
        public decimal? GiaBinhQuanKyNay { get; set; }
        public string? GhiChu { get; set; }
    }
}
