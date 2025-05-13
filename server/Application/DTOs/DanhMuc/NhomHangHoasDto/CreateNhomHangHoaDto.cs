using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.NhomHangHoasDto
{
    public class CreateNhomHangHoaDto : BaseDto
    {
        [Required]
        public string MaNhom { get; set; }
        [Required]
        public string TenNhom { get; set; }
        public string? GhiChu { get; set; }
        [Required]
        [Display(Name = "Ngày hiệu lực")]
        [DataType(DataType.Date)]
        public DateTime NgayHieuLuc { get; set; }

        [Required]
        [Display(Name = "Ngày hết hiệu lực")]
        [DataType(DataType.Date)]
        public DateTime NgayHetHieuLuc { get; set; }

        // Cho phép gán nhóm cha nếu có
        public Guid? NhomChaId { get; set; }
    }

}
