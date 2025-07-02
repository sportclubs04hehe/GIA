using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.DTOs.DanhMuc.Dm_LoaiGia;
using Application.DTOs.NghiepVu.ThuThapGiaChiTiet;
using Application.DTOs.NghiepVu.ThuThapGiaThiTruong;
using Application.ServiceInterface.INghiepVu;
using AutoMapper;
using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.NghiepVu;
using Core.Helpers;
using Core.Interfaces.IRepository.IDanhMuc;
using Core.Interfaces.IRepository.INghiepVu;
using System.Linq.Expressions;

namespace Application.ServiceImplement.NghiepVu
{
    public class ThuThapGiaThiTruongService : IThuThapGiaThiTruongService
    {
        private readonly IThuThapGiaThiTruongRepository _thuThapGiaRepository;
        private readonly IDm_LoaiGiaRepository _loaiGiaRepository;
        private readonly IMapper _mapper;

        public ThuThapGiaThiTruongService(
            IThuThapGiaThiTruongRepository thuThapGiaRepository,
            IDm_LoaiGiaRepository loaiGiaRepository,
            IMapper mapper)
        {
            _thuThapGiaRepository = thuThapGiaRepository;
            _loaiGiaRepository = loaiGiaRepository;
            _mapper = mapper;
        }

        // Lấy danh sách các loại giá (để hiển thị trong dropdown chọn)
        public async Task<IEnumerable<LoaiGiaDto>> GetLoaiGiaAsync()
        {
            var loaiGiaList = await _loaiGiaRepository.GetListAsync(lg => !lg.IsDelete);
            return _mapper.Map<IEnumerable<LoaiGiaDto>>(loaiGiaList);
        }

        // Lấy thông tin của một phiếu thu thập giá đã có
        public async Task<ThuThapGiaThiTruongDto> GetByIdAsync(Guid id)
        {
            var thuThapGia = await _thuThapGiaRepository.GetByIdAsync(id);
            return _mapper.Map<ThuThapGiaThiTruongDto>(thuThapGia);
        }

        // Lấy thông tin chi tiết của một phiếu thu thập giá bao gồm danh sách chi tiết giá
        public async Task<ThuThapGiaThiTruongDto> GetWithDetailsAsync(Guid id)
        {
            var thuThapGia = await _thuThapGiaRepository.GetThuThapGiaThiTruongWithDetailsAsync(id);
            var thuThapGiaDto = _mapper.Map<ThuThapGiaThiTruongDto>(thuThapGia);
            return thuThapGiaDto;
        }

        // Tạo mới phiếu thu thập giá và danh sách chi tiết giá
        public async Task<ThuThapGiaThiTruongDto> CreateThuThapGiaVaChiTietAsync(
            ThuThapGiaThiTruongCreateDto thuThapGiaDto,
            IEnumerable<ThuThapGiaChiTietCreateDto> chiTietGiaDto)
        {
            // Chuyển đổi từ DTO sang entity
            var thuThapGiaEntity = _mapper.Map<ThuThapGiaThiTruong>(thuThapGiaDto);
            var chiTietGiaEntities = _mapper.Map<IEnumerable<ThuThapGiaChiTiet>>(chiTietGiaDto);
            
            // Gọi repository để lưu dữ liệu
            var result = await _thuThapGiaRepository.CreateThuThapGiaVaChiTietAsync(
                thuThapGiaEntity, chiTietGiaEntities);
            
            // Trả về kết quả đã được chuyển đổi sang DTO
            return _mapper.Map<ThuThapGiaThiTruongDto>(result);
        }

        // Cập nhật phiếu thu thập giá và danh sách chi tiết giá
        public async Task<bool> UpdateThuThapGiaVaChiTietAsync(
            ThuThapGiaThiTruongUpdateDto thuThapGiaDto,
            IEnumerable<ThuThapGiaChiTietUpdateDto> chiTietGiaDto)
        {
            // Chuyển đổi từ DTO sang entity
            var thuThapGiaEntity = _mapper.Map<ThuThapGiaThiTruong>(thuThapGiaDto);
            var chiTietGiaEntities = _mapper.Map<IEnumerable<ThuThapGiaChiTiet>>(chiTietGiaDto);
            
            // Gọi repository để cập nhật dữ liệu
            return await _thuThapGiaRepository.UpdateThuThapGiaVaChiTietAsync(
                thuThapGiaEntity, chiTietGiaEntities);
        }

        // Xóa phiếu thu thập giá
        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _thuThapGiaRepository.DeleteAsync(id);
        }

        // Lấy danh sách phiếu thu thập giá có phân trang
        public async Task<PagedList<ThuThapGiaThiTruongDto>> GetAllAsync(PaginationParams paginationParams)
        {
            var pagedList = await _thuThapGiaRepository.GetAllAsync(paginationParams);
            return _mapper.Map<PagedList<ThuThapGiaThiTruongDto>>(pagedList);
        }

        // Tìm kiếm phiếu thu thập giá
        public async Task<PagedList<ThuThapGiaThiTruongDto>> SearchAsync(SearchParams searchParams)
        {
            Expression<Func<ThuThapGiaThiTruong, string>> exprTuan = t => t.Tuan.ToString();
            Expression<Func<ThuThapGiaThiTruong, string>> exprNam = t => t.Nam.ToString();
            
            var pagedList = await _thuThapGiaRepository.SearchAsync(
                searchParams, 
                exprTuan, 
                exprNam);
            
            return _mapper.Map<PagedList<ThuThapGiaThiTruongDto>>(pagedList);
        }
    }
}
