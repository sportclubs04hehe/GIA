using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.Dm_ThuocTinhDto
{
    public class Dm_ThuocTinhCreateManyDto
    {
        public List<Dm_ThuocTinhCreateDto> ThuocTinhs { get; set; } = new List<Dm_ThuocTinhCreateDto>();
    }
}
