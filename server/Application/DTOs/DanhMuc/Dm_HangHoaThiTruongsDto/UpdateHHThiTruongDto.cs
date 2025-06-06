using Application.Resolver;
using Core.Entities.Domain.DanhMuc.Enum;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto
{
    public class UpdateHHThiTruongDto : BaseDto
    {
        [Required(ErrorMessage = "Mã mặt hàng không được để trống")]
        [MaxLength(50, ErrorMessage = "Mã mặt hàng không được vượt quá 50 ký tự")]
        public string Ma { get; set; }

        [Required(ErrorMessage = "Tên mặt hàng không được để trống")]
        [MaxLength(250, ErrorMessage = "Tên mặt hàng không được vượt quá 250 ký tự")]
        public string Ten { get; set; }

        public string? GhiChu { get; set; }
        
        [DateRangeValidation("NgayHetHieuLuc", "Ngày hiệu lực không được lớn hơn ngày hết hiệu lực")]
        public DateTime NgayHieuLuc { get; set; }
        
        public DateTime NgayHetHieuLuc { get; set; }

        [Required(ErrorMessage = "Loại mặt hàng không được để trống")]
        public Loai LoaiMatHang { get; set; }

        public Guid? MatHangChaId { get; set; }

        // Thuộc tính cho hàng hóa
        public string? DacTinh { get; set; }
        public Guid? DonViTinhId { get; set; }
    }
}
