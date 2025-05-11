using Application.DTOs.DanhMuc.NhomHangHoasDto;
using Application.Mappings;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.Enum;
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
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedList<NhomHangHoaDto>> GetAllAsync(PaginationParams paginationParams)
        {
            // Get paged data
            var pagedData = await _repository.GetAllAsync(paginationParams);

            // Map to DTOs using extension method
            return pagedData.MapTo<Dm_NhomHangHoa, NhomHangHoaDto>(_mapper);
        }

        public async Task<NhomHangHoaDto> AddAsync(CreateNhomHangHoaDto createNhomHangHoaDto)
        {
            // Kiểm tra mã nhóm đã tồn tại chưa
            if (await _repository.ExistsByMaNhomAsync(createNhomHangHoaDto.MaNhom))
            {
                throw new Exception($"Mã nhóm '{createNhomHangHoaDto.MaNhom}' đã tồn tại");
            }

            // Kiểm tra quan hệ cha-con hợp lệ nếu có nhóm cha
            if (createNhomHangHoaDto.NhomChaId.HasValue)
            {
                var parentId = createNhomHangHoaDto.NhomChaId.Value;
                
                // Kiểm tra nhóm cha tồn tại
                if (!await _repository.ExistsAsync(parentId))
                {
                    throw new Exception($"Nhóm cha với ID: {parentId} không tồn tại");
                }
                
                // Kiểm tra nhóm cha có thể làm cha của loại nhóm này không
                if (!await _repository.CanBeParentAsync(parentId, createNhomHangHoaDto.LoaiNhom))
                {
                    throw new Exception("Nhóm cha không thể chứa loại nhóm con này");
                }
            }

            // Map to entity
            var entity = _mapper.Map<Dm_NhomHangHoa>(createNhomHangHoaDto);
            
            // Add to repository
            var result = await _repository.AddAsync(entity);
            
            // Map back to DTO and return
            return _mapper.Map<NhomHangHoaDto>(result);
        }

        public async Task<bool> UpdateAsync(UpdateNhomHangHoaDto updateNhomHangHoaDto)
        {
            // Kiểm tra tồn tại
            if (!await _repository.ExistsAsync(updateNhomHangHoaDto.Id))
            {
                throw new Exception($"Nhóm hàng hóa với ID: {updateNhomHangHoaDto.Id} không tồn tại");
            }

            // Kiểm tra mã nhóm đã tồn tại chưa
            var existingEntity = await _repository.GetByMaNhomAsync(updateNhomHangHoaDto.MaNhom);
            if (existingEntity != null && existingEntity.Id != updateNhomHangHoaDto.Id)
            {
                throw new Exception($"Mã nhóm '{updateNhomHangHoaDto.MaNhom}' đã được sử dụng bởi nhóm khác");
            }

            // Kiểm tra quan hệ cha-con hợp lệ nếu có nhóm cha
            if (updateNhomHangHoaDto.NhomChaId.HasValue)
            {
                var parentId = updateNhomHangHoaDto.NhomChaId.Value;
                
                // Không thể gán chính nó làm cha
                if (parentId == updateNhomHangHoaDto.Id)
                {
                    throw new Exception("Không thể gán nhóm làm cha của chính nó");
                }
                
                // Kiểm tra nhóm cha tồn tại
                if (!await _repository.ExistsAsync(parentId))
                {
                    throw new Exception($"Nhóm cha với ID: {parentId} không tồn tại");
                }
                
                // Kiểm tra nhóm cha có thể làm cha của loại nhóm này không
                if (!await _repository.CanBeParentAsync(parentId, updateNhomHangHoaDto.LoaiNhom))
                {
                    throw new Exception("Nhóm cha không thể chứa loại nhóm con này");
                }
                
                // Kiểm tra không tạo chu trình
                await CheckForCycles(updateNhomHangHoaDto.Id, parentId);
            }

            // Map to entity
            var entity = _mapper.Map<Dm_NhomHangHoa>(updateNhomHangHoaDto);

            // Update and return result
            return await _repository.UpdateAsync(entity);
        }

        // Kiểm tra chu trình trong cây phân cấp
        private async Task CheckForCycles(Guid childId, Guid parentId)
        {
            // Kiểm tra nếu nhóm con là cha của nhóm cha (trực tiếp hoặc gián tiếp)
            var currentId = parentId;
            var visitedIds = new HashSet<Guid>();
            
            while (currentId != Guid.Empty && !visitedIds.Contains(currentId))
            {
                visitedIds.Add(currentId);
                
                var parent = await _repository.GetByIdAsync(currentId);
                if (parent == null) break;
                
                if (parent.NhomChaId == childId)
                {
                    throw new Exception("Không thể tạo chu trình trong cấu trúc phân cấp");
                }
                
                currentId = parent.NhomChaId ?? Guid.Empty;
            }
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
            return pagedData.MapTo<Dm_NhomHangHoa, NhomHangHoaDto>(_mapper);
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
            return pagedData.MapTo<Dm_NhomHangHoa, NhomHangHoaDto>(_mapper);
        }

        public async Task<List<NhomHangHoaDto>> GetByLoaiNhomAsync(LoaiNhom loaiNhom)
        {
            var entities = await _repository.GetByLoaiNhomAsync(loaiNhom);
            return _mapper.Map<List<NhomHangHoaDto>>(entities);
        }
        
        public async Task<List<NhomHangHoaDto>> GetChildGroupsByLoaiNhomAsync(Guid parentId, LoaiNhom loaiNhom)
        {
            if (!await _repository.ExistsAsync(parentId))
            {
                throw new Exception($"Nhóm cha với ID: {parentId} không tồn tại");
            }
            
            var entities = await _repository.GetChildGroupsByLoaiNhomAsync(parentId, loaiNhom);
            return _mapper.Map<List<NhomHangHoaDto>>(entities);
        }
        
        public async Task<bool> CanBeParentAsync(Guid parentId, LoaiNhom childLoaiNhom)
        {
            return await _repository.CanBeParentAsync(parentId, childLoaiNhom);
        }
        
        public async Task<HierarchyDto> GetHierarchyWithLoaiNhomAsync()
        {
            var hierarchyData = await _repository.GetHierarchyWithLoaiNhomAsync();
            
            // Chuyển đổi kết quả thành cấu trúc phù hợp với DTO phân cấp
            var result = new HierarchyDto
            {
                Roots = _mapper.Map<List<HierarchyNodeDto>>(hierarchyData)
            };
            
            return result;
        }
        
        public async Task<bool> IsParentOfAnyGroupAsync(Guid id)
        {
            return await _repository.IsParentOfAnyGroupAsync(id);
        }
        
        public async Task<bool> HasProductsAsync(Guid id)
        {
            return await _repository.HasProductsAsync(id);
        }
    }
}
