using Application.DTOs.DanhMuc.DonViTinhDto;
using Application.DTOs.DanhMuc.HangHoasDto;
using Application.DTOs.DanhMuc.NhomHangHoasDto;
using Application.Resolver;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;

namespace Application.Mappings
{
    public class MappingsProfile : Profile
    {
        public MappingsProfile()
        {
            // NhomHangHoa mappings
            CreateMap<Dm_NhomHangHoa, NhomHangHoaDto>();
            CreateMap<NhomHangHoaDto, Dm_NhomHangHoa>();

            // HangHoa mappings
            CreateMap<Dm_HangHoa, HangHoaDto>();
            CreateMap<HangHoaDto, Dm_HangHoa>();

            // HangHoaCreateDto mappings
            CreateMap<HangHoaCreateDto, HangHoaDto>();
            CreateMap<HangHoaCreateDto, Dm_HangHoa>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc));
            CreateMap<HangHoaUpdateDto, HangHoaDto>();
            CreateMap<HangHoaUpdateDto, Dm_HangHoa>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc));

            // NhomHangHoa mappings
            CreateMap<CreateNhomHangHoaDto, NhomHangHoaDto>();
            CreateMap<CreateNhomHangHoaDto, Dm_NhomHangHoa>();
            CreateMap<UpdateNhomHangHoaDto, NhomHangHoaDto>();
            CreateMap<UpdateNhomHangHoaDto, Dm_NhomHangHoa>();

            //DonViTinh mappings
            CreateMap<Dm_DonViTinh, DonViTinhDto>()
                .ForMember(d => d.HangHoaIds, o => o.MapFrom(s => s.HangHoas.Select(h => h.Id).ToList()));
            CreateMap<DonViTinhCreateDto, Dm_DonViTinh>()
                  .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc));
            CreateMap<DonViTinhUpdateDto, Dm_DonViTinh>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc));
            CreateMap<DonViTinhCreateDto, DonViTinhDto>();
        }
    }
}
