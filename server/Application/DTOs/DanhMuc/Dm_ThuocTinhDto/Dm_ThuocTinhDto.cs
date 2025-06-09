using Application.DTOs;
using Core.Entities.Domain.DanhMuc.Enum;
using System;

namespace Application.DTOs.DanhMuc.Dm_ThuocTinhDto
{
    public class Dm_ThuocTinhDto : BaseDto
    {
        public string Stt { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string GhiChu { get; set; }
        public string DinhDang { get; set; }
        public string Width { get; set; }
        public string CongThuc { get; set; }
        public string CanChinhCot { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        public Loai Loai { get; set; }
        public Guid? ThuocTinhChaId { get; set; }
        public string TenThuocTinhCha { get; set; }
        public string MaThuocTinhCha { get; set; }
    }
}
