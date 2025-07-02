using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class ThuThapGiaThiTruongDto : BaseDto
    {
        public int? Tuan { get; set; }
        public int Nam { get; set; }
        public DateTime? NgayNhap { get; set; }
        public Guid LoaiGiaId { get; set; }
        public string TenLoaiGia { get; set; }
        public Guid? NhomHangHoaId { get; set; }
        public string TenNhomHangHoa { get; set; }
        public int LoaiNghiepVu { get; set; }
    }
}
