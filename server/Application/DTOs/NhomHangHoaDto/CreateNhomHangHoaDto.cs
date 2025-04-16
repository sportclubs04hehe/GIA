using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.NhomHangHoaDto
{
    public class CreateNhomHangHoaDto
    {
        public required string MaNhom { get; set; }
        public required string TenNhom { get; set; }
        public string? GhiChu { get; set; }

        // Cho phép gán nhóm cha nếu có
        public Guid? NhomChaId { get; set; }
    }

}
