using Core.Entities.IdentityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain.DanhMuc
{
    [Table("HangHoa")]
    public class Dm_HangHoa : BaseIdentity
    {
        public required string MaMatHang { get; set; } 
        public required string TenMatHang { get; set; } 
        public string? GhiChu { get; set; }
        public string? DacTinh { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        public Guid? NhomHangHoaId { get; set; } = null!;
        public virtual Dm_NhomHangHoa? NhomHangHoa { get; set; }

        // Add the relationship to DonViTinh
        public Guid? DonViTinhId { get; set; }
        public virtual Dm_DonViTinh? DonViTinh { get; set; }
    }
}
