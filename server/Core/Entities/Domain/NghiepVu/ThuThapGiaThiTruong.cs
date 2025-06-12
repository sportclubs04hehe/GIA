using Core.Entities.IdentityBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;

namespace Core.Entities.Domain.NghiepVu
{
    public class ThuThapGiaThiTruong : BaseIdentity
    {
        public DateTime NgayThuThap { get; set; }
        
        // Quan hệ với hàng hóa
        public Guid HangHoaId { get; set; }
        [ForeignKey("HangHoaId")]
        public virtual Dm_HangHoaThiTruong HangHoa { get; set; }
        
        // Quan hệ với loại giá
        public Guid LoaiGiaId { get; set; }
        [ForeignKey("LoaiGiaId")]
        public virtual Dm_LoaiGia LoaiGia { get; set; }
        
        // Thuộc tính loại nghiệp vụ
        public LoaiNghiepVu LoaiNghiepVu { get; set; }
    }
}
