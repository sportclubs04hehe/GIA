using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto
{
    public class CreateManyHHThiTruongDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một mặt hàng thị trường để thêm")]
        public List<CreateHHThiTruongDto> Items { get; set; } = new List<CreateHHThiTruongDto>();
    }
}
