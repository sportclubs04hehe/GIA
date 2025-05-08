using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.DonViTinhDto
{
    public class DonViTinhSelectDto
    {
        public Guid Id { get; set; }
        public string Ten { get; set; } = null!;
        public string Ma { get; set; } = null!;
    }
}
