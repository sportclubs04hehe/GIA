using Application.Resolver;
using Core.Entities.Domain.DanhMuc.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto
{
    public class CreateHHThiTruongDto
    {
        [Required(ErrorMessage = "Mã mặt hàng không được để trống")]
        [MaxLength(50, ErrorMessage = "Mã mặt hàng không được vượt quá 50 ký tự")]
        public string Ma { get; set; }

        [Required(ErrorMessage = "Tên mặt hàng không được để trống")]
        [MaxLength(250, ErrorMessage = "Tên mặt hàng không được vượt quá 250 ký tự")]
        public string Ten { get; set; }

        public string? GhiChu { get; set; }
        
        [DateRangeValidation("NgayHetHieuLuc", "Ngày hiệu lực không được lớn hơn ngày hết hiệu lực")]
        public DateTime NgayHieuLuc { get; set; } = DateTime.Now;
        
        public DateTime NgayHetHieuLuc { get; set; } = DateTime.Now.AddYears(5);

        [Required(ErrorMessage = "Loại mặt hàng không được để trống")]
        public LoaiMatHangEnum LoaiMatHang { get; set; }

        public Guid? MatHangChaId { get; set; }

        // Thuộc tính cho hàng hóa
        public string? DacTinh { get; set; }
        public Guid? DonViTinhId { get; set; }
    }
}
