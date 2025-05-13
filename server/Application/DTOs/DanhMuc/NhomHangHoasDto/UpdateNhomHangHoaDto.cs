using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.NhomHangHoasDto
{
    public class UpdateNhomHangHoaDto : BaseDto
    {
        [Required]
        public string MaNhom { get; set; }
        [Required]
        public string TenNhom { get; set; }
        [Required]
        [Display(Name = "Ngày hiệu lực")]
        [DataType(DataType.Date)]
        public DateTime NgayHieuLuc { get; set; }

        [Required]
        [Display(Name = "Ngày hết hiệu lực")]
        [DataType(DataType.Date)]
        public DateTime NgayHetHieuLuc { get; set; }
        public string? GhiChu { get; set; }

        // Cho phép cập nhật lại nhóm cha nếu cần
        public Guid? NhomChaId { get; set; }
    }
}
