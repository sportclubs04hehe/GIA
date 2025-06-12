using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.Dm_LoaiGia
{
    public class LoaiGiaUpdateDto
    {
        [Required(ErrorMessage = "Mã loại giá là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã loại giá không được vượt quá 50 ký tự")]
        public string Ma { get; set; }

        [Required(ErrorMessage = "Tên loại giá là bắt buộc")]
        [StringLength(250, ErrorMessage = "Tên loại giá không được vượt quá 250 ký tự")]
        public string Ten { get; set; }
    }
}
