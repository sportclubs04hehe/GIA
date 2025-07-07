using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.DTOs.DanhMuc.Dm_LoaiGia;
using Application.DTOs.NghiepVu.ThuThapGiaChiTiet;
using Application.DTOs.NghiepVu.ThuThapGiaThiTruong;
using Application.ServiceInterface.INghiepVu;
using AutoMapper;
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
        private readonly IGiaDichVuRepository _giaDichVuRepository;

        private readonly IMapper _mapper;

        public ThuThapGiaThiTruongService(
            IThuThapGiaThiTruongRepository thuThapGiaRepository,
            IDm_LoaiGiaRepository loaiGiaRepository,
            IGiaDichVuRepository giaDichVuRepository,
            IMapper mapper)
        {
            _thuThapGiaRepository = thuThapGiaRepository;
            _loaiGiaRepository = loaiGiaRepository;
            _giaDichVuRepository = giaDichVuRepository;
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



        public async Task<List<HHThiTruongTreeNodeDto>> GetAllChildrenRecursiveAsync(Guid parentId, DateTime? ngayNhap = null)
        {
            // Đảm bảo ngày nhập là UTC
            DateTime? utcNgayNhap = ngayNhap.HasValue
                ? DateTime.SpecifyKind(ngayNhap.Value, DateTimeKind.Utc)
                : null;

            // Sử dụng repository mới để lấy cả hàng hóa và giá kỳ trước trong một lần gọi
            var (hangHoa, giaKyTruoc) = await _giaDichVuRepository.GetHangHoaVaGiaKyTruocAsync(parentId, utcNgayNhap);

            if (!hangHoa.Any())
                return new List<HHThiTruongTreeNodeDto>();

            // Chuyển đổi danh sách phẳng thành cấu trúc cây
            Dictionary<Guid, HHThiTruongTreeNodeDto> nodeDict = new();
            List<HHThiTruongTreeNodeDto> rootNodes = new();

            // Ánh xạ tất cả các node vào từ điển để truy cập nhanh
            foreach (var item in hangHoa)
            {
                var dto = _mapper.Map<HHThiTruongTreeNodeDto>(item);

                // Cập nhật giá kỳ trước nếu có
                if (giaKyTruoc.ContainsKey(item.Id))
                {
                    dto.GiaBinhQuanKyTruoc = giaKyTruoc[item.Id];
                }

                nodeDict[item.Id] = dto;
            }

            // Xây dựng cấu trúc cây
            foreach (var item in hangHoa)
            {
                if (item.MatHangChaId == parentId)
                {
                    // Nếu parent là ID gốc được yêu cầu, đây là node gốc cấp 1
                    rootNodes.Add(nodeDict[item.Id]);
                }
                else if (item.MatHangChaId.HasValue && nodeDict.ContainsKey(item.MatHangChaId.Value))
                {
                    // Nếu không, thêm vào node cha tương ứng
                    nodeDict[item.MatHangChaId.Value].MatHangCon.Add(nodeDict[item.Id]);
                }
            }

            // Sắp xếp tất cả các cấp trong cây theo mã
            SortTreeNodesByCode(rootNodes);

            return rootNodes;
        }

        /// <summary>
        /// Sắp xếp cấu trúc cây theo mã một cách đệ quy
        /// </summary>
        private void SortTreeNodesByCode(List<HHThiTruongTreeNodeDto> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            // Luôn sắp xếp dù chỉ có 1 phần tử để đảm bảo thứ tự đúng khi thêm vào cây
            nodes.Sort((a, b) => CompareNumericCodes(a.Ma, b.Ma));

            // Sắp xếp đệ quy tất cả các con, kể cả khi chỉ có 1 con
            foreach (var node in nodes)
            {
                if (node.MatHangCon != null)
                {
                    SortTreeNodesByCode(node.MatHangCon);
                }
            }
        }


        /// <summary>
        /// So sánh hai mã số có thể có nhiều định dạng khác nhau
        /// </summary>
        private int CompareNumericCodes(string x, string y)
        {
            if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y)) return 0;
            if (string.IsNullOrEmpty(x)) return -1;
            if (string.IsNullOrEmpty(y)) return 1;

            // Tách các phần của mã theo dấu chấm
            string[] xParts = x.Split('.', StringSplitOptions.RemoveEmptyEntries);
            string[] yParts = y.Split('.', StringSplitOptions.RemoveEmptyEntries);

            // So sánh từng phần
            int minLength = Math.Min(xParts.Length, yParts.Length);
            for (int i = 0; i < minLength; i++)
            {
                // Thử parse thành số để so sánh
                bool xIsNumber = int.TryParse(xParts[i], out int xNum);
                bool yIsNumber = int.TryParse(yParts[i], out int yNum);

                if (xIsNumber && yIsNumber)
                {
                    // Cả hai đều là số, so sánh theo giá trị số
                    int comparison = xNum.CompareTo(yNum);
                    if (comparison != 0)
                        return comparison;
                }
                else
                {
                    // Ít nhất một trong hai không phải số, so sánh như chuỗi
                    int comparison = string.Compare(xParts[i], yParts[i], StringComparison.OrdinalIgnoreCase);
                    if (comparison != 0)
                        return comparison;
                }
            }

            // Nếu tất cả các phần đều bằng nhau, mã có ít phần hơn sẽ đứng trước
            return xParts.Length.CompareTo(yParts.Length);
        }

        // tìm kiếm giá thị trường theo mã hàng hóa hoặc tên hàng hóa trong table thêm mới và sửa
        public async Task<List<HHThiTruongTreeNodeDto>> SearchMatHangAsync(Guid nhomHangHoaId, string searchTerm, int maxResults = 50)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                return new List<HHThiTruongTreeNodeDto>();

            // Gọi repository để lấy dữ liệu
            var hangHoa = await _giaDichVuRepository.SearchMatHangAsync(nhomHangHoaId, searchTerm, maxResults);

            if (!hangHoa.Any())
                return new List<HHThiTruongTreeNodeDto>();

            // Chuyển đổi kết quả sang DTO
            var dtoList = hangHoa.Select(item => _mapper.Map<HHThiTruongTreeNodeDto>(item)).ToList();

            // Xây dựng cấu trúc cây
            var result = BuildTreeFromFlatList(dtoList, nhomHangHoaId);

            return result;
        }

        // Phương thức mới để xây dựng cây từ danh sách phẳng
        private List<HHThiTruongTreeNodeDto> BuildTreeFromFlatList(List<HHThiTruongTreeNodeDto> items, Guid rootId)
        {
            // Tạo dictionary để truy cập nhanh theo ID
            var lookup = items.ToDictionary(x => x.Id);
            
            // Kết quả cuối cùng
            var result = new List<HHThiTruongTreeNodeDto>();
            
            // Xây dựng cây
            foreach (var item in items)
            {
                // Nếu là con trực tiếp của root
                if (item.MatHangChaId == rootId)
                {
                    result.Add(item);
                }
                // Nếu có cha và cha nằm trong danh sách
                else if (item.MatHangChaId.HasValue && lookup.ContainsKey(item.MatHangChaId.Value))
                {
                    var parent = lookup[item.MatHangChaId.Value];
                    parent.MatHangCon.Add(item);
                }
                // Nếu không có cha trong danh sách nhưng không phải con trực tiếp của root
                else if (item.MatHangChaId.HasValue)
                {
                    // Thêm vào kết quả cuối cùng
                    result.Add(item);
                }
            }
            
            // Sắp xếp theo mã
            result.Sort((a, b) => CompareNumericCodes(a.Ma, b.Ma));
            
            // Sắp xếp các con của mỗi nút
            foreach (var item in items)
            {
                if (item.MatHangCon.Count > 0)
                {
                    item.MatHangCon.Sort((a, b) => CompareNumericCodes(a.Ma, b.Ma));
                }
            }
            
            return result;
        }
    }
}
