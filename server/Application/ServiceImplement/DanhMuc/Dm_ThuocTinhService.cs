using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.DTOs.DanhMuc.Dm_ThuocTinhDto;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ServiceImplement.DanhMuc
{
    public class Dm_ThuocTinhService : IDm_ThuocTinhService
    {
        private readonly IDm_ThuocTinhRepository _repository;
        private readonly IMapper _mapper;

        public Dm_ThuocTinhService(IDm_ThuocTinhRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Lấy thuộc tính theo ID
        public async Task<Dm_ThuocTinhDto> GetByIdAsync(Guid id) =>
            _mapper.Map<Dm_ThuocTinhDto>(await _repository.GetByIdWithRelationsAsync(id));

        // Tạo thuộc tính mới
        public async Task<Dm_ThuocTinhDto> CreateAsync(Dm_ThuocTinhCreateDto createDto)
        {
            await ValidateCreateDtoAsync(createDto);
            var entity = _mapper.Map<Dm_ThuocTinh>(createDto);
            var result = await _repository.AddAsync(entity);
            return _mapper.Map<Dm_ThuocTinhDto>(result);
        }

        // Cập nhật thuộc tính
        public async Task<Dm_ThuocTinhDto> UpdateAsync(Dm_ThuocTinhUpdateDto updateDto)
        {
            await ValidateUpdateDtoAsync(updateDto);
            
            var existingEntity = await _repository.GetByIdAsync(updateDto.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"Thuộc tính có ID {updateDto.Id} không tồn tại");
            
            var entity = _mapper.Map(updateDto, existingEntity);
            await _repository.UpdateAsync(entity);
            
            return _mapper.Map<Dm_ThuocTinhDto>(await _repository.GetByIdWithRelationsAsync(entity.Id));
        }

        // Xóa thuộc tính và các thuộc tính con
        public async Task<bool> DeleteAsync(Guid id) => 
            await _repository.DeleteWithChildrenAsync(id);

        // Xóa nhiều thuộc tính
        public async Task<bool> DeleteMultipleAsync(List<Guid> ids)
        {
            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
                foreach (var id in ids)
                {
                    if (!await _repository.DeleteWithChildrenAsync(id))
                        throw new Exception($"Không thể xóa thuộc tính ID {id}");
                }
                
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Lấy tất cả thuộc tính cha
        public async Task<List<Dm_ThuocTinhDto>> GetAllParentCategoriesAsync() =>
            _mapper.Map<List<Dm_ThuocTinhDto>>(await _repository.GetAllParentCategoriesAsync());

        // Lấy tất cả thuộc tính kèm thông tin có con hay không
        public async Task<List<Dm_ThuocTinhCategoryInfoDto>> GetAllCategoriesWithChildInfoAsync()
        {
            var categoriesWithChildInfo = await _repository.GetAllCategoriesWithChildInfoAsync();
            return categoriesWithChildInfo.Select(tuple => {
                var dto = _mapper.Map<Dm_ThuocTinhCategoryInfoDto>(tuple.Category);
                dto.HasChildren = tuple.HasChildren;
                return dto;
            }).ToList();
        }

        // Kiểm tra mã có hợp lệ không
        public async Task<bool> IsValidCodeAsync(string ma, Guid? parentId, Guid? exceptId = null) =>
            !await _repository.ExistsByMaInSameLevelAsync(ma, parentId, exceptId);

        // Xác thực DTO tạo mới
        public async Task<bool> ValidateCreateDtoAsync(Dm_ThuocTinhCreateDto createDto)
        {
            if (!await IsValidCodeAsync(createDto.Ma, createDto.ThuocTinhChaId))
                throw new ArgumentException($"Mã '{createDto.Ma}' đã tồn tại trong cùng nhóm thuộc tính");
            
            if (createDto.NgayHieuLuc > createDto.NgayHetHieuLuc)
                throw new ArgumentException("Ngày hiệu lực không được lớn hơn ngày hết hiệu lực");
            
            return true;
        }

        // Xác thực DTO cập nhật
        public async Task<bool> ValidateUpdateDtoAsync(Dm_ThuocTinhUpdateDto updateDto)
        {
            if (!await IsValidCodeAsync(updateDto.Ma, updateDto.ThuocTinhChaId, updateDto.Id))
                throw new ArgumentException($"Mã '{updateDto.Ma}' đã tồn tại trong cùng nhóm thuộc tính");

            if (updateDto.NgayHieuLuc > updateDto.NgayHetHieuLuc)
                throw new ArgumentException("Ngày hiệu lực không được lớn hơn ngày hết hiệu lực");
            
            return true;
        }

        // Tạo nhiều thuộc tính
        public async Task<List<Dm_ThuocTinhDto>> CreateManyAsync(Dm_ThuocTinhCreateManyDto createDto)
        {
            var entities = new List<Dm_ThuocTinh>();
            
            foreach (var dto in createDto.ThuocTinhs)
            {
                await ValidateCreateDtoAsync(dto);
                entities.Add(_mapper.Map<Dm_ThuocTinh>(dto));
            }
            
            var result = await _repository.AddRangeWithValidationAsync(entities);
            return _mapper.Map<List<Dm_ThuocTinhDto>>(result);
        }

        // Tìm kiếm phân cấp
        public async Task<PagedList<Dm_ThuocTinhTreeNodeDto>> SearchHierarchicalAsync(string searchTerm, PaginationParams paginationParams)
        {
            var matchingItems = await _repository.SearchAllPagedAsync(
                searchTerm,
                paginationParams,
                x => x.Ma,
                x => x.Ten
            );

            if (!matchingItems.Any())
            {
                return new PagedList<Dm_ThuocTinhTreeNodeDto>(
                    new List<Dm_ThuocTinhTreeNodeDto>(),
                    0,
                    paginationParams.PageIndex,
                    paginationParams.PageSize
                );
            }

            // Thu thập parent IDs
            var itemIds = matchingItems.Select(x => x.Id).ToList();
            var parentIds = new HashSet<Guid>();

            foreach (var item in matchingItems.Where(x => x.ThuocTinhChaId.HasValue))
            {
                var currentId = item.ThuocTinhChaId.Value;
                var visited = new HashSet<Guid>();
                
                while (!visited.Contains(currentId))
                {
                    parentIds.Add(currentId);
                    visited.Add(currentId);
                    
                    var parent = await _repository.GetByIdNoTrackingAsync(currentId);
                    if (parent?.ThuocTinhChaId == null) break;
                    
                    currentId = parent.ThuocTinhChaId.Value;
                }
            }

            var rootItems = await _repository.GetRootItemsForSearchAsync(parentIds, itemIds);
            var treeNodes = _mapper.Map<List<Dm_ThuocTinhTreeNodeDto>>(rootItems);

            return new PagedList<Dm_ThuocTinhTreeNodeDto>(
                treeNodes,
                matchingItems.TotalCount,
                matchingItems.CurrentPage,
                matchingItems.PageSize
            );
        }

        // Lấy thuộc tính con phân trang theo ID cha
        public async Task<PagedList<Dm_ThuocTinhTreeNodeDto>> GetChildrenByParentIdPagedAsync(Guid parentId, PaginationParams paginationParams)
        {
            var entities = await _repository.GetChildrenByParentIdPagedAsync(parentId, paginationParams);
            
            return new PagedList<Dm_ThuocTinhTreeNodeDto>(
                _mapper.Map<List<Dm_ThuocTinhTreeNodeDto>>(entities),
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );
        }

        // Lấy đường dẫn đầy đủ tới thuộc tính
        public async Task<List<Dm_ThuocTinhTreeNodeDto>> GetFullPathWithChildrenAsync(Guid targetNodeId, Guid? newItemId = null)
        {
            if (!await _repository.ExistsAsync(targetNodeId))
                throw new KeyNotFoundException($"Thuộc tính có ID {targetNodeId} không tồn tại");

            var pathIds = await _repository.GetPathToRootAsync(targetNodeId);
            if (!pathIds.Any())
                return new List<Dm_ThuocTinhTreeNodeDto>();

            var rootNodes = await _repository.GetRootNodesWithRequiredChildrenAsync(pathIds, newItemId);
            return _mapper.Map<List<Dm_ThuocTinhTreeNodeDto>>(rootNodes);
        }

        // Xác thực mã
        public async Task<CodeValidationResult> ValidateCodeAsync(string ma, Guid? parentId = null, Guid? exceptId = null)
        {
            if (string.IsNullOrWhiteSpace(ma))
                return new CodeValidationResult
                {
                    IsValid = false,
                    Code = ma,
                    ParentId = parentId,
                    ExceptId = exceptId,
                    Message = "Mã không được để trống"
                };

            var exists = await _repository.ExistsByMaInSameLevelAsync(ma, parentId, exceptId);
            
            return new CodeValidationResult
            {
                IsValid = !exists,
                Code = ma,
                ParentId = parentId,
                ExceptId = exceptId,
                Message = exists ? "Mã đã tồn tại trong cùng nhóm thuộc tính" : "Mã hợp lệ, có thể sử dụng"
            };
        }

        // Xác thực nhiều mã
        public async Task<List<CodeValidationResult>> ValidateMultipleCodesAsync(List<string> codes, Guid? parentId = null)
        {
            if (codes == null || !codes.Any())
                return new List<CodeValidationResult>();

            var results = new List<CodeValidationResult>();
            
            // Xử lý mã rỗng
            foreach (var code in codes.Where(c => string.IsNullOrWhiteSpace(c)))
                results.Add(new CodeValidationResult
                {
                    IsValid = false,
                    Code = code,
                    ParentId = parentId,
                    Message = "Mã không được để trống"
                });
            
            // Xử lý mã không rỗng
            var validCodes = codes.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
            if (validCodes.Any())
            {
                var existingCodes = await _repository.GetExistingCodesInSameLevelAsync(validCodes, parentId);
                var existingCodesSet = new HashSet<string>(existingCodes, StringComparer.OrdinalIgnoreCase);
                
                foreach (var code in validCodes)
                    results.Add(new CodeValidationResult
                    {
                        IsValid = !existingCodesSet.Contains(code),
                        Code = code,
                        ParentId = parentId,
                        Message = existingCodesSet.Contains(code) 
                            ? "Mã đã tồn tại trong cùng nhóm thuộc tính" 
                            : "Mã hợp lệ, có thể sử dụng"
                    });
            }
            
            return results;
        }
    }
}
