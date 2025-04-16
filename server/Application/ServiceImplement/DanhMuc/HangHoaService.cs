using Application.DTOs.DanhMuc.HangHoasDto;
using Application.Mappings;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using Core.Entities.Domain;
using Core.Helpers;
using Core.Interfaces.IRepository;

namespace Application.ServiceImplement.DanhMuc
{
    public class HangHoaService : IHangHoaService
    {
        private readonly IHangHoaRepository _hangHoaRepository;
        private readonly IMapper _mapper;

        public HangHoaService(IHangHoaRepository hangHoaRepository, IMapper mapper)
        {
            _hangHoaRepository = hangHoaRepository;
            _mapper = mapper;
        }

        public async Task<HangHoaDto> AddAsync(HangHoaDto hangHoaDto)
        {
            var hangHoa = _mapper.Map<HangHoa>(hangHoaDto);
            var result = await _hangHoaRepository.AddAsync(hangHoa);
            return _mapper.Map<HangHoaDto>(result);
        }

        public async Task<(bool IsSuccess, List<HangHoaDto> Data, List<string> Errors)> CreateManyAsync(List<HangHoaCreateDto> dtos)
        {
            var errors = new List<string>();
            var validEntities = new List<HangHoa>();

            foreach (var dto in dtos)
            {
                var validation = await ValidateCreateHangHoaAsync(dto);
                if (!validation.IsValid)
                {
                    errors.Add($"[{dto.MaMatHang}] - {validation.ErrorMessage}");
                    continue;
                }

                var entity = _mapper.Map<HangHoa>(dto);
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

        public async Task<bool> ExistsByMaMatHangAsync(string maMatHang)
        {
            return await _hangHoaRepository.ExistsByMaMatHangAsync(maMatHang);
        }

        public async Task<PagedList<HangHoaDto>> GetActiveHangHoaAsync(PaginationParams paginationParams)
        {
            var hangHoas = await _hangHoaRepository.GetActiveHangHoaAsync(paginationParams);
            return hangHoas.MapTo<HangHoa, HangHoaDto>(_mapper);
        }

        public async Task<PagedList<HangHoaDto>> GetAllAsync(PaginationParams paginationParams)
        {
            var hangHoas = await _hangHoaRepository.GetAllAsync(paginationParams);
            return hangHoas.MapTo<HangHoa, HangHoaDto>(_mapper);
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
            return hangHoas.MapTo<HangHoa, HangHoaDto>(_mapper);
        }

        public async Task<PagedList<HangHoaDto>> GetWithFilterAsync(SpecificationParams specParams)
        {
            var hangHoas = await _hangHoaRepository.GetWithFilterAsync(specParams);
            return hangHoas.MapTo<HangHoa, HangHoaDto>(_mapper);
        }

        public async Task<PagedList<HangHoaDto>> SearchAsync(SearchParams searchParams)
        {
            var hangHoas = await _hangHoaRepository.SearchAsync(searchParams);
            return hangHoas.MapTo<HangHoa, HangHoaDto>(_mapper);
        }

        public async Task<bool> UpdateAsync(HangHoaDto hangHoaDto)
        {
            var hangHoa = _mapper.Map<HangHoa>(hangHoaDto);
            return await _hangHoaRepository.UpdateAsync(hangHoa);
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateHangHoaAsync(HangHoaDto hangHoaDto, bool isUpdate = false)
        {
            // Check required fields
            if (string.IsNullOrWhiteSpace(hangHoaDto.MaMatHang))
                return (false, "MaMatHang cannot be blank");
                
            if (string.IsNullOrWhiteSpace(hangHoaDto.TenMatHang))
                return (false, "TenMatHang cannot be blank");

            // Check if MaMatHang already exists (for new records)
            if (!isUpdate)
            {
                var exists = await ExistsByMaMatHangAsync(hangHoaDto.MaMatHang);
                if (exists)
                    return (false, $"MaMatHang'{hangHoaDto.MaMatHang}' already exists");
            }
            else
            {
                // For updates, check if the code exists but belongs to a different record
                var existingHangHoa = await GetByMaMatHangAsync(hangHoaDto.MaMatHang);
                if (existingHangHoa != null && existingHangHoa.Id != hangHoaDto.Id)
                    return (false, $"MaMatHang'{hangHoaDto.MaMatHang}' has been used by another item");
            }

            // Check if dates are valid
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
        
        public async Task<(bool IsValid, string ErrorMessage)> ValidateUpdateHangHoaAsync(HangHoaUpdateDto updateDto)
        {
            // Map updateDto to HangHoaDto for common validation
            var hangHoaDto = _mapper.Map<HangHoaDto>(updateDto);
            
            // Use the existing validation method with isUpdate=true
            return await ValidateHangHoaAsync(hangHoaDto, true);
        }

        public async Task<HangHoaDto> AddAsync(HangHoaCreateDto createDto)
        {
            var hangHoa = _mapper.Map<HangHoa>(createDto);
            var result = await _hangHoaRepository.AddAsync(hangHoa);
            return _mapper.Map<HangHoaDto>(result);
        }

        public async Task<bool> UpdateAsync(HangHoaUpdateDto updateDto)
        {
            var hangHoa = _mapper.Map<HangHoa>(updateDto);
            return await _hangHoaRepository.UpdateAsync(hangHoa);
        }
    }
}
