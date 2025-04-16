using Core.Entities.IdentityBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain
{
    [Table("NhomHangHoa")]
    public class NhomHangHoa : BaseIdentity
    {
        public required string MaNhom { get; set; }
        public required string TenNhom { get; set; }
        public string? GhiChu { get; set; }

        // 🔁 Cha của nhóm này
        public Guid? NhomChaId { get; set; }

        [ForeignKey("NhomChaId")]
        public virtual NhomHangHoa NhomCha { get; set; }

        // 🔁 Các nhóm con trực tiếp
        public virtual ICollection<NhomHangHoa> NhomCon { get; set; }

        // 🔗 Hàng hóa trong nhóm
        public virtual ICollection<HangHoa> HangHoas { get; set; }
    }
}
