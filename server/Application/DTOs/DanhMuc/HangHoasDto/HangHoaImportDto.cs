using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.HangHoasDto
{
    public class HangHoaImportDto
    {
        [Required(ErrorMessage = "Mã mặt hàng là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã mặt hàng không được vượt quá 50 ký tự")]
        public string MaMatHang { get; set; }

        [Required(ErrorMessage = "Tên mặt hàng là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên mặt hàng không được vượt quá 200 ký tự")]
        public string TenMatHang { get; set; }

        [StringLength(500)]
        public string? GhiChu { get; set; }

        [StringLength(500)]
        public string? dacTinh { get; set; }

        [Required(ErrorMessage = "Ngày hiệu lực là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime NgayHieuLuc { get; set; }

        [Required(ErrorMessage = "Ngày hết hiệu lực là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime NgayHetHieuLuc { get; set; }

        public string? NhomHangHoaMa { get; set; }
        
        [Required(ErrorMessage = "Đơn vị tính là bắt buộc")]
        public string DonViTinhTen { get; set; }
    }
}
