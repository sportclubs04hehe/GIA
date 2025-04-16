using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.NhomHangHoasDto
{
    public class NhomHangHoaDto : BaseDto
    {
        [Required(ErrorMessage = "Mã nhóm là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã nhóm không được vượt quá 50 ký tự")]
        [Display(Name = "Mã nhóm")]
        public string MaNhom { get; set; }

        [Required(ErrorMessage = "Tên nhóm là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên nhóm không được vượt quá 200 ký tự")]
        [Display(Name = "Tên nhóm")]
        public string TenNhom { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Nhóm cha")]
        public Guid? NhomChaId { get; set; }
    }
}
