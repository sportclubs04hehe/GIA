namespace Application.DTOs.DanhMuc.DonViTinhDto
{
    public class DonViTinhDto : BaseDto
    {
        public string Ten { get; set; } = null!;
        public string Ma { get; set; } = null!;
        public string? GhiChu { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
        public List<Guid>? HangHoaIds { get; set; }
    }
}
