using Core.Entities.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DanhMuc.NhomHangHoasDto
{
    public class CreateNhomHangHoaDto : BaseDto
    {
        public required string MaNhom { get; set; }
        public required string TenNhom { get; set; }
        public LoaiNhom LoaiNhom { get; set; }
        public string? GhiChu { get; set; }

        // Cho phép gán nhóm cha nếu có
        public Guid? NhomChaId { get; set; }
    }

}
