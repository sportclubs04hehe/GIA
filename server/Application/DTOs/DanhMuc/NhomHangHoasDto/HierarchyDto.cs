using Core.Entities.Domain.Enum;

namespace Application.DTOs.DanhMuc.NhomHangHoasDto
{
    public class HierarchyDto
    {
        public List<HierarchyNodeDto> Roots { get; set; } = new List<HierarchyNodeDto>();
    }

    public class HierarchyNodeDto : BaseDto
    {
        public string MaNhom { get; set; }
        public string TenNhom { get; set; }
        public LoaiNhom LoaiNhom { get; set; }
        public string? GhiChu { get; set; }
        public Guid? NhomChaId { get; set; }
        public List<HierarchyNodeDto> Children { get; set; } = new List<HierarchyNodeDto>();
    }
}