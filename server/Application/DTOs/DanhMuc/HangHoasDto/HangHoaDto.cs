using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.HangHoasDto
{
    public class HangHoaDto : BaseDto
    {

        [Required(ErrorMessage = "Mã mặt hàng là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã mặt hàng không được vượt quá 50 ký tự")]
        [Display(Name = "Mã mặt hàng")]
        public string MaMatHang { get; set; }
        
        [Required(ErrorMessage = "Tên mặt hàng là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên mặt hàng không được vượt quá 200 ký tự")]
        [Display(Name = "Tên mặt hàng")]
        public string TenMatHang { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required]
        [Display(Name = "Ngày hiệu lực")]
        [DataType(DataType.Date)]
        public DateTime NgayHieuLuc { get; set; }
        
        [Required]
        [Display(Name = "Ngày hết hiệu lực")]
        [DataType(DataType.Date)]
        public DateTime NgayHetHieuLuc { get; set; }

        public Guid NhomHangHoaId { get; set; }
    }
}
