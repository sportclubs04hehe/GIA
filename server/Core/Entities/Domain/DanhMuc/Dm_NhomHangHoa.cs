﻿using Core.Entities.IdentityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain.DanhMuc
{
    [Table("NhomHangHoa")]
    public class Dm_NhomHangHoa : BaseIdentity
    {
        public required string MaNhom { get; set; }
        public required string TenNhom { get; set; }
        public string? GhiChu { get; set; }

        // 🔁 Cha của nhóm này
        public Guid? NhomChaId { get; set; }

        [ForeignKey("NhomChaId")]
        public virtual Dm_NhomHangHoa NhomCha { get; set; }

        // 🔁 Các nhóm con trực tiếp
        public virtual ICollection<Dm_NhomHangHoa> NhomCon { get; set; }

        // 🔗 Hàng hóa trong nhóm
        public virtual ICollection<Dm_HangHoa> HangHoas { get; set; }
    }
}
