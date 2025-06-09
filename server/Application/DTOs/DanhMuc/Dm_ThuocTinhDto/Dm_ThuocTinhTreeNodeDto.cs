using Core.Entities.Domain.DanhMuc.Enum;
using System;
using System.Collections.Generic;

namespace Application.DTOs.DanhMuc.Dm_ThuocTinhDto
{
    public class Dm_ThuocTinhTreeNodeDto
    {
        public Guid Id { get; set; }
        public string Stt { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public Loai Loai { get; set; }
        public string GhiChu { get; set; }
        public string DinhDang { get; set; }
        public string Width { get; set; }
        public string CongThuc { get; set; }
        public string CanChinhCot { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        public Guid? ThuocTinhChaId { get; set; }

        // Danh sách các thuộc tính con
        public List<Dm_ThuocTinhTreeNodeDto> ThuocTinhCon { get; set; } = new List<Dm_ThuocTinhTreeNodeDto>();

        // Danh sách các hàng hóa liên kết (nếu cần)
        public List<HangHoaInfoDto> HangHoas { get; set; } = new List<HangHoaInfoDto>();
    }

    public class HangHoaInfoDto
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
    }
}
