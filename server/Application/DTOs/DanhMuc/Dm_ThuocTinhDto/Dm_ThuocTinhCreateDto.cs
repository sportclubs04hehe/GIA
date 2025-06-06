using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.Dm_ThuocTinhDto
{
    public class Dm_ThuocTinhCreateDto
    {
        [Required]
        public string Stt { get; set; }

        [Required]
        public string Ma { get; set; }

        [Required]
        public string Ten { get; set; }

        public string GhiChu { get; set; }
        public string DinhDang { get; set; }
        public string Width { get; set; }
        public string CongThuc { get; set; }
        public string CanChinhCot { get; set; }
        public DateTime NgayHieuLuc { get; set; } = DateTime.Now;
        public DateTime NgayHetHieuLuc { get; set; } = DateTime.Now.AddYears(10);
        public Guid? ThuocTinhChaId { get; set; }
    }
}
