﻿using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.DTOs.DanhMuc.Dm_LoaiGia;
using Application.DTOs.DanhMuc.Dm_ThuocTinhDto;
using Application.DTOs.DanhMuc.DonViTinhDto;
using Application.DTOs.NghiepVu.ThuThapGiaChiTiet;
using Application.DTOs.NghiepVu.ThuThapGiaThiTruong;
using Application.Resolver;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.NghiepVu;

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

            #region DM Loại Giá
            // Map entity to DTO
            CreateMap<Dm_LoaiGia, LoaiGiaDto>();

            // Map create DTO to entity
            CreateMap<LoaiGiaCreateDto, Dm_LoaiGia>();

            // Map update DTO to entity
            CreateMap<LoaiGiaUpdateDto, Dm_LoaiGia>();
            #endregion

            #region Thu Thập Giá Thị Trường
            // Map entity to DTO
            CreateMap<ThuThapGiaThiTruong, ThuThapGiaThiTruongDto>()
                .ForMember(d => d.TenLoaiGia, o => o.MapFrom(s => s.LoaiGia.Ten))
                .ForMember(d => d.TenNhomHangHoa, o => o.MapFrom(s => s.NhomHangHoa != null ? s.NhomHangHoa.Ten : null));

            // Map create DTO to entity
            CreateMap<ThuThapGiaThiTruongCreateDto, ThuThapGiaThiTruong>()
                .ForMember(dest => dest.NgayNhap,
                    opt => opt.MapFrom<NullableUtcDateTimeResolver, DateTime?>(src => src.NgayNhap));

            // Map update DTO to entity
            CreateMap<ThuThapGiaThiTruongUpdateDto, ThuThapGiaThiTruong>()
                .ForMember(dest => dest.NgayNhap,
                    opt => opt.MapFrom<NullableUtcDateTimeResolver, DateTime?>(src => src.NgayNhap));
            #endregion

            #region Thu Thập Giá Chi Tiết
            // Map entity to DTO
            CreateMap<ThuThapGiaChiTiet, ThuThapGiaChiTietDto>()
                .ForMember(d => d.TenHangHoa, o => o.MapFrom(s => s.HangHoaThiTruong.Ten))
                .ForMember(d => d.MaHangHoa, o => o.MapFrom(s => s.HangHoaThiTruong.Ma))
                .ForMember(d => d.DonViTinh, o => o.MapFrom(s => s.HangHoaThiTruong.DonViTinh != null ? s.HangHoaThiTruong.DonViTinh.Ten : null));

            // Map create DTO to entity
            CreateMap<ThuThapGiaChiTietCreateDto, ThuThapGiaChiTiet>();

            // Map update DTO to entity
            CreateMap<ThuThapGiaChiTietUpdateDto, ThuThapGiaChiTiet>();
            #endregion
        }
    }
}
