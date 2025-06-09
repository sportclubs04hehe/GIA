using Application.Resolver;
using Core.Entities.Domain.DanhMuc.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DanhMuc.Dm_ThuocTinhDto
{
    public class Dm_ThuocTinhUpdateDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Stt { get; set; }

        [Required]
        public string Ma { get; set; }

        [Required]
        public string Ten { get; set; }

        [Required]
        public Loai Loai { get; set; }

        public string GhiChu { get; set; }
        public string DinhDang { get; set; }
        public string Width { get; set; }
        public string CongThuc { get; set; }
        public string CanChinhCot { get; set; }

        [DateRangeValidation("NgayHetHieuLuc", "Ngày hiệu lực không được lớn hơn ngày hết hiệu lực")]
        public DateTime NgayHieuLuc { get; set; } = DateTime.Now;
        public DateTime NgayHetHieuLuc { get; set; } = DateTime.Now.AddYears(5);
        public Guid? ThuocTinhChaId { get; set; }
    }
}
