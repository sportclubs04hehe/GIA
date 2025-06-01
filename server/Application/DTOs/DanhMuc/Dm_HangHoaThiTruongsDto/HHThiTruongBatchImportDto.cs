using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto
{
    public class HHThiTruongBatchImportDto
    {
        [Required(ErrorMessage = "ID nhóm cha là bắt buộc")]
        public Guid? MatHangChaId { get; set; }

        [Required(ErrorMessage = "Danh sách mặt hàng không được để trống")]
        public List<HHThiTruongImportDto> Items { get; set; } = new();
    }
}