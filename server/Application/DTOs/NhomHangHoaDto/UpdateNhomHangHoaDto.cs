﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.NhomHangHoaDto
{
    public class UpdateNhomHangHoaDto
    {
        public required string MaNhom { get; set; }
        public required string TenNhom { get; set; }
        public string? GhiChu { get; set; }

        // Cho phép cập nhật lại nhóm cha nếu cần
        public Guid? NhomChaId { get; set; }
    }
}
