using Core.Entities.IdentityBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;

namespace Core.Entities.Domain.NghiepVu
{
    public class ThuThapGiaThiTruong : BaseIdentity
    {
        public DateTime NgayThuThap { get; set; }

        // Quan hệ với hàng hóa
        public Guid HangHoaId { get; set; }
        [ForeignKey("HangHoaId")]
        public virtual Dm_HangHoaThiTruong HangHoa { get; set; }

        // Quan hệ với loại giá
        public Guid LoaiGiaId { get; set; }
        [ForeignKey("LoaiGiaId")]
        public virtual Dm_LoaiGia LoaiGia { get; set; }

        // Các trường nhập liệu
        public decimal? GiaPhoBienKyBaoCao { get; set; }
        public decimal? GiaBinhQuanKyNay { get; set; }
        public string? NguonThongTin { get; set; }
        public string? GhiChu { get; set; }
        public decimal? GiaBinhQuanKyTruoc { get; set; } // Tự tính từ bản ghi kỳ trước (nếu có)
        public decimal? MucTangGiam { get; set; }        // = GiaBinhQuanKyNay - GiaBinhQuanKyTruoc
        public decimal? TyLeTangGiam { get; set; }       // = (MucTangGiam / GiaBinhQuanKyTruoc) * 100
    }
}
