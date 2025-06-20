﻿using Application.DTOs.NghiepVu.ThuThapGiaThiTruong;
using Application.ServiceInterface.INghiepVu;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.NghiepVu;
using Core.Helpers;
using Core.Interfaces.IRepository.INghiepVu;

namespace Application.ServiceImplement.NghiepVu
{
    public class ThuThapGiaThiTruongService : IThuThapGiaThiTruongService
    {
        private readonly IThuThapGiaThiTruongRepository _repository;
        private readonly IMapper _mapper;

        public ThuThapGiaThiTruongService(
            IThuThapGiaThiTruongRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ThuThapGiaThiTruongDto> CreateAsync(ThuThapGiaThiTruongCreateDto createDto)
        {
            var entity = _mapper.Map<ThuThapGiaThiTruong>(createDto);

            var result = await _repository.AddAsync(entity);
            
            return _mapper.Map<ThuThapGiaThiTruongDto>(result);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<PagedList<ThuThapGiaThiTruongDto>> GetAllAsync(PaginationParams paginationParams)
        {
            var entities = await _repository.GetAllAsync(paginationParams);
            
            var dtos = _mapper.Map<List<ThuThapGiaThiTruongDto>>(entities);
            
            return new PagedList<ThuThapGiaThiTruongDto>(
                dtos,
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );
        }

        public async Task<ThuThapGiaThiTruongDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            
            return _mapper.Map<ThuThapGiaThiTruongDto>(entity);
        }

        public async Task<PagedList<ThuThapGiaThiTruongDto>> SearchAsync(SearchParams searchParams)
        {
            var entities = await _repository.SearchAsync(searchParams);
            
            var dtos = _mapper.Map<List<ThuThapGiaThiTruongDto>>(entities);
            
            return new PagedList<ThuThapGiaThiTruongDto>(
                dtos,
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );
        }

        public async Task<bool> UpdateAsync(ThuThapGiaThiTruongUpdateDto updateDto)
        {
            var entity = await _repository.GetByIdAsync(updateDto.Id);
            if (entity == null) return false;
            
            _mapper.Map(updateDto, entity);
            
            return await _repository.UpdateAsync(entity);
        }

        public async Task<List<HangHoaGiaThiTruongDto>> GetHierarchicalDataWithPreviousPricesAsync(
          Guid parentId,
          DateTime ngayThuThap,
          Guid loaiGiaId)
        {
            var hierarchicalData = await _repository.GetHierarchicalDataWithPreviousPricesAsync(
                parentId, ngayThuThap, loaiGiaId);

            var result = _mapper.Map<List<HangHoaGiaThiTruongDto>>(hierarchicalData);
            
            // Lấy giá kỳ trước và set vào DTO
            var hangHoaIds = GetAllHangHoaIds(result);
            var previousPrices = await _repository.GetPreviousPricesAsync(hangHoaIds, ngayThuThap, loaiGiaId);
            
            SetPreviousPrices(result, previousPrices);
            
            return result;
        }

        private List<Guid> GetAllHangHoaIds(List<HangHoaGiaThiTruongDto> items)
        {
            var ids = new List<Guid>();
            foreach (var item in items)
            {
                ids.Add(item.Id);
                if (item.MatHangCon?.Any() == true)
                {
                    ids.AddRange(GetAllHangHoaIds(item.MatHangCon));
                }
            }
            return ids;
        }

        private void SetPreviousPrices(List<HangHoaGiaThiTruongDto> items, Dictionary<Guid, decimal?> previousPrices)
        {
            foreach (var item in items)
            {
                if (previousPrices.TryGetValue(item.Id, out var price))
                {
                    item.GiaBinhQuanKyTruoc = price;
                }

                if (item.MatHangCon?.Any() == true)
                {
                    SetPreviousPrices(item.MatHangCon, previousPrices);
                }
            }
        }
    }
}
