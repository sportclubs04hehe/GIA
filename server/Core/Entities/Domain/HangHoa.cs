using Core.Entities.IdentityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain
{
    [Table("HangHoa")]
    public class HangHoa : BaseIdentity
    {
        public required string MaMatHang { get; set; } 
        public required string TenMatHang { get; set; } 
        public string? GhiChu { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        public Guid? NhomHangHoaId { get; set; }
        public virtual NhomHangHoa? NhomHangHoa { get; set; }
    }
}
