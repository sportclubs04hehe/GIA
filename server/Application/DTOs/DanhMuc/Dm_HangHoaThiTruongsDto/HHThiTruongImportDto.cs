using Core.Entities.Domain.DanhMuc.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto
{
    public class HHThiTruongImportDto
    {
        [Required(ErrorMessage = "Mã mặt hàng là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã mặt hàng không được vượt quá 50 ký tự")]
        public string Ma { get; set; }

        [Required(ErrorMessage = "Tên mặt hàng là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên mặt hàng không được vượt quá 200 ký tự")]
        public string Ten { get; set; }

        [StringLength(500)]
        public string? GhiChu { get; set; }

        // Phân loại: 0 = Nhóm, 1 = Hàng hóa
        [Required(ErrorMessage = "Loại mặt hàng là bắt buộc")]
        public Loai LoaiMatHang { get; set; }

        // Thuộc tính đặc trưng cho hàng hóa
        [StringLength(500)]
        public string? DacTinh { get; set; }

        [Required(ErrorMessage = "Ngày hiệu lực là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime NgayHieuLuc { get; set; }

        [Required(ErrorMessage = "Ngày hết hiệu lực là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime NgayHetHieuLuc { get; set; }

        // Đơn vị tính (chỉ áp dụng cho loại hàng hóa)
        public string? DonViTinhTen { get; set; }
    }
}