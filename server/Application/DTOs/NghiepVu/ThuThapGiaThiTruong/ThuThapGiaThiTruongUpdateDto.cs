using Core.Entities.Domain.DanhMuc.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class ThuThapGiaThiTruongUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Ngày thu thập là bắt buộc")]
        public DateTime NgayThuThap { get; set; }

        [Required(ErrorMessage = "Hàng hóa là bắt buộc")]
        public Guid HangHoaId { get; set; }

        [Required(ErrorMessage = "Loại giá là bắt buộc")]
        public Guid LoaiGiaId { get; set; }

        [Required(ErrorMessage = "Loại nghiệp vụ là bắt buộc")]
        public LoaiNghiepVu LoaiNghiepVu { get; set; }
    }
}
