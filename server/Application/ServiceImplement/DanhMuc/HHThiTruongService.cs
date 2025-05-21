using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;

namespace Application.ServiceImplement.DanhMuc
{
    public class HHThiTruongService : IHHThiTruongService
    {
        private readonly IHHThiTruongRepository _repository;
        private readonly IDonViTinhRepository _donViTinhRepository;
        private readonly IMapper _mapper;

        public HHThiTruongService(IHHThiTruongRepository repository, IDonViTinhRepository donViTinhRepository, IMapper mapper)
        {
            _repository = repository;
            _donViTinhRepository = donViTinhRepository;
            _mapper = mapper;
        }

        public async Task<HHThiTruongDto> CreateAsync(CreateHHThiTruongDto createDto)
        {
            // Validate the DTO first
            await ValidateCreateDtoAsync(createDto);
            
            var entity = _mapper.Map<Dm_HangHoaThiTruong>(createDto);

            // Set LoaiMatHang based on whether it has DonViTinhId (is a product or category)
            entity.LoaiMatHang = createDto.DonViTinhId.HasValue
                ? LoaiMatHangEnum.HangHoa
                : LoaiMatHangEnum.Nhom;

            // If it's a category (not a product), clear product-specific fields
            if (entity.LoaiMatHang == LoaiMatHangEnum.Nhom)
            {
                entity.DonViTinhId = null;
                entity.DacTinh = null;
            }

            var result = await _repository.AddAsync(entity);
            return _mapper.Map<HHThiTruongDto>(result);
        }

        public async Task<HHThiTruongDto> UpdateAsync(UpdateHHThiTruongDto updateDto)
        {
            var existingEntity = await _repository.GetByIdAsync(updateDto.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"Mặt hàng có ID {updateDto.Id} không tồn tại");

            // Validate the DTO first
            await ValidateUpdateDtoAsync(updateDto);
            
            var entity = _mapper.Map<Dm_HangHoaThiTruong>(updateDto);

            // Set LoaiMatHang based on whether it has DonViTinhId
            entity.LoaiMatHang = updateDto.DonViTinhId.HasValue
                ? LoaiMatHangEnum.HangHoa
                : LoaiMatHangEnum.Nhom;

            // If changing from product to category, clear product-specific fields
            if (entity.LoaiMatHang == LoaiMatHangEnum.Nhom)
            {
                entity.DonViTinhId = null;
                entity.DacTinh = null;
            }

            await _repository.UpdateAsync(entity);

            // Get updated entity with relationships
            var updatedEntity = await _repository.GetByIdNoTrackingAsync(entity.Id);
            return _mapper.Map<HHThiTruongDto>(updatedEntity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteWithChildrenAsync(id);
        }

        public async Task<bool> DeleteMultipleAsync(List<Guid> ids)
        {
            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
                bool success = true;
                foreach (var id in ids)
                {
                    var result = await _repository.DeleteWithChildrenAsync(id);
                    if (!result)
                    {
                        success = false;
                    }
                }

                if (success)
                {
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedList<HHThiTruongDto>> GetAllAsync(PaginationParams paginationParams)
        {
            var entities = await _repository.GetAllAsync(paginationParams);
            return new PagedList<HHThiTruongDto>(
                _mapper.Map<List<HHThiTruongDto>>(entities.ToList()),
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );
        }

        public async Task<List<HHThiTruongDto>> GetAllParentCategoriesAsync()
        {
            var entities = await _repository.GetAllParentCategoriesAsync();
            return _mapper.Map<List<HHThiTruongDto>>(entities);
        }

        public async Task<List<CategoryInfoDto>> GetAllCategoriesWithChildInfoAsync()
        {
            var categoriesWithChildInfo = await _repository.GetAllCategoriesWithChildInfoAsync();

            return categoriesWithChildInfo.Select(tuple => {
                var dto = _mapper.Map<CategoryInfoDto>(tuple.Category);
                dto.HasChildren = tuple.HasChildren;
                // Đảm bảo các trường liên quan đến đơn vị tính luôn là null
                dto.TenDonViTinh = null;
                dto.DonViTinhId = null;
                return dto;
            }).ToList();
        }

        public async Task<HHThiTruongTreeNodeDto> GetWithChildrenAsync(Guid id)
        {
            var entity = await _repository.GetWithChildrenAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Mặt hàng có ID {id} không tồn tại");

            return _mapper.Map<HHThiTruongTreeNodeDto>(entity);
        }

        public async Task<bool> IsValidCodeAsync(string ma, Guid? parentId, Guid? exceptId = null)
        {
            return !await _repository.ExistsByMaInSameLevelAsync(ma, parentId, exceptId);
        }

        public async Task<bool> ValidateCreateDtoAsync(CreateHHThiTruongDto createDto)
        {
            // Check for duplicate code in the same level
            if (!await IsValidCodeAsync(createDto.Ma, createDto.MatHangChaId))
            {
                throw new ArgumentException($"Mã '{createDto.Ma}' đã tồn tại trong cùng nhóm hàng hóa. Vui lòng chọn mã khác.");
            }
            
            // Date validation - this could be handled by attributes but adding here for completeness
            if (createDto.NgayHieuLuc > createDto.NgayHetHieuLuc)
            {
                throw new ArgumentException("Ngày hiệu lực không được lớn hơn ngày hết hiệu lực.");
            }
            
            return true;
        }

        public async Task<bool> ValidateUpdateDtoAsync(UpdateHHThiTruongDto updateDto)
        {
            // Check for duplicate code in the same level (excluding the current item)
            if (!await IsValidCodeAsync(updateDto.Ma, updateDto.MatHangChaId, updateDto.Id))
            {
                throw new ArgumentException($"Mã '{updateDto.Ma}' đã tồn tại trong cùng nhóm hàng hóa. Vui lòng chọn mã khác.");
            }

            // Date validation - this could be handled by attributes but adding here for completeness
            if (updateDto.NgayHieuLuc > updateDto.NgayHetHieuLuc)
            {
                throw new ArgumentException("Ngày hiệu lực không được lớn hơn ngày hết hiệu lực.");
            }
            
            return true;
        }

        // Add this method to the existing service class
        public async Task<List<HHThiTruongDto>> CreateManyAsync(CreateManyHHThiTruongDto createDto)
        {
            // Validate all items
            foreach (var item in createDto.Items)
            {
                await ValidateCreateDtoAsync(item);
            }
            
            // Collect all DonViTinhIds to validate in a single query
            var donViTinhIds = createDto.Items
                .Where(x => x.DonViTinhId.HasValue)
                .Select(x => x.DonViTinhId!.Value)
                .Distinct()
                .ToList();
            
            if (donViTinhIds.Any())
            {
                // Sử dụng phương thức kế thừa từ GenericRepository
                var existingDonViTinhIds = await _donViTinhRepository.ExistsManyAsync(donViTinhIds);
                var missingIds = donViTinhIds.Except(existingDonViTinhIds).ToList();
                
                if (missingIds.Any())
                {
                    throw new ArgumentException($"Một hoặc nhiều đơn vị tính không tồn tại: {string.Join(", ", missingIds)}");
                }
            }
            
            // Prepare entities
            var entities = new List<Dm_HangHoaThiTruong>();
            
            foreach (var dto in createDto.Items)
            {
                var entity = _mapper.Map<Dm_HangHoaThiTruong>(dto);
                
                // If it's a category (LoaiMatHang = 0), ensure product-specific fields are cleared
                if (dto.LoaiMatHang == LoaiMatHangEnum.Nhom)
                {
                    entity.DonViTinhId = null;
                    entity.DacTinh = null;
                }
                // If it's a product (LoaiMatHang = 1), ensure required fields are present
                else if (dto.LoaiMatHang == LoaiMatHangEnum.HangHoa && !dto.DonViTinhId.HasValue)
                {
                    throw new ArgumentException($"Mặt hàng '{dto.Ten}' có loại là hàng hóa nhưng không có đơn vị tính");
                }
                
                entities.Add(entity);
            }
            
            // Add to database with validation
            var result = await _repository.AddRangeWithValidationAsync(entities);
            
            // Map back to DTOs
            return _mapper.Map<List<HHThiTruongDto>>(result);
        }

        public async Task<List<HHThiTruongTreeNodeDto>> SearchHierarchicalAsync(string searchTerm)
        {
            // Tìm tất cả items khớp với searchTerm
            var matchingItems = await _repository.SearchAllAsync(
                searchTerm,
                x => x.Ma,
                x => x.Ten
            );
            
            // Thu thập tất cả parent IDs cần thiết
            var itemIds = matchingItems.Select(x => x.Id).ToList();
            var parentIds = new HashSet<Guid>();
            
            foreach (var item in matchingItems.Where(x => x.MatHangChaId.HasValue))
            {
                var current = item;
                while (current.MatHangChaId.HasValue)
                {
                    if (parentIds.Contains(current.MatHangChaId.Value))
                        break;
                    
                    parentIds.Add(current.MatHangChaId.Value);
                    current = await _repository.GetByIdNoTrackingAsync(current.MatHangChaId.Value);
                    
                    if (current == null)
                        break;
                }
            }
            
            var rootItems = await _repository.GetRootItemsForSearchAsync(parentIds, itemIds);
            return _mapper.Map<List<HHThiTruongTreeNodeDto>>(rootItems);
        }

        public async Task<PagedList<HHThiTruongTreeNodeDto>> GetChildrenByParentIdPagedAsync(Guid parentId, PaginationParams paginationParams)
        {
            var entities = await _repository.GetChildrenByParentIdPagedAsync(parentId, paginationParams);
            
            return new PagedList<HHThiTruongTreeNodeDto>(
                _mapper.Map<List<HHThiTruongTreeNodeDto>>(entities.ToList()),
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );
        }
    }
}
