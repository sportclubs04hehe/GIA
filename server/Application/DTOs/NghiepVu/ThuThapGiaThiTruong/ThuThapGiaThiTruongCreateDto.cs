using Core.Entities.Domain.DanhMuc.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.NghiepVu.ThuThapGiaThiTruong
{
    public class ThuThapGiaThiTruongCreateDto : BaseDto
    {
        [Required(ErrorMessage = "Ngày thu thập là bắt buộc")]
        public DateTime NgayThuThap { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Hàng hóa là bắt buộc")]
        public Guid HangHoaId { get; set; }

        [Required(ErrorMessage = "Loại giá là bắt buộc")]
        public Guid LoaiGiaId { get; set; }

        [Required(ErrorMessage = "Loại nghiệp vụ là bắt buộc")]
        public LoaiNghiepVu LoaiNghiepVu { get; set; }
    }
}
