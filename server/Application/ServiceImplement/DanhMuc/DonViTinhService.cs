﻿using Application.DTOs.DanhMuc.DonViTinhDto;
using Application.Mappings;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Microsoft.Extensions.Logging;

namespace Application.ServiceImplement.DanhMuc
{
    public class DonViTinhService : IDonViTinhService
    {
        private readonly IDonViTinhRepository _repository;
        private readonly IMapper _mapper;

        public DonViTinhService(
            IDonViTinhRepository repository,
            IMapper mapper,
            ILogger<DonViTinhService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DonViTinhsDto> CreateAsync(DonViTinhCreateDto createDto)
        {
            var entity = _mapper.Map<Dm_DonViTinh>(createDto);

            // Save to database
            var result = await _repository.AddAsync(entity);

            // Map the result back to DTO
            return _mapper.Map<DonViTinhsDto>(result);
        }

        public async Task<IEnumerable<DonViTinhsDto>> CreateManyAsync(IEnumerable<DonViTinhCreateDto> createDtos)
        {
            var entities = new List<Dm_DonViTinh>();

            foreach (var createDto in createDtos)
            {
                // Validate uniqueness of Ma for each entity
                if (!await _repository.IsMaUniqueAsync(createDto.Ma))
                    throw new InvalidOperationException($"Mã đơn vị tính '{createDto.Ma}' đã tồn tại.");

                var entity = _mapper.Map<Dm_DonViTinh>(createDto);
                entities.Add(entity);
            }

            var results = await _repository.AddRangeAsync(entities);
            return _mapper.Map<IEnumerable<DonViTinhsDto>>(results);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<bool> ExistsByMaAsync(
            string maMatHang, 
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            return await _repository.ExistsByMaAsync(maMatHang, excludeId, cancellationToken);
        }

        public async Task<PagedList<DonViTinhsDto>> GetAllAsync(PaginationParams paginationParams)
        {
            var entities = await _repository.GetAllAsync(paginationParams);
            return entities.MapTo<Dm_DonViTinh, DonViTinhsDto>(_mapper);
        }

        public async Task<PagedList<DonViTinhSelectDto>> GetAllSelectAsync(PaginationParams paginationParams)
        {
            var query = _repository.GetActive()
                .Select(d => new DonViTinhSelectDto
                {
                    Id = d.Id,
                    Ma = d.Ma,
                    Ten = d.Ten
                });

            if (string.IsNullOrWhiteSpace(paginationParams.OrderBy))
                query = query.OrderBy(x => x.Ten);
            else
                query = query.OrderByProperty(paginationParams.OrderBy, paginationParams.SortDescending);

            return await PagedList<DonViTinhSelectDto>.CreateAsync(
                query,
                paginationParams.PageIndex,
                paginationParams.PageSize
            );
        }
        public async Task<DonViTinhsDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<DonViTinhsDto>(entity);
        }

        public async Task<DonViTinhsDto> GetByMaAsync(string ma)
        {
            var entity = await _repository.GetByMaAsync(ma);
            return _mapper.Map<DonViTinhsDto>(entity);
        }

        public async Task<PagedList<DonViTinhsDto>> SearchAsync(SearchParams searchParams)
        {
            var entities = await _repository.SearchByNameAsync(searchParams);
            return entities.MapTo<Dm_DonViTinh, DonViTinhsDto>(_mapper);
        }

        public async Task<(bool isSuccess, string? errorMessage)> UpdateAsync(DonViTinhUpdateDto updateDto)
        {
            // Check if entity exists
            if (!await _repository.ExistsAsync(updateDto.Id))
                return (false, $"Đơn vị tính với ID {updateDto.Id} không tồn tại.");

            // Check for unique Ma
            if (!await _repository.IsMaUniqueAsync(updateDto.Ma, updateDto.Id))
                return (false, $"Mã đơn vị tính '{updateDto.Ma}' đã tồn tại.");

            // Map and update
            var entity = _mapper.Map<Dm_DonViTinh>(updateDto);
            var result = await _repository.UpdateAsync(entity);

            return result
                ? (true, null)
                : (false, "Cập nhật đơn vị tính không thành công.");
        }

        private async Task<(bool IsValid, string ErrorMessage)> ValidateHangHoaAsync(DonViTinhsDto dto, bool isUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(dto.Ma))
            {
                return (false, "Mã không được để trống.");
            }
            else if (string.IsNullOrWhiteSpace(dto.Ten))
            {
                return (false, "Tên không được để trống.");
            }

            if (!isUpdate)
            {
                var exists = await _repository.ExistsByMaAsync(dto.Ma, excludeId: null);
                if (exists)
                {
                    return (false, $"Mã đã tồn tại.");
                }
            }
            
            if(dto.NgayHieuLuc > dto.NgayHetHieuLuc)
            {
                return (false, "Ngày hiệu lực không được lớn hơn ngày hết hiệu lực.");
            }

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateCreateAsync(DonViTinhCreateDto dto)
        {
            var donViTinhDto = _mapper.Map<DonViTinhsDto>(dto);
            return await ValidateHangHoaAsync(donViTinhDto);
        }
    }
}
