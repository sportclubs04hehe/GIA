using Application.DTOs.DanhMuc.HangHoasDto;
using Application.Mappings;
using Application.ServiceInterface.IDanhMuc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Microsoft.EntityFrameworkCore;

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
            var query = _hangHoaRepository.SearchQuery(p);

            var dtoQuery = query
                .ProjectTo<HangHoaDto>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            return await PagedList<HangHoaDto>.CreateAsync(
                dtoQuery,
                p.PageIndex,
                p.PageSize
            );
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

    }
}
