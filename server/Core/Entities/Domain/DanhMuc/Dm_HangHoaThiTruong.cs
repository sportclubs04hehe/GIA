﻿using Core.Entities.Domain.DanhMuc.Enum;
using Core.Entities.Domain.NghiepVu;
using Core.Entities.IdentityBase;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain.DanhMuc
{
    [Table("Dm_HangHoaThiTruong")]
    public class Dm_HangHoaThiTruong : BaseIdentity
    {
        // Thuộc tính chung
        public required string Ma { get; set; }
        public required string Ten { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        [NotMapped]
    public int Level { get; set; } = 0;

        // Phân loại: 0 = Nhóm, 1 = Hàng hóa
        public Loai LoaiMatHang { get; set; }

        // Quan hệ phân cấp
        public Guid? MatHangChaId { get; set; }

        // Thuộc tính đặc trưng cho hàng hóa
        public string? DacTinh { get; set; }
        public Guid? DonViTinhId { get; set; }

        // Navigation properties
        [ForeignKey("MatHangChaId")]
        public virtual Dm_HangHoaThiTruong? MatHangCha { get; set; }
        public virtual ICollection<Dm_HangHoaThiTruong> MatHangCon { get; set; } = new List<Dm_HangHoaThiTruong>();
        [ForeignKey("DonViTinhId")]
        public virtual Dm_DonViTinh? DonViTinh { get; set; }

        // Thêm navigation property đến Dm_ThuocTinh
        public Guid? ThuocTinhId { get; set; }
        [ForeignKey("ThuocTinhId")]
        public virtual Dm_ThuocTinh? ThuocTinh { get; set; }

        // Quan hệ một-nhiều với ThuThapGiaChiTiet
        public virtual ICollection<ThuThapGiaChiTiet> ChiTietGia { get; set; } = new List<ThuThapGiaChiTiet>();
    }
}
