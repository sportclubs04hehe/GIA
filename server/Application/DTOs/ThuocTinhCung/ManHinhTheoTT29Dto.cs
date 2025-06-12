using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ThuocTinhCung
{
    public class ManHinhTheoTT29DTO : BaseDto
    {
        // Giá phổ biến kỳ báo cáo
        public decimal GiaPhoBienKyBaoCao { get; set; }

        // Giá bình quân kỳ trước
        public decimal GiaBinhQuanKyTruoc { get; set; }

        // Giá bình quân kỳ này
        public decimal GiaBinhQuanKyNay { get; set; }

        // Mức tăng (giảm) giá bình quân
        public decimal MucTangGiamGiaBinhQuan { get; set; }

        // Tỷ lệ tăng (giảm) giá bình quân (%)
        public decimal TyLeTangGiamGiaBinhQuan { get; set; }

        // Nguồn thông tin
        public string NguonThongTin { get; set; }

        // Ghi chú
        public string GhiChu { get; set; }
    }
}
