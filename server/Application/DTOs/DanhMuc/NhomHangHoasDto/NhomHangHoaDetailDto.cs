using Application.DTOs.DanhMuc.HangHoasDto;

namespace Application.DTOs.DanhMuc.NhomHangHoasDto
{
    public class NhomHangHoaDetailDto : NhomHangHoaDto
    {
        public string? NhomChaName { get; set; }
        public List<NhomHangHoaDto> NhomCon { get; set; } = new();
        public List<HangHoaDto> HangHoas { get; set; } = new();
    }
}
