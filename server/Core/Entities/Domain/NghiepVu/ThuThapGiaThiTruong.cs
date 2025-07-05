using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
using Core.Entities.IdentityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain.NghiepVu
{
    public class ThuThapGiaThiTruong : BaseIdentity
    {
        public int? Tuan { get; set; }
        public int Nam { get; set; } = 0;
        public DateTime? NgayNhap { get; set; }
        public LoaiNghiepVu LoaiNghiepVu { get; set; } = LoaiNghiepVu.HH29;
        
        // Quan hệ một-nhiều với Dm_LoaiGia
        public Guid LoaiGiaId { get; set; }
        [ForeignKey("LoaiGiaId")]
        public virtual Dm_LoaiGia LoaiGia { get; set; }

        // Nhóm hàng hóa được chọn để thu thập giá
        public Guid? NhomHangHoaId { get; set; }
        [ForeignKey("NhomHangHoaId")]
        public virtual Dm_HangHoaThiTruong NhomHangHoa { get; set; }

        // Quan hệ một-nhiều với ThuThapGiaChiTiet
        public virtual ICollection<ThuThapGiaChiTiet> ChiTietGia { get; set; } = new List<ThuThapGiaChiTiet>();
    }
}
