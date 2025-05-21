using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto
{
    public class CategoryInfoDto : HHThiTruongDto
    {
        public bool HasChildren { get; set; }
    }
}
