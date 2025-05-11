using Application.DTOs.DanhMuc.HangHoasDto;
using Application.Mappings;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;

namespace Application.ServiceImplement.DanhMuc
{
    public class HangHoaService : IHangHoaService
    {
        private readonly IHangHoaRepository _hangHoaRepository;
        private readonly IDonViTinhService _donViTinhService;
        private readonly IMapper _mapper;

        public HangHoaService(IHangHoaRepository hangHoaRepository, IDonViTinhService donViTinhService, IMapper mapper)
        {
            _hangHoaRepository = hangHoaRepository;
            _donViTinhService = donViTinhService;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, List<HangHoaDto> Data, List<string> Errors)> CreateManyAsync(List<HangHoaCreateDto> dtos)
        {
            var errors = new List<string>();
            var validEntities = new List<Dm_HangHoa>();

            foreach (var dto in dtos)
            {
                var validation = await ValidateCreateHangHoaAsync(dto);
                if (!validation.IsValid)
                {
                    errors.Add($"[{dto.MaMatHang}] - {validation.ErrorMessage}");
                    continue;
                }

                var entity = _mapper.Map<Dm_HangHoa>(dto);
                validEntities.Add(entity);
            }

            if (validEntities.Any())
            {
                var savedEntities = await _hangHoaRepository.AddRangeAsync(validEntities);
                var data = _mapper.Map<List<HangHoaDto>>(savedEntities);
                return (errors.Count == 0, data, errors);
            }

            return (false, new List<HangHoaDto>(), errors);
        }

        public async Task<int> CountAsync()
        {
            return await _hangHoaRepository.CountAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _hangHoaRepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _hangHoaRepository.ExistsAsync(id);
        }

        public Task<bool> ExistsByMaMatHangAsync(
            string maMatHang,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(maMatHang))
                throw new ArgumentException("Mã mặt hàng không được để trống", nameof(maMatHang));

            return _hangHoaRepository.ExistsByMaMatHangAsync(
                maMatHang,
                excludeId,
                cancellationToken
            );
        }
        public async Task<PagedList<HangHoaDto>> GetActiveHangHoaAsync(PaginationParams paginationParams)
        {
            var hangHoas = await _hangHoaRepository.GetActiveHangHoaAsync(paginationParams);
            return hangHoas.MapTo<Dm_HangHoa, HangHoaDto>(_mapper);
        }

        public async Task<PagedList<HangHoaDto>> GetAllAsync(PaginationParams paginationParams)
        {
            var entities = await _hangHoaRepository.GetAllAsync(paginationParams);

            return entities.MapTo<Dm_HangHoa, HangHoaDto>(_mapper);
        }

        public async Task<HangHoaDto> GetByIdAsync(Guid id)
        {
            var hangHoa = await _hangHoaRepository.GetByIdAsync(id);
            return _mapper.Map<HangHoaDto>(hangHoa);
        }

        public async Task<HangHoaDto> GetByMaMatHangAsync(string maMatHang)
        {
            var hangHoa = await _hangHoaRepository.GetByMaMatHangAsync(maMatHang);
            return _mapper.Map<HangHoaDto>(hangHoa);
        }

        public async Task<PagedList<HangHoaDto>> GetByNhomHangHoaAsync(Guid nhomHangHoaId, PaginationParams paginationParams)
        {
            var hangHoas = await _hangHoaRepository.GetByNhomHangHoaAsync(nhomHangHoaId, paginationParams);
            return hangHoas.MapTo<Dm_HangHoa, HangHoaDto>(_mapper);
        }

        public async Task<PagedList<HangHoaDto>> GetWithFilterAsync(SpecificationParams specParams)
        {
            var hangHoas = await _hangHoaRepository.GetWithFilterAsync(specParams);
            return hangHoas.MapTo<Dm_HangHoa, HangHoaDto>(_mapper);
        }

        public async Task<PagedList<HangHoaDto>> SearchAsync(SearchParams p)
        {
            var ebntities = await _hangHoaRepository.SearchQuery(p);
            return ebntities.MapTo<Dm_HangHoa, HangHoaDto>(_mapper);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateHangHoaAsync(
         HangHoaDto hangHoaDto,
         bool isUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(hangHoaDto.MaMatHang))
                return (false, "Mã mặt hàng không được để trống");

            if (string.IsNullOrWhiteSpace(hangHoaDto.TenMatHang))
                return (false, "Tên mặt hàng không được để trống");

            if (!isUpdate)
            {
                // <-- đây sửa: không truyền hangHoaDto.Id
                var exists = await ExistsByMaMatHangAsync(hangHoaDto.MaMatHang, excludeId: null);
                if (exists)
                    return (false, $"Mã mặt hàng {hangHoaDto.MaMatHang} đã tồn tại");
            }

            if (hangHoaDto.NgayHieuLuc > hangHoaDto.NgayHetHieuLuc)
                return (false, "NgayHieuLuc cannot be after NgayHetHieuLuc");

            return (true, string.Empty);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateCreateHangHoaAsync(HangHoaCreateDto createDto)
        {
            // Map createDto to HangHoaDto for common validation
            var hangHoaDto = _mapper.Map<HangHoaDto>(createDto);

            // Use the existing validation method with isUpdate=false
            return await ValidateHangHoaAsync(hangHoaDto, false);
        }


        public async Task<HangHoaDto> AddAsync(HangHoaCreateDto createDto)
        {
            var hangHoa = _mapper.Map<Dm_HangHoa>(createDto);
            var result = await _hangHoaRepository.AddAsync(hangHoa);
            return _mapper.Map<HangHoaDto>(result);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateUpdateHangHoaAsync(HangHoaUpdateDto updateDto)
        {
            var hangHoaDto = _mapper.Map<HangHoaDto>(updateDto);
            return await ValidateHangHoaAsync(hangHoaDto, true);
        }


        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateAsync(HangHoaUpdateDto updateDto)
        {
            var (isValid, errorMessage) = await ValidateUpdateHangHoaAsync(updateDto);

            if (!isValid)
            {
                return (false, errorMessage);
            }

            var hangHoa = _mapper.Map<Dm_HangHoa>(updateDto);
            var success = await _hangHoaRepository.UpdateAsync(hangHoa);

            if (!success)
            {
                return (false, string.Empty);
            }

            return (true, string.Empty);
        }

        // Thêm phương thức implementation

        public async Task<(bool IsSuccess, List<HangHoaDto> ImportedItems, List<string> Errors)> ImportFromExcelAsync(
            List<HangHoaImportDto> importDtos)
        {
            var errors = new List<string>();
            var createdEntities = new List<Dm_HangHoa>();
            var importedItems = new List<HangHoaDto>();
            
            // Lấy tất cả tên đơn vị tính từ danh sách import để kiểm tra/tạo một lần
            var donViTinhNames = importDtos.Select(x => x.DonViTinhTen.Trim())
                                         .Distinct()
                                         .Where(x => !string.IsNullOrWhiteSpace(x))
                                         .ToList();
                                         
            // Dictionary để lưu trữ ánh xạ từ tên đơn vị tính đến ID
            var donViTinhMapping = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            
            foreach (var tenDonViTinh in donViTinhNames)
            {
                try
                {
                    // Tìm hoặc tạo đơn vị tính
                    var donViTinh = await _donViTinhService.AddIfNotExistsAsync(tenDonViTinh);
                    if (donViTinh != null)
                    {
                        donViTinhMapping[tenDonViTinh] = donViTinh.Id;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Lỗi xử lý đơn vị tính '{tenDonViTinh}': {ex.Message}");
                }
            }
            
            // Xử lý từng hàng hóa
            foreach (var importDto in importDtos)
            {
                try
                {
                    // Validate dữ liệu cơ bản
                    if (string.IsNullOrWhiteSpace(importDto.MaMatHang))
                    {
                        errors.Add($"Hàng hóa '{importDto.TenMatHang}': Mã mặt hàng không được để trống");
                        continue;
                    }
                    
                    if (string.IsNullOrWhiteSpace(importDto.TenMatHang))
                    {
                        errors.Add($"Hàng hóa có mã '{importDto.MaMatHang}': Tên mặt hàng không được để trống");
                        continue;
                    }
                    
                    if (string.IsNullOrWhiteSpace(importDto.DonViTinhTen))
                    {
                        errors.Add($"Hàng hóa '{importDto.TenMatHang}': Đơn vị tính không được để trống");
                        continue;
                    }
                    
                    // Kiểm tra mã hàng hóa đã tồn tại chưa
                    if (await _hangHoaRepository.ExistsByMaMatHangAsync(importDto.MaMatHang))
                    {
                        errors.Add($"Hàng hóa có mã '{importDto.MaMatHang}' đã tồn tại trong hệ thống");
                        continue;
                    }
                    
                    // Kiểm tra đơn vị tính đã được xử lý chưa
                    if (!donViTinhMapping.TryGetValue(importDto.DonViTinhTen.Trim(), out var donViTinhId))
                    {
                        errors.Add($"Hàng hóa '{importDto.TenMatHang}': Không thể xác định đơn vị tính '{importDto.DonViTinhTen}'");
                        continue;
                    }
                    
                    var hangHoa = _mapper.Map<Dm_HangHoa>(importDto);
                    
                    // Gán DonViTinhId sau khi mapping
                    hangHoa.DonViTinhId = donViTinhId;
                    
                    // Xử lý nhóm hàng hóa nếu cần
                    if (!string.IsNullOrWhiteSpace(importDto.NhomHangHoaMa))
                    {
                        // Có thể thêm logic tìm NhomHangHoa theo mã ở đây
                        // hangHoa.NhomHangHoaId = await FindNhomHangHoaByMaAsync(importDto.NhomHangHoaMa);
                    }
                    
                    createdEntities.Add(hangHoa);
                }
                catch (Exception ex)
                {
                    errors.Add($"Lỗi xử lý hàng hóa '{importDto.TenMatHang}': {ex.Message}");
                }
            }
            
            // Lưu tất cả hàng hóa hợp lệ vào database
            if (createdEntities.Any())
            {
                try
                {
                    var savedEntities = await _hangHoaRepository.AddRangeAsync(createdEntities);
                    importedItems = _mapper.Map<List<HangHoaDto>>(savedEntities);
                }
                catch (Exception ex)
                {
                    errors.Add($"Lỗi khi lưu dữ liệu: {ex.Message}");
                }
            }
            
            return (errors.Count == 0, importedItems, errors);
        }
    }
}
