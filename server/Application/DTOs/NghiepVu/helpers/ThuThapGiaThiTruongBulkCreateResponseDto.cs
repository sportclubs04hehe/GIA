using Application.DTOs.NghiepVu.ThuThapGiaThiTruong;

namespace Application.DTOs.NghiepVu.helpers
{
    public class ThuThapGiaThiTruongBulkCreateResponseDto
    {
        public int TotalCreated { get; set; }
        public int TotalSkipped { get; set; }
        public List<ThuThapGiaThiTruongDto> CreatedItems { get; set; } = new List<ThuThapGiaThiTruongDto>();
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
