using Application.DTOs.DanhMuc.HangHoasDto;
using Application.DTOs.DanhMuc.NhomHangHoasDto;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;

namespace Application.ServiceImplement.DanhMuc
{
    public class NhomHangHoaService : INhomHangHoaService
    {
        private readonly INhomHangHoaRepository _repository;
        private readonly IMapper _mapper;

        public NhomHangHoaService(INhomHangHoaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<NhomHangHoaDto> CreateNhomHangHoaAsync(CreateNhomHangHoaDto createDto)
        {
            // Kiểm tra mã nhóm trùng lặp trong cùng nhóm cha
            if (await _repository.IsMaNhomExistsInSameParentAsync(createDto.MaNhom, createDto.NhomChaId))
            {
                throw new Exception($"Mã nhóm '{createDto.MaNhom}' đã tồn tại trong cùng nhóm cha");
            }
            
            var entity = _mapper.Map<Dm_NhomHangHoa>(createDto);
            await _repository.AddAsync(entity);
            
            return _mapper.Map<NhomHangHoaDto>(entity);
        }

        public async Task<NhomHangHoaDto> UpdateNhomHangHoaAsync(Guid id, UpdateNhomHangHoaDto updateDto)
        {
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
                throw new Exception($"Nhóm hàng hóa với ID {id} không tồn tại");
            
            _mapper.Map(updateDto, existingEntity);
            
            var success = await _repository.UpdateAsync(existingEntity);

            if (!success)
            {
                var refreshedEntity = await _repository.GetByIdAsync(id);
            }
            
            return _mapper.Map<NhomHangHoaDto>(existingEntity);
        }

        public async Task<List<NhomHangHoaDto>> GetRootNodesAsync()
        {
            var rootNodes = await _repository.GetRootNodesAsync();
            return _mapper.Map<List<NhomHangHoaDto>>(rootNodes);
        }

        public async Task<NhomHangHoaDto> GetNhomHangHoaByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<NhomHangHoaDto>(entity) : null;
        }

        public async Task<PagedList<NhomHangHoaDto>> GetAllNhomHangHoasAsync(PaginationParams paginationParams)
        {
            var entitiesPage = await _repository.GetAllAsync(paginationParams);
            
            var dtos = _mapper.Map<List<NhomHangHoaDto>>(entitiesPage);
            
            return new PagedList<NhomHangHoaDto>(
                dtos,
                entitiesPage.TotalCount,
                entitiesPage.CurrentPage,
                entitiesPage.PageSize);
        }

        public async Task<PagedList<NhomHangHoaDto>> SearchNhomHangHoasAsync(SearchParams searchParams)
        {
            var entitiesPage = await _repository.SearchAsync(
                searchParams,
                x => x.MaNhom,
                x => x.TenNhom);
                
            var dtos = _mapper.Map<List<NhomHangHoaDto>>(entitiesPage);
            
            return new PagedList<NhomHangHoaDto>(
                dtos,
                entitiesPage.TotalCount,
                entitiesPage.CurrentPage,
                entitiesPage.PageSize);
        }

        public async Task<List<NhomHangHoaDto>> GetChildNhomHangHoasAsync(Guid parentId)
        {
            var children = await _repository.GetChildGroupsAsync(parentId);
            return _mapper.Map<List<NhomHangHoaDto>>(children);
        }

        public async Task<NhomHangHoaDetailDto> GetNhomHangHoaWithChildrenAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return null;

            var detailDto = _mapper.Map<NhomHangHoaDetailDto>(entity);

            // Load direct children
            var children = await _repository.GetChildGroupsAsync(id);
            var childrenDtos = _mapper.Map<List<NhomHangHoaDto>>(children);
            
            // For each child, recursively load its children
            foreach (var childDto in childrenDtos)
            {
                await LoadChildrenRecursivelyAsync(childDto);
            }
            
            detailDto.NhomCon = childrenDtos;
            return detailDto;
        }

        // New helper method to recursively load the tree
        public async Task LoadChildrenRecursivelyAsync(NhomHangHoaDto parentDto)
        {
            if (parentDto == null || parentDto.Id == Guid.Empty)
                return;
                
            // Change this line - don't parse the Id, it's already a Guid
            var children = await _repository.GetChildGroupsAsync(parentDto.Id);
            var childrenDtos = _mapper.Map<List<NhomHangHoaDto>>(children);
            
            parentDto.NhomCon = childrenDtos;
            
            // Recursively load children for each child
            foreach (var childDto in childrenDtos)
            {
                await LoadChildrenRecursivelyAsync(childDto);
            }
        }

        public async Task<PagedList<HangHoaDto>> GetAllProductsInGroupAsync(Guid groupId, PaginationParams paginationParams)
        {
            var productsPage = await _repository.GetAllProductsInGroupAsync(groupId, paginationParams);
            
            var dtos = _mapper.Map<List<HangHoaDto>>(productsPage);
            
            return new PagedList<HangHoaDto>(
                dtos,
                productsPage.TotalCount,
                productsPage.CurrentPage,
                productsPage.PageSize);
        }
    }
}
