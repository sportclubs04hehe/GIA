namespace Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto
{
    /// <summary>
    /// Kết quả kiểm tra mã mặt hàng thị trường
    /// </summary>
    public class CodeValidationResult
    {
        /// <summary>
        /// Mã có hợp lệ không
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Mã đã kiểm tra
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// ID nhóm cha
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Thông báo chi tiết
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// ID của mục đang được loại trừ khỏi kiểm tra (khi cập nhật)
        /// </summary>
        public Guid? ExceptId { get; set; }
    }
}