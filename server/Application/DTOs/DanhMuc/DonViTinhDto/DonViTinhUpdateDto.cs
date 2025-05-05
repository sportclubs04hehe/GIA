using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.DonViTinhDto
{
    public class DonViTinhUpdateDto : BaseDto
    {

        [Required, StringLength(20, ErrorMessage = "Mã không được vượt quá 20 ký tự")]
        public string Ma { get; set; } = null!;

        [Required, StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Ten { get; set; } = null!;

        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Ngày hiệu lực là bắt buộc")]
        public DateTime NgayHieuLuc { get; set; }

        [Required(ErrorMessage = "Ngày hết hiệu lực là bắt buộc")]
        public DateTime NgayHetHieuLuc { get; set; }
    }
}
