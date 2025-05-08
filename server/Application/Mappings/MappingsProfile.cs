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
            #region Hàng hóa thị trường mappings
            // Entity → DTO
            CreateMap<Dm_HangHoa, HangHoaDto>()
                .ForMember(dest => dest.NhomHangHoaId, opt => opt.MapFrom(src => src.NhomHangHoaId))
                .ForMember(dest => dest.DonViTinhId, opt => opt.MapFrom(src => src.DonViTinhId))
                .ForMember(dest => dest.DonViTinhDto, opt => opt.MapFrom(src => src.DonViTinh));

            // CreateDto → Entity
            CreateMap<HangHoaCreateDto, Dm_HangHoa>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc))
                .ForMember(dest => dest.DonViTinhId,
                    opt => opt.MapFrom(src => src.DonViTinhId))
                .ForMember(dest => dest.DonViTinh, opt => opt.Ignore());

            // UpdateDto → Entity
            CreateMap<HangHoaUpdateDto, Dm_HangHoa>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc))
                .ForMember(dest => dest.DonViTinhId,
                    opt => opt.MapFrom(src => src.DonViTinhId))
                .ForMember(dest => dest.DonViTinh, opt => opt.Ignore());

            // CreateDto → DTO
            CreateMap<HangHoaCreateDto, HangHoaDto>()
                .ForMember(dest => dest.DonViTinhId,
                    opt => opt.MapFrom(src => src.DonViTinhId));

            // UpdateDto → DTO
            CreateMap<HangHoaUpdateDto, HangHoaDto>()
                .ForMember(dest => dest.DonViTinhId,
                    opt => opt.MapFrom(src => src.DonViTinhId));

            // DonViTinh Entity → DTO
            CreateMap<Dm_DonViTinh, DonViTinhsDto>();

            #endregion

            // NhomHangHoa mappings
            CreateMap<CreateNhomHangHoaDto, NhomHangHoaDto>();
            CreateMap<CreateNhomHangHoaDto, Dm_NhomHangHoa>();
            CreateMap<UpdateNhomHangHoaDto, NhomHangHoaDto>();
            CreateMap<UpdateNhomHangHoaDto, Dm_NhomHangHoa>();

            //DonViTinh mappings
            CreateMap<Dm_DonViTinh, DonViTinhsDto>()
                .ForMember(d => d.HangHoaIds, o => o.MapFrom(s => s.HangHoas.Select(h => h.Id).ToList()));
            CreateMap<Dm_DonViTinh, DonViTinhSelectDto>();
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
            CreateMap<DonViTinhCreateDto, DonViTinhsDto>();
        }
    }
}
