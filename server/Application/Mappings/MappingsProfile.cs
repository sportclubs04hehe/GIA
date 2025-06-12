using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.DTOs.DanhMuc.Dm_ThuocTinhDto;
using Application.DTOs.DanhMuc.DonViTinhDto;
using Application.Resolver;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;

namespace Application.Mappings
{
    public class MappingsProfile : Profile
    {
        public MappingsProfile()
        {
            #region DM Hàng hóa thị trường 
            CreateMap<Dm_HangHoaThiTruong, HHThiTruongDto>()
               .ForMember(d => d.TenMatHangCha, o => o.MapFrom(s => s.MatHangCha != null ? s.MatHangCha.Ten : null))
               .ForMember(d => d.TenDonViTinh, o => o.MapFrom(s => s.DonViTinh != null ? s.DonViTinh.Ten : null));

            CreateMap<CreateHHThiTruongDto, Dm_HangHoaThiTruong>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc));

            CreateMap<UpdateHHThiTruongDto, Dm_HangHoaThiTruong>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc));

            CreateMap<Dm_HangHoaThiTruong, HHThiTruongTreeNodeDto>()
                .ForMember(d => d.MatHangCon, o => o.MapFrom(s => s.MatHangCon.Where(c => !c.IsDelete)))
                .ForMember(d => d.TenDonViTinh, o => o.MapFrom(s => s.DonViTinh != null ? s.DonViTinh.Ten : null))
                .ForMember(d => d.DonViTinhId, o => o.MapFrom(s => s.DonViTinhId));

            CreateMap<Dm_HangHoaThiTruong, CategoryInfoDto>()
               .ForMember(d => d.TenMatHangCha, o => o.MapFrom(s => s.MatHangCha != null ? s.MatHangCha.Ten : null))
               .ForMember(d => d.TenDonViTinh, o => o.MapFrom(s => (string)null))
               .ForMember(d => d.DonViTinhId, o => o.MapFrom(s => (Guid?)null))
               .ForMember(d => d.HasChildren, o => o.Ignore());

            CreateMap<HHThiTruongImportDto, Dm_HangHoaThiTruong>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc))
                .ForMember(dest => dest.DonViTinhId, opt => opt.Ignore())
                .ForMember(dest => dest.DonViTinh, opt => opt.Ignore())
                .ForMember(dest => dest.MatHangChaId, opt => opt.Ignore())
                .ForMember(dest => dest.MatHangCha, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => false));
            #endregion

            #region DM Đơn vị tính
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
            #endregion

            #region DM Thuộc tính
            // Map entity to base DTO
            CreateMap<Dm_ThuocTinh, Dm_ThuocTinhDto>()
                .ForMember(d => d.TenThuocTinhCha, o => o.MapFrom(s => s.ThuocTinhCha != null ? s.ThuocTinhCha.Ten : null))
                .ForMember(d => d.MaThuocTinhCha, o => o.MapFrom(s => s.ThuocTinhCha != null ? s.ThuocTinhCha.Ma : null));

            // Map create DTO to entity
            CreateMap<Dm_ThuocTinhCreateDto, Dm_ThuocTinh>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc));

            // Map update DTO to entity
            CreateMap<Dm_ThuocTinhUpdateDto, Dm_ThuocTinh>()
                .ForMember(dest => dest.NgayHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHieuLuc))
                .ForMember(dest => dest.NgayHetHieuLuc,
                    opt => opt.MapFrom<UtcDateTimeResolver, DateTime>(src => src.NgayHetHieuLuc));

            // Map entity to tree node DTO
            CreateMap<Dm_ThuocTinh, Dm_ThuocTinhTreeNodeDto>()
                .ForMember(d => d.ThuocTinhCon, o => o.MapFrom(s => s.ThuocTinhCon.Where(c => !c.IsDelete)));

            // Map entity to category info DTO
            CreateMap<Dm_ThuocTinh, Dm_ThuocTinhCategoryInfoDto>()
                .ForMember(d => d.TenThuocTinhCha, o => o.MapFrom(s => s.ThuocTinhCha != null ? s.ThuocTinhCha.Ten : null))
                .ForMember(d => d.MaThuocTinhCha, o => o.MapFrom(s => s.ThuocTinhCha != null ? s.ThuocTinhCha.Ma : null))
                .ForMember(d => d.HasChildren, o => o.MapFrom(s => s.ThuocTinhCon.Any(c => !c.IsDelete)));

            #endregion
        }
    }
}
