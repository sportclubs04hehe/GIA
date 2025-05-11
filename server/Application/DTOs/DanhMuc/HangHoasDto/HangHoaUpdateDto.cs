using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.HangHoasDto
{
    public class HangHoaUpdateDto : BaseDto
    {
        [Required]
        [StringLength(50)]
        public string MaMatHang { get; set; }

        [Required]
        [StringLength(400)]
        public string TenMatHang { get; set; }
        
        [StringLength(500)]
        public string? GhiChu { get; set; }

        [StringLength(500)]
        public string? DacTinh { get; set; }

        [Required]
        public DateTime NgayHieuLuc { get; set; }
        
        [Required]
        public DateTime NgayHetHieuLuc { get; set; }
        public Guid? NhomHangHoaId { get; set; }
        public Guid? DonViTinhId { get; set; }
    }
}
