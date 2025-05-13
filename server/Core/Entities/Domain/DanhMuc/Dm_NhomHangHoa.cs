using Core.Entities.IdentityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain.DanhMuc
{
    [Table("NhomHangHoa")]
    public class Dm_NhomHangHoa : BaseIdentity
    {
        public required string MaNhom { get; set; }
        public required string TenNhom { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        public Guid? NhomChaId { get; set; }

        [ForeignKey("NhomChaId")]
        public virtual Dm_NhomHangHoa NhomCha { get; set; }
        public virtual ICollection<Dm_NhomHangHoa> NhomCon { get; set; }
        public virtual ICollection<Dm_HangHoa> HangHoas { get; set; }
    }
}
