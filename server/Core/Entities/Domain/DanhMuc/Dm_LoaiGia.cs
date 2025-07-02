using Core.Entities.Domain.NghiepVu;
using Core.Entities.IdentityBase;
using System;
using System.Collections.Generic;

namespace Core.Entities.Domain.DanhMuc
{
    public class Dm_LoaiGia: BaseIdentity
    {
        public required string Ma { get; set; }
        public required string Ten { get; set; }

        // Quan hệ một-nhiều với ThuThapGiaThiTruong
        public virtual ICollection<ThuThapGiaThiTruong> ThuThapGiaThiTruongs { get; set; } = new List<ThuThapGiaThiTruong>();
    }
}
