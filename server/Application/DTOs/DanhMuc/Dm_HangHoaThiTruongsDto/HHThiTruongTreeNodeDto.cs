using System;
using System.Collections.Generic;

namespace Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto
{
    public class HHThiTruongTreeNodeDto
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string GhiChu { get; set; }
        public string DacTinh { get; set; }
        public int LoaiMatHang { get; set; }
        public Guid? MatHangChaId { get; set; }
        // Thêm thông tin đơn vị tính
        public Guid? DonViTinhId { get; set; }
        public string? TenDonViTinh { get; set; }
        
        // Đổi tên từ Children thành MatHangCon để khớp với tên được sử dụng trong code
        public List<HHThiTruongTreeNodeDto> MatHangCon { get; set; } = new List<HHThiTruongTreeNodeDto>();
    }
}
