using Core.Entities.Domain.DanhMuc.Enum;
using Core.Entities.IdentityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain.DanhMuc
{
    [Table("Dm_ThuocTinh")]
    public class Dm_ThuocTinh : BaseIdentity
    {
        // Thuộc tính chung
        public required string Stt { get; set; }
        public required string Ma { get; set; }
        public required string Ten { get; set; }
        public string? GhiChu { get; set; }
        public string? DinhDang { get; set; }
        public string? Width { get; set; }   
        public string? CongThuc { get; set; }
        public string? CanChinhCot { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        public Loai Loai { get; set; }

        // Quan hệ phân cấp
        public Guid? ThuocTinhChaId { get; set; }

        // Navigation properties
        [ForeignKey("ThuocTinhChaId")]  // Sửa tên ForeignKey từ MatHangChaId thành ThuocTinhChaId
        public virtual Dm_ThuocTinh? ThuocTinhCha { get; set; }
        public virtual ICollection<Dm_ThuocTinh> ThuocTinhCon { get; set; } = new List<Dm_ThuocTinh>();
        
        // Thêm collection navigation property để liên kết với Dm_HangHoaThiTruong
        public virtual ICollection<Dm_HangHoaThiTruong> HangHoaThiTruongs { get; set; } = new List<Dm_HangHoaThiTruong>();
    }
}
