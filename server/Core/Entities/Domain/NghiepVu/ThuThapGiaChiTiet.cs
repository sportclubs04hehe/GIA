using Core.Entities.Domain.DanhMuc;
using Core.Entities.IdentityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain.NghiepVu
{
    public class ThuThapGiaChiTiet : BaseIdentity
    {
        // Quan hệ với ThuThapGiaThiTruong
        public Guid ThuThapGiaThiTruongId { get; set; }
        [ForeignKey("ThuThapGiaThiTruongId")]
        public virtual ThuThapGiaThiTruong ThuThapGiaThiTruong { get; set; }
        
        // Quan hệ với Dm_HangHoaThiTruong - mặt hàng được thu thập giá
        public Guid HangHoaThiTruongId { get; set; }
        [ForeignKey("HangHoaThiTruongId")]
        public virtual Dm_HangHoaThiTruong HangHoaThiTruong { get; set; }
        
        public decimal? GiaPhoBienKyBaoCao { get; set; }

        // Giá bình quân kỳ trước
        public decimal? GiaBinhQuanKyTruoc { get; set; }

        // Giá bình quân kỳ này
        public decimal? GiaBinhQuanKyNay { get; set; }

        // Mức tăng (giảm) giá bình quân
        public decimal? MucTangGiamGiaBinhQuan { get; set; }

        // Tỷ lệ tăng giảm giá bình quân (%)
        public decimal? TyLeTangGiamGiaBinhQuan { get; set; }

        // Nguồn thông tin
        public string? NguonThongTin { get; set; }

        // Ghi chú
        public string? GhiChu { get; set; }
    }
}
