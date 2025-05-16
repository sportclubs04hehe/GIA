using Core.Entities.IdentityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain.DanhMuc
{
    [Table("DonViTinh")]
    public class Dm_DonViTinh : BaseIdentity
    {
        public required string Ma { get; set; }
        public required string Ten { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        
        // Thêm collection cho Dm_MatHang mới
        public virtual ICollection<Dm_HangHoaThiTruong> MatHangs { get; set; } = new List<Dm_HangHoaThiTruong>();
        
        // Giữ lại navigation property cho Dm_HangHoa cũ trong giai đoạn chuyển đổi
        public virtual ICollection<Dm_HangHoa> HangHoas { get; set; } = new List<Dm_HangHoa>();
    }
}
