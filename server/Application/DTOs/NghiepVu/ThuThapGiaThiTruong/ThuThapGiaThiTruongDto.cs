using Core.Entities.Domain.DanhMuc.Enum;

namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class ThuThapGiaThiTruongDto : BaseDto
    {
        public DateTime NgayThuThap { get; set; }
        public Guid HangHoaId { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public Guid LoaiGiaId { get; set; }
        public string TenLoaiGia { get; set; }
        public LoaiNghiepVu LoaiNghiepVu { get; set; }
    }
}
