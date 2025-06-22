using Application.DTOs.NghiepVu.helpers;
using Application.DTOs.NghiepVu.ThuThapGiaThiTruong;
using Application.ServiceInterface.INghiepVu;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.NghiepVu;
using Core.Helpers;
using Core.Interfaces.IRepository.INghiepVu;

namespace Application.ServiceImplement.NghiepVu
{
    public class ThuThapGiaThiTruongService : IThuThapGiaThiTruongService
    {
        private readonly IThuThapGiaThiTruongRepository _repository;
        private readonly IMapper _mapper;

        public ThuThapGiaThiTruongService(
            IThuThapGiaThiTruongRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ThuThapGiaThiTruongDto> CreateAsync(ThuThapGiaThiTruongCreateDto createDto)
        {
            var entity = _mapper.Map<ThuThapGiaThiTruong>(createDto);

            var result = await _repository.AddAsync(entity);
            
            return _mapper.Map<ThuThapGiaThiTruongDto>(result);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<PagedList<ThuThapGiaThiTruongDto>> GetAllAsync(PaginationParams paginationParams)
        {
            var entities = await _repository.GetAllAsync(paginationParams);
            
            var dtos = _mapper.Map<List<ThuThapGiaThiTruongDto>>(entities);
            
            return new PagedList<ThuThapGiaThiTruongDto>(
                dtos,
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );
        }

        public async Task<ThuThapGiaThiTruongDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            
            return _mapper.Map<ThuThapGiaThiTruongDto>(entity);
        }

        public async Task<PagedList<ThuThapGiaThiTruongDto>> SearchAsync(SearchParams searchParams)
        {
            var entities = await _repository.SearchAsync(searchParams);
            
            var dtos = _mapper.Map<List<ThuThapGiaThiTruongDto>>(entities);
            
            return new PagedList<ThuThapGiaThiTruongDto>(
                dtos,
                entities.TotalCount,
                entities.CurrentPage,
                entities.PageSize
            );
        }

        public async Task<bool> UpdateAsync(ThuThapGiaThiTruongUpdateDto updateDto)
        {
            var entity = await _repository.GetByIdAsync(updateDto.Id);
            if (entity == null) return false;
            
            _mapper.Map(updateDto, entity);
            
            return await _repository.UpdateAsync(entity);
        }

        public async Task<List<HangHoaGiaThiTruongDto>> GetHierarchicalDataWithPreviousPricesAsync(
          Guid parentId,
          DateTime ngayThuThap,
          Guid loaiGiaId)
        {
            var hierarchicalData = await _repository.GetHierarchicalDataWithPreviousPricesAsync(
                parentId, ngayThuThap, loaiGiaId);

            var result = _mapper.Map<List<HangHoaGiaThiTruongDto>>(hierarchicalData);
            
            // Lấy giá kỳ trước và set vào DTO
            var hangHoaIds = GetAllHangHoaIds(result);
            var previousPrices = await _repository.GetPreviousPricesAsync(hangHoaIds, ngayThuThap, loaiGiaId);
            
            SetPreviousPrices(result, previousPrices);
            
            return result;
        }

        private List<Guid> GetAllHangHoaIds(List<HangHoaGiaThiTruongDto> items)
        {
            var ids = new List<Guid>();
            foreach (var item in items)
            {
                ids.Add(item.Id);
                if (item.MatHangCon?.Any() == true)
                {
                    ids.AddRange(GetAllHangHoaIds(item.MatHangCon));
                }
            }
            return ids;
        }

        private void SetPreviousPrices(List<HangHoaGiaThiTruongDto> items, Dictionary<Guid, decimal?> previousPrices)
        {
            foreach (var item in items)
            {
                if (previousPrices.TryGetValue(item.Id, out var price))
                {
                    item.GiaBinhQuanKyTruoc = price;
                }

                if (item.MatHangCon?.Any() == true)
                {
                    SetPreviousPrices(item.MatHangCon, previousPrices);
                }
            }
        }

        public async Task<ThuThapGiaThiTruongBulkCreateResponseDto> BulkCreateAsync(ThuThapGiaThiTruongBulkCreateDto bulkCreateDto)
        {
            var response = new ThuThapGiaThiTruongBulkCreateResponseDto();

            try
            {
                // Lấy danh sách HangHoaId từ input
                var hangHoaIds = bulkCreateDto.DanhSachGiaHangHoa.Select(x => x.HangHoaId).ToList();

                // Kiểm tra sự tồn tại của các bản ghi
                var existenceCheck = await _repository.CheckExistenceForBulkAsync(
                    hangHoaIds, bulkCreateDto.NgayThuThap, bulkCreateDto.LoaiGiaId);

                // Lấy giá kỳ trước cho tính toán
                var previousPrices = await _repository.GetPreviousPricesAsync(
                    hangHoaIds, bulkCreateDto.NgayThuThap, bulkCreateDto.LoaiGiaId);

                var entitiesToCreate = new List<ThuThapGiaThiTruong>();

                foreach (var giaHangHoa in bulkCreateDto.DanhSachGiaHangHoa)
                {
                    // Bỏ qua nếu đã tồn tại
                    if (existenceCheck.TryGetValue(giaHangHoa.HangHoaId, out bool exists) && exists)
                    {
                        response.TotalSkipped++;
                        response.Warnings.Add($"Hàng hóa ID {giaHangHoa.HangHoaId} đã có dữ liệu cho ngày {bulkCreateDto.NgayThuThap:dd/MM/yyyy}");
                        continue;
                    }

                    // Bỏ qua nếu không có giá nào được nhập
                    if (!giaHangHoa.GiaPhoBienKyBaoCao.HasValue && !giaHangHoa.GiaBinhQuanKyNay.HasValue)
                    {
                        response.TotalSkipped++;
                        response.Warnings.Add($"Hàng hóa ID {giaHangHoa.HangHoaId} không có giá được nhập");
                        continue;
                    }

                    // Tính toán giá kỳ trước và các chỉ số
                    var giaBinhQuanKyTruoc = previousPrices.TryGetValue(giaHangHoa.HangHoaId, out var prevPrice) ? prevPrice : null;

                    decimal? mucTangGiam = null;
                    decimal? tyLeTangGiam = null;

                    if (giaHangHoa.GiaBinhQuanKyNay.HasValue && giaBinhQuanKyTruoc.HasValue && giaBinhQuanKyTruoc > 0)
                    {
                        mucTangGiam = giaHangHoa.GiaBinhQuanKyNay.Value - giaBinhQuanKyTruoc.Value;
                        tyLeTangGiam = (mucTangGiam / giaBinhQuanKyTruoc.Value) * 100;
                    }

                    var entity = new ThuThapGiaThiTruong
                    {
                        Id = Guid.NewGuid(),
                        NgayThuThap = bulkCreateDto.NgayThuThap,
                        HangHoaId = giaHangHoa.HangHoaId,
                        LoaiGiaId = bulkCreateDto.LoaiGiaId,
                        GiaPhoBienKyBaoCao = giaHangHoa.GiaPhoBienKyBaoCao,
                        GiaBinhQuanKyNay = giaHangHoa.GiaBinhQuanKyNay,
                        NguonThongTin = bulkCreateDto.NguonThongTin,
                        GhiChu = giaHangHoa.GhiChu,
                        GiaBinhQuanKyTruoc = giaBinhQuanKyTruoc,
                        MucTangGiam = mucTangGiam,
                        TyLeTangGiam = tyLeTangGiam,
                        CreatedDate = DateTime.UtcNow,
                        IsDelete = false
                    };

                    entitiesToCreate.Add(entity);
                }

                // Thực hiện bulk insert
                if (entitiesToCreate.Any())
                {
                    var createdEntities = await _repository.BulkAddAsync(entitiesToCreate);
                    response.TotalCreated = createdEntities.Count;

                    // Map sang DTO để trả về
                    response.CreatedItems = _mapper.Map<List<ThuThapGiaThiTruongDto>>(createdEntities);
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Lỗi khi thêm mới: {ex.Message}");
            }

            return response;
        }

    }
}
