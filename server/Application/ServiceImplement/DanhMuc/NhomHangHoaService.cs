using Application.DTOs.DanhMuc.NhomHangHoasDto;
using Application.Mappings;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using Core.Entities.Domain;
using Core.Helpers;
using Core.Interfaces.IRepository;

namespace Application.ServiceImplement.DanhMuc
{
    public class NhomHangHoaService : INhomHangHoaService
    {
        private readonly INhomHangHoaRepository _repository;
        private readonly IMapper _mapper;

        public NhomHangHoaService(INhomHangHoaRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedList<NhomHangHoaDto>> GetAllAsync(PaginationParams paginationParams)
        {
            // Get paged data
            var pagedData = await _repository.GetAllAsync(paginationParams);

            // Map to DTOs using extension method
            return pagedData.MapTo<NhomHangHoa, NhomHangHoaDto>(_mapper);
        }

        public async Task<NhomHangHoaDto> AddAsync(CreateNhomHangHoaDto createNhomHangHoaDto)
        {
            // Check if maNhom already exists
            if (await _repository.ExistsByMaNhomAsync(createNhomHangHoaDto.MaNhom))
            {
                throw new Exception($"Group code '{createNhomHangHoaDto.MaNhom}' already exists");
            }

            // Map to entity
            var entity = _mapper.Map<NhomHangHoa>(createNhomHangHoaDto);
            
            // Add to repository
            var result = await _repository.AddAsync(entity);
            
            // Map back to DTO and return
            return _mapper.Map<NhomHangHoaDto>(result);
        }

        public async Task<bool> UpdateAsync(UpdateNhomHangHoaDto updateNhomHangHoaDto)
        {
            // Check if exists
            if (!await _repository.ExistsAsync(updateNhomHangHoaDto.Id))
            {
                throw new Exception($"Product group with ID not found: {updateNhomHangHoaDto.Id}");
            }

            // Check if the updated MaNhom already exists for a different entity
            var existingEntity = await _repository.GetByMaNhomAsync(updateNhomHangHoaDto.MaNhom);
            if (existingEntity != null && existingEntity.Id != updateNhomHangHoaDto.Id)
            {
                throw new Exception($"Group code '{updateNhomHangHoaDto.MaNhom}' is already used by another product group");
            }

            // Map to entity
            var entity = _mapper.Map<NhomHangHoa>(updateNhomHangHoaDto);

            // Update and return result
            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // Check if exists
            if (!await _repository.ExistsAsync(id))
            {
                throw new Exception($"Product group with ID: {id} not found");
            }

            // Delete
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<bool> ExistsByMaNhomAsync(string maNhom)
        {
            return await _repository.ExistsByMaNhomAsync(maNhom);
        }

        public async Task<NhomHangHoaDto> GetByIdAsync(Guid id)
        {
            // Get entity
            var entity = await _repository.GetByIdAsync(id);
            
            if (entity == null)
            {
                throw new Exception($"Product group with ID: {id} not found");
            }
            
            // Map to DTO and return
            return _mapper.Map<NhomHangHoaDto>(entity);
        }

        public async Task<NhomHangHoaDto> GetByMaNhomAsync(string maNhom)
        {
            // Get entity
            var entity = await _repository.GetByMaNhomAsync(maNhom);
            
            if (entity == null)
            {
                throw new Exception($"No product group found with group code: {maNhom}");
            }
            
            // Map to DTO and return
            return _mapper.Map<NhomHangHoaDto>(entity);
        }

        public async Task<List<NhomHangHoaDto>> GetChildGroupsAsync(Guid parentId)
        {
            // Check if parent exists
            if (!await _repository.ExistsAsync(parentId))
            {
                throw new Exception($"No parent product group found with ID: {parentId}");
            }
            
            // Get child groups
            var entities = await _repository.GetChildGroupsAsync(parentId);
            
            // Map to DTOs and return
            return _mapper.Map<List<NhomHangHoaDto>>(entities);
        }

        public async Task<List<NhomHangHoaDto>> GetHierarchyAsync()
        {
            // Start with root groups
            var rootGroups = await _repository.GetRootGroupsAsync();
            
            // Map to DTOs
            var result = _mapper.Map<List<NhomHangHoaDto>>(rootGroups);
            
            // Return hierarchy
            return result;
        }

        public async Task<List<NhomHangHoaDto>> GetRootGroupsAsync()
        {
            // Get root groups
            var rootGroups = await _repository.GetRootGroupsAsync();
            
            // Map to DTOs and return
            return _mapper.Map<List<NhomHangHoaDto>>(rootGroups);
        }

        public async Task<PagedList<NhomHangHoaDto>> GetWithFilterAsync(SpecificationParams specParams)
        {
            // Get filtered data
            var pagedData = await _repository.GetFilteredAsync(specParams);
            
            // Map to DTOs using extension method
            return pagedData.MapTo<NhomHangHoa, NhomHangHoaDto>(_mapper);
        }

        public async Task<PagedList<NhomHangHoaDto>> SearchAsync(SearchParams searchParams)
        {
            // Create specification params
            var specParams = new SpecificationParams
            {
                PageIndex = searchParams.PageIndex,
                PageSize = searchParams.PageSize,
                SearchTerm = searchParams.SearchTerm
            };

            // Get filtered data
            var pagedData = await _repository.GetFilteredAsync(specParams);

            // Map to DTOs using extension method
            return pagedData.MapTo<NhomHangHoa, NhomHangHoaDto>(_mapper);
        }
    }
}
