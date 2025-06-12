using Application.DTOs.DanhMuc.Dm_LoaiGia;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceImplement.DanhMuc
{
    public class Dm_LoaiGiaService : IDm_LoaiGiaService
    {
        private readonly IDm_LoaiGiaRepository _repository;
        private readonly IMapper _mapper;

        public Dm_LoaiGiaService(
            IDm_LoaiGiaRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<LoaiGiaDto> CreateAsync(LoaiGiaCreateDto createDto)
        {
            var entity = _mapper.Map<Dm_LoaiGia>(createDto);
            var result = await _repository.AddAsync(entity);
            return _mapper.Map<LoaiGiaDto>(result);
        }

        public async Task<bool> DeleteAsync(Guid id) => 
            await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => 
            await _repository.ExistsAsync(id);

        public async Task<PagedList<LoaiGiaDto>> GetAllAsync(PaginationParams paginationParams)
        {
            var entities = await _repository.GetAllAsync(paginationParams);
            var dtos = _mapper.Map<List<LoaiGiaDto>>(entities);
            
            return new PagedList<LoaiGiaDto>(
                dtos,
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );
        }

        public async Task<LoaiGiaDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<LoaiGiaDto>(entity);
        }

        public async Task<PagedList<LoaiGiaDto>> SearchAsync(SearchParams searchParams)
        {
            var entities = await _repository.SearchAsync(
                searchParams,
                e => e.Ma,
                e => e.Ten
            );
            
            var dtos = _mapper.Map<List<LoaiGiaDto>>(entities);
            
            return new PagedList<LoaiGiaDto>(
                dtos,
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );
        }

        public async Task<bool> UpdateAsync(Guid id, LoaiGiaUpdateDto updateDto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            
            _mapper.Map(updateDto, entity);
            return await _repository.UpdateAsync(entity);
        }
    }
}
