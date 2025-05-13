namespace Application.DTOs.DanhMuc.NhomHangHoasDto
{
    public class NhomHangHoaDto : BaseDto
    {
        public string MaNhom { get; set; }
        public string TenNhom { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        public string? GhiChu { get; set; }
        public Guid? NhomChaId { get; set; }
        public List<NhomHangHoaDto> NhomCon { get; set; } = new();
    }
}
