using Core.Entities.IdentityBase;

namespace Core.Entities.Domain.DanhMuc
{
    public class Dm_DonViTinh : BaseIdentity
    {
        public required string Ma { get; set; }
        public required string Ten { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        
        // Navigation property for the relationship
        public virtual ICollection<Dm_HangHoa> HangHoas { get; set; } = new List<Dm_HangHoa>();
    }
}
