using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.NghiepVu.ThuThapGiaChiTiet
{
    public class ThuThapGiaChiTietDto :BaseDto
    {
        public Guid ThuThapGiaThiTruongId { get; set; }
        public Guid HangHoaThiTruongId { get; set; }
        public string TenHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string DonViTinh { get; set; }
        public decimal? GiaPhoBienKyBaoCao { get; set; }
        public decimal? GiaBinhQuanKyTruoc { get; set; }
        public decimal? GiaBinhQuanKyNay { get; set; }
        public decimal? MucTangGiamGiaBinhQuan { get; set; }
        public decimal? TyLeTangGiamGiaBinhQuan { get; set; }
        public string NguonThongTin { get; set; }
        public string GhiChu { get; set; }
    }
}
