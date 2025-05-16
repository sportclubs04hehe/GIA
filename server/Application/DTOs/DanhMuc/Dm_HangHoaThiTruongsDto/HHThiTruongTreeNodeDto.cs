using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto
{
    public class HHThiTruongTreeNodeDto
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int LoaiMatHang { get; set; }
        public Guid? MatHangChaId { get; set; }
        public List<HHThiTruongTreeNodeDto> Children { get; set; } = new List<HHThiTruongTreeNodeDto>();
    }
}
