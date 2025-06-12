using Core.Entities.Domain.NghiepVu;
using Core.Entities.IdentityBase;

namespace Core.Entities.Domain.DanhMuc
{
    public class Dm_LoaiGia: BaseIdentity
    {
        public required string Ma { get; set; }
        public required string Ten { get; set; }
        
        // Thêm navigation property để tham chiếu ngược lại
        public virtual ICollection<ThuThapGiaThiTruong> ThuThapGiaThiTruongs { get; set; } = new List<ThuThapGiaThiTruong>();
    }
}
