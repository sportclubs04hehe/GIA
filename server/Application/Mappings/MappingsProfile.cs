using Application.DTOs.DanhMuc.HangHoasDto;
using Application.DTOs.DanhMuc.NhomHangHoasDto;
using Application.Resolver;
using AutoMapper;
using Core.Entities.Domain;

namespace Application.Mappings
{
    public class MappingsProfile : Profile
    {
        public MappingsProfile()
        {
            // NhomHangHoa mappings
            CreateMap<NhomHangHoa, NhomHangHoaDto>();
            CreateMap<NhomHangHoaDto, NhomHangHoa>();

            // HangHoa mappings
            CreateMap<HangHoa, HangHoaDto>();
            CreateMap<HangHoaDto, HangHoa>();

            // HangHoaCreateDto mappings
            CreateMap<HangHoaCreateDto, HangHoaDto>();
            CreateMap<HangHoaCreateDto, HangHoa>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc));
            CreateMap<HangHoaUpdateDto, HangHoaDto>();
            CreateMap<HangHoaUpdateDto, HangHoa>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc));

            // NhomHangHoa mappings
            CreateMap<CreateNhomHangHoaDto, NhomHangHoaDto>();
            CreateMap<CreateNhomHangHoaDto, NhomHangHoa>();
            CreateMap<UpdateNhomHangHoaDto, NhomHangHoaDto>();
            CreateMap<UpdateNhomHangHoaDto, NhomHangHoa>();
        }
    }
}
