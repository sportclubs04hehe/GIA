using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.DanhMuc.Enum;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Infrastructure.Data.DanhMuc.Repository;

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
            await ValidateCreateDtoAsync(createDto);
            
            var entity = _mapper.Map<Dm_HangHoaThiTruong>(createDto);

            if (entity.LoaiMatHang == Loai.Cha)
            {
                entity.DonViTinhId = null;
                entity.DacTinh = null;
            }
            else if (entity.LoaiMatHang == Loai.Con && !entity.DonViTinhId.HasValue)
            {
                throw new ArgumentException("Mặt hàng thuộc loại hàng hóa phải có đơn vị tính");
            }

            var result = await _repository.AddAsync(entity);
            return _mapper.Map<HHThiTruongDto>(result);
        }

        public async Task<HHThiTruongDto> UpdateAsync(UpdateHHThiTruongDto updateDto)
        {
            var existingEntity = await _repository.GetByIdWithRelationsAsync(updateDto.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"Mặt hàng có ID {updateDto.Id} không tồn tại");

            await ValidateUpdateDtoAsync(updateDto);
            
            var entity = _mapper.Map<Dm_HangHoaThiTruong>(updateDto);
            if (entity.LoaiMatHang == Loai.Cha)
            {
                entity.DonViTinhId = null;
                entity.DacTinh = null;
            }
            else if (entity.LoaiMatHang == Loai.Con && !entity.DonViTinhId.HasValue)
            {
                throw new ArgumentException("Mặt hàng thuộc loại hàng hóa phải có đơn vị tính");
            }

            await _repository.UpdateAsync(entity);

            var updatedEntity = await _repository.GetByIdWithRelationsAsync(entity.Id);
            return _mapper.Map<HHThiTruongDto>(updatedEntity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteWithChildrenAsync(id);
        }

        /// <summary>
        /// Xóa nhiều mặt hàng cùng lúc
        /// </summary>
        public async Task<bool> DeleteMultipleAsync(List<Guid> ids)
        {
            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
                bool success = true;
                foreach (var id in ids)
                {
                    var result = await ((HHThiTruongRepository)_repository).DeleteWithChildrenAsync(id, useExternalTransaction: true);
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
            catch (Exception ex)
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
                dto.TenDonViTinh = null;
                dto.DonViTinhId = null;
                return dto;
            }).ToList();
        }

        public async Task<bool> IsValidCodeAsync(string ma, Guid? parentId, Guid? exceptId = null)
        {
            return !await _repository.ExistsByMaInSameLevelAsync(ma, parentId, exceptId);
        }

        public async Task<bool> ValidateCreateDtoAsync(CreateHHThiTruongDto createDto)
        {
            if (!await IsValidCodeAsync(createDto.Ma, createDto.MatHangChaId))
            {
                throw new ArgumentException($"Mã '{createDto.Ma}' đã tồn tại trong cùng nhóm hàng hóa. Vui lòng chọn mã khác.");
            }
            
            if (createDto.NgayHieuLuc > createDto.NgayHetHieuLuc)
            {
                throw new ArgumentException("Ngày hiệu lực không được lớn hơn ngày hết hiệu lực.");
            }
            
            return true;
        }

        public async Task<bool> ValidateUpdateDtoAsync(UpdateHHThiTruongDto updateDto)
        {
            if (!await IsValidCodeAsync(updateDto.Ma, updateDto.MatHangChaId, updateDto.Id))
            {
                throw new ArgumentException($"Mã '{updateDto.Ma}' đã tồn tại trong cùng nhóm hàng hóa. Vui lòng chọn mã khác.");
            }

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
                
                // Xử lý theo loại mặt hàng từ input
                if (dto.LoaiMatHang == Loai.Cha)
                {
                    entity.DonViTinhId = null;
                    entity.DacTinh = null;
                }
                else if (dto.LoaiMatHang == Loai.Con && !dto.DonViTinhId.HasValue)
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

        public async Task<PagedList<HHThiTruongTreeNodeDto>> SearchHierarchicalAsync(string searchTerm, PaginationParams paginationParams)
        {
            // Tìm tất cả items khớp với searchTerm có phân trang
            var matchingItems = await _repository.SearchAllPagedAsync(
                searchTerm,
                paginationParams,
                x => x.Ma,
                x => x.Ten
            );

            if (matchingItems.Count == 0)
            {
                return new PagedList<HHThiTruongTreeNodeDto>(
                    new List<HHThiTruongTreeNodeDto>(),
                    0,
                    paginationParams.PageIndex,
                    paginationParams.PageSize
                );
            }

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

            // Lấy cấu trúc cây với các node tìm thấy
            var rootItems = await _repository.GetRootItemsForSearchAsync(parentIds, itemIds);
            var treeNodes = _mapper.Map<List<HHThiTruongTreeNodeDto>>(rootItems);

            // Trả về PagedList từ kết quả đã có
            return new PagedList<HHThiTruongTreeNodeDto>(
                treeNodes,
                matchingItems.TotalCount,
                matchingItems.CurrentPage,
                matchingItems.PageSize
            );
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

        public async Task<HHThiTruongDto> GetByIdAsync(Guid id) =>
            _mapper.Map<HHThiTruongDto>(await _repository.GetByIdWithRelationsAsync(id));

        /// <summary>
        /// Lấy đường dẫn đầy đủ từ gốc đến node bao gồm các node con và các node anh em
        /// </summary>
        public async Task<List<HHThiTruongTreeNodeDto>> GetFullPathWithChildrenAsync(Guid targetNodeId, Guid? newItemId = null)
        {
            // Kiểm tra sự tồn tại của node đích
            var targetNode = await _repository.GetByIdNoTrackingAsync(targetNodeId);
            if (targetNode == null)
                throw new KeyNotFoundException($"Mặt hàng có ID {targetNodeId} không tồn tại");

            // Lấy đường dẫn dạng Guid từ gốc đến node đích
            var pathIds = await _repository.GetPathToRootAsync(targetNodeId);

            if (pathIds.Count == 0)
                return new List<HHThiTruongTreeNodeDto>();

            // Chuẩn bị kết quả với node gốc và tất cả các node con cần thiết
            var rootNodes = await _repository.GetRootNodesWithRequiredChildrenAsync(pathIds, newItemId);

            // Ánh xạ kết quả sang DTO
            return _mapper.Map<List<HHThiTruongTreeNodeDto>>(rootNodes);
        }

        public async Task<(bool IsSuccess, List<HHThiTruongDto> ImportedItems, List<string> Errors)> ImportFromExcelAsync(
            HHThiTruongBatchImportDto importDto)
        {
            var errors = new List<string>();
            var createdEntities = new List<Dm_HangHoaThiTruong>();
            var importedItems = new List<HHThiTruongDto>();
            
            // Kiểm tra tính hợp lệ của nhóm cha
            if (importDto.MatHangChaId.HasValue)
            {
                var parentExists = await _repository.ExistsAsync(importDto.MatHangChaId.Value);
                if (!parentExists)
                {
                    errors.Add($"Nhóm cha với ID {importDto.MatHangChaId.Value} không tồn tại");
                    return (false, importedItems, errors);
                }
            }
            
            // Lấy tất cả tên đơn vị tính từ danh sách import để kiểm tra/tạo một lần
            var donViTinhNames = importDto.Items
                .Where(x => x.LoaiMatHang == Loai.Con && !string.IsNullOrWhiteSpace(x.DonViTinhTen))
                .Select(x => x.DonViTinhTen.Trim())
                .Distinct()
                .ToList();
                                     
            // Dictionary để lưu trữ ánh xạ từ tên đơn vị tính đến ID
            var donViTinhMapping = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            
            foreach (var tenDonViTinh in donViTinhNames)
            {
                try
                {
                    // Tìm hoặc tạo đơn vị tính
                    var donViTinh = await _donViTinhRepository.GetByTenAsync(tenDonViTinh);
                    if (donViTinh != null)
                    {
                        donViTinhMapping[tenDonViTinh] = donViTinh.Id;
                    }
                    else
                    {
                        // Tạo đơn vị tính mới nếu chưa tồn tại
                        var newDonViTinh = new Dm_DonViTinh
                        {
                            Ma = tenDonViTinh.Replace(" ", "").ToUpper(),
                            Ten = tenDonViTinh,
                            NgayHieuLuc = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc),  
                            NgayHetHieuLuc = DateTime.SpecifyKind(DateTime.Today.AddYears(50), DateTimeKind.Utc), 
                            CreatedDate = DateTime.UtcNow, 
                            IsDelete = false
                        };
                        var createdDonViTinh = await _donViTinhRepository.AddAsync(newDonViTinh);
                        donViTinhMapping[tenDonViTinh] = createdDonViTinh.Id;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Lỗi xử lý đơn vị tính '{tenDonViTinh}': {ex.Message}");
                }
            }
            
            // Lưu trữ mã mặt hàng đã xử lý để kiểm tra trùng lặp trong cùng batch
            var processedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            // Xử lý từng mặt hàng thị trường
            foreach (var importItem in importDto.Items)
            {
                try
                {
                    // Validate dữ liệu cơ bản
                    if (string.IsNullOrWhiteSpace(importItem.Ma))
                    {
                        errors.Add($"Mặt hàng '{importItem.Ten}': Mã không được để trống");
                        continue;
                    }
                    
                    if (string.IsNullOrWhiteSpace(importItem.Ten))
                    {
                        errors.Add($"Mặt hàng có mã '{importItem.Ma}': Tên không được để trống");
                        continue;
                    }
                    
                    // Kiểm tra trùng mã trong cùng batch
                    if (processedCodes.Contains(importItem.Ma))
                    {
                        errors.Add($"Mã '{importItem.Ma}' bị trùng lặp trong file import");
                        continue;
                    }
                    
                    // Kiểm tra mã đã tồn tại trong cùng cấp (cùng nhóm cha) chưa
                    if (await _repository.ExistsByMaInSameLevelAsync(importItem.Ma, importDto.MatHangChaId))
                    {
                        errors.Add($"Mã '{importItem.Ma}' đã tồn tại trong cùng nhóm hàng hóa");
                        continue;
                    }
                    
                    // Ghi nhận mã đã xử lý
                    processedCodes.Add(importItem.Ma);
                    
                    // Xác thực ngày hiệu lực/hết hiệu lực
                    if (importItem.NgayHieuLuc > importItem.NgayHetHieuLuc)
                    {
                        errors.Add($"Mặt hàng '{importItem.Ma}': Ngày hiệu lực không được lớn hơn ngày hết hiệu lực");
                        continue;
                    }
                    
                    // Validate loại mặt hàng và đơn vị tính
                    if (importItem.LoaiMatHang == Loai.Con)
                    {
                        // Nếu là hàng hóa, phải có đơn vị tính
                        if (string.IsNullOrWhiteSpace(importItem.DonViTinhTen))
                        {
                            errors.Add($"Mặt hàng '{importItem.Ma}': Loại là hàng hóa nhưng không có đơn vị tính");
                            continue;
                        }
                        
                        // Kiểm tra đơn vị tính đã được xử lý chưa
                        if (!donViTinhMapping.TryGetValue(importItem.DonViTinhTen.Trim(), out var donViTinhId))
                        {
                            errors.Add($"Mặt hàng '{importItem.Ma}': Không thể xác định đơn vị tính '{importItem.DonViTinhTen}'");
                            continue;
                        }
                    }
                    
                    var entity = _mapper.Map<Dm_HangHoaThiTruong>(importItem);
                    
                    // Gán nhóm cha
                    entity.MatHangChaId = importDto.MatHangChaId;
                    
                    // Xử lý theo loại mặt hàng
                    if (entity.LoaiMatHang == Loai.Cha)
                    {
                        // Nếu là nhóm, đảm bảo không có đơn vị tính và đặc tính
                        entity.DonViTinhId = null;
                        entity.DacTinh = null;
                    }
                    else if (entity.LoaiMatHang == Loai.Con)
                    {
                        // Nếu là hàng hóa, gán đơn vị tính
                        entity.DonViTinhId = donViTinhMapping[importItem.DonViTinhTen.Trim()];
                    }
                    
                    createdEntities.Add(entity);
                }
                catch (Exception ex)
                {
                    errors.Add($"Lỗi xử lý mặt hàng '{importItem.Ten}': {ex.Message}");
                }
            }
            
            // Lưu tất cả mặt hàng hợp lệ vào database
            if (createdEntities.Any())
            {
                try
                {
                    // Sử dụng phương thức có sẵn để thêm các mặt hàng với validation
                    var savedEntities = await _repository.AddRangeWithValidationAsync(createdEntities);
                    importedItems = _mapper.Map<List<HHThiTruongDto>>(savedEntities);
                }
                catch (Exception ex)
                {
                    errors.Add($"Lỗi khi lưu dữ liệu: {ex.Message}");
                }
            }
            
            return (errors.Count == 0 || importedItems.Any(), importedItems, errors);
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của mã mặt hàng trong cùng một nhóm
        /// </summary>
        /// <param name="ma">Mã cần kiểm tra</param>
        /// <param name="parentId">ID nhóm cha (null nếu là cấp cao nhất)</param>
        /// <param name="exceptId">ID của mặt hàng cần loại trừ khỏi việc kiểm tra (khi cập nhật)</param>
        /// <returns>Kết quả kiểm tra chi tiết</returns>
        public async Task<CodeValidationResult> ValidateCodeAsync(string ma, Guid? parentId = null, Guid? exceptId = null)
        {
            if (string.IsNullOrWhiteSpace(ma))
            {
                return new CodeValidationResult
                {
                    IsValid = false,
                    Code = ma,
                    ParentId = parentId,
                    ExceptId = exceptId,
                    Message = "Mã không được để trống"
                };
            }

            var exists = await _repository.ExistsByMaInSameLevelAsync(ma, parentId, exceptId);
            
            return new CodeValidationResult
            {
                IsValid = !exists,
                Code = ma,
                ParentId = parentId,
                ExceptId = exceptId,
                Message = !exists 
                    ? "Mã hợp lệ, có thể sử dụng" 
                    : "Mã đã tồn tại trong cùng nhóm hàng hóa"
            };
        }
        public async Task<List<CodeValidationResult>> ValidateMultipleCodesAsync(List<string> codes, Guid? parentId = null)
        {
            if (codes == null || !codes.Any())
            {
                return new List<CodeValidationResult>();
            }

            var results = new List<CodeValidationResult>();

            // Xử lý các mã rỗng trước
            foreach (var code in codes.Where(c => string.IsNullOrWhiteSpace(c)))
            {
                results.Add(new CodeValidationResult
                {
                    IsValid = false,
                    Code = code,
                    ParentId = parentId,
                    Message = "Mã không được để trống"
                });
            }

            // Lấy các mã không rỗng để kiểm tra
            var nonEmptyCodes = codes.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            if (nonEmptyCodes.Any())
            {
                // Sử dụng repository để kiểm tra mã tồn tại
                var existingCodes = await _repository.GetExistingCodesInSameLevelAsync(nonEmptyCodes, parentId);

                // Tạo set mã đã tồn tại để tìm kiếm nhanh
                var existingCodesSet = new HashSet<string>(existingCodes, StringComparer.OrdinalIgnoreCase);

                // Kiểm tra từng mã không rỗng
                foreach (var code in nonEmptyCodes)
                {
                    bool exists = existingCodesSet.Contains(code);
                    results.Add(new CodeValidationResult
                    {
                        IsValid = !exists,
                        Code = code,
                        ParentId = parentId,
                        Message = !exists
                            ? "Mã hợp lệ, có thể sử dụng"
                            : "Mã đã tồn tại trong cùng nhóm hàng hóa"
                    });
                }
            }

            return results;
        }


    }
}
