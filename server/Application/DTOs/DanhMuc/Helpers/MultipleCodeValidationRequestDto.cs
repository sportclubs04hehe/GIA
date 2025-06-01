using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.Helpers
{
    /// <summary>
    /// DTO yêu cầu kiểm tra nhiều mã cùng lúc
    /// </summary>
    public class MultipleCodeValidationRequestDto
    {
        /// <summary>
        /// Danh sách mã cần kiểm tra
        /// </summary>
        [Required(ErrorMessage = "Danh sách mã không được để trống")]
        public List<string> Codes { get; set; } = new List<string>();

        /// <summary>
        /// ID nhóm cha để kiểm tra (null nếu là cấp cao nhất)
        /// </summary>
        public Guid? ParentId { get; set; }
    }
}