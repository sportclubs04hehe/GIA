using Core.Entities.IdentityBase;

namespace Core.Entities.Domain.DanhMuc
{
    public class Dm_LoaiGia: BaseIdentity
    {
        public required string Ma { get; set; }
        public required string Ten { get; set; }
    }
}
