
namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class ThuThapGiaThiTruongCreateDto
    {
        public int? Tuan { get; set; }
        public int? Nam { get; set; }
        public DateTime? NgayNhap { get; set; }
        public Guid LoaiGiaId { get; set; }
        public Guid? NhomHangHoaId { get; set; }
        public int LoaiNghiepVu { get; set; }
    }
}
