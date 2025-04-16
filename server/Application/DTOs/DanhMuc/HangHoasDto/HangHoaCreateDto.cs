using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.HangHoasDto
{
    public class HangHoaCreateDto : BaseDto
    {
        [Required]
        [StringLength(50)]
        public string MaMatHang { get; set; }

        [Required]
        [StringLength(400)]
        public string TenMatHang { get; set; }
        
        [StringLength(500)]
        public string? GhiChu { get; set; }
        
        [Required]
        public DateTime NgayHieuLuc { get; set; }
        
        [Required]
        public DateTime NgayHetHieuLuc { get; set; }
        
        public Guid? NhomHangHoaId { get; set; }
    }
}
