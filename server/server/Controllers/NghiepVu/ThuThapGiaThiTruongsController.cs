using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.DTOs.DanhMuc.Dm_LoaiGia;
using Application.DTOs.NghiepVu.ThuThapGiaThiTruong;
using Application.ServiceInterface.INghiepVu;
using Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using server.Errors;
using server.Helpers;

namespace server.Controllers.NghiepVu
{
    public class ThuThapGiaThiTruongsController : BaseApiController
    {
        private readonly IThuThapGiaThiTruongService _thuThapGiaThiTruongService;

        public ThuThapGiaThiTruongsController(IThuThapGiaThiTruongService thuThapGiaThiTruongService)
        {
            _thuThapGiaThiTruongService = thuThapGiaThiTruongService;
        }

        #region Lấy danh sách dữ liệu cơ bản cho dropdown

        /// <summary>
        /// Lấy danh sách loại giá để hiển thị trong dropdown
        /// </summary>
        [HttpGet("loai-gia")]
        public async Task<ActionResult<IEnumerable<LoaiGiaDto>>> GetLoaiGia()
        {
            var result = await _thuThapGiaThiTruongService.GetLoaiGiaAsync();
            return Ok(result);
        }

        #endregion

        #region Quản lý phiếu thu thập giá

        /// <summary>
        /// Lấy danh sách phiếu thu thập giá (có phân trang)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedList<ThuThapGiaThiTruongDto>>> GetThuThapGiaThiTruongs(
            [FromQuery] PaginationParams paginationParams)
        {
            var result = await _thuThapGiaThiTruongService.GetAllAsync(paginationParams);
            Response.AddPaginationHeader(result);
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm phiếu thu thập giá theo các tiêu chí
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<PagedList<ThuThapGiaThiTruongDto>>> SearchThuThapGiaThiTruongs(
            [FromQuery] SearchParams searchParams)
        {
            var result = await _thuThapGiaThiTruongService.SearchAsync(searchParams);
            Response.AddPaginationHeader(result);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin một phiếu thu thập giá theo id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ThuThapGiaThiTruongDto>> GetById(Guid id)
        {
            var result = await _thuThapGiaThiTruongService.GetByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse.NotFound("Không tìm thấy phiếu thu thập giá"));

            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một phiếu thu thập giá cùng danh sách chi tiết giá
        /// </summary>
        [HttpGet("{id}/details")]
        public async Task<ActionResult<ThuThapGiaThiTruongDto>> GetWithDetails(Guid id)
        {
            var result = await _thuThapGiaThiTruongService.GetWithDetailsAsync(id);
            if (result == null)
                return NotFound(ApiResponse.NotFound("Không tìm thấy phiếu thu thập giá"));

            return Ok(result);
        }

        /// <summary>
        /// Tạo mới phiếu thu thập giá kèm danh sách chi tiết giá
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ThuThapGiaThiTruongDto>>> Create(
            [FromBody] CreateThuThapGiaModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ThuThapGiaThiTruongDto>.BadRequest(
                    errors: ModelState));

            var result = await _thuThapGiaThiTruongService.CreateThuThapGiaVaChiTietAsync(
                model.ThuThapGia, model.ChiTietGia);

            return Ok(ApiResponse<ThuThapGiaThiTruongDto>.Created(
                result,
                THONGBAO,
                "Tạo mới phiếu thu thập giá thành công"));
        }

        /// <summary>
        /// Cập nhật phiếu thu thập giá kèm danh sách chi tiết giá
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> Update(
            Guid id, [FromBody] UpdateThuThapGiaModel model)
        {
            if (id != model.ThuThapGia.Id)
                return BadRequest(ApiResponse<Guid>.BadRequest("Id trong route và body không khớp"));

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.BadRequest(errors: ModelState));

            var exists = await _thuThapGiaThiTruongService.GetByIdAsync(id);
            if (exists == null)
                return NotFound(ApiResponse<Guid>.NotFound("Không tìm thấy phiếu thu thập giá"));

            var result = await _thuThapGiaThiTruongService.UpdateThuThapGiaVaChiTietAsync(
                model.ThuThapGia, model.ChiTietGia);

            if (result)
                return Ok(ApiResponse<Guid>.Success(
                    id, THONGBAO, "Cập nhật phiếu thu thập giá thành công"));
            else
                return BadRequest(ApiResponse<Guid>.BadRequest("Cập nhật phiếu thu thập giá thất bại"));
        }

        /// <summary>
        /// Xóa một phiếu thu thập giá
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
        {
            var exists = await _thuThapGiaThiTruongService.GetByIdAsync(id);
            if (exists == null)
                return NotFound(ApiResponse<Guid>.NotFound("Không tìm thấy phiếu thu thập giá"));

            var result = await _thuThapGiaThiTruongService.DeleteAsync(id);

            if (result)
                return Ok(ApiResponse<Guid>.Success(
                    id, THONGBAO, "Xóa phiếu thu thập giá thành công"));
            else
                return BadRequest(ApiResponse<Guid>.BadRequest("Xóa phiếu thu thập giá thất bại"));
        }

        #endregion

        #region Danh mục hàng hóa thị trường

        /// <summary>
        /// Lấy tất cả các mặt hàng con theo id cha (bao gồm cả lồng nhau)
        /// </summary>
        [HttpGet("recursive-children/{parentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<HHThiTruongTreeNodeDto>>>> GetAllChildrenRecursive(
            Guid parentId, 
            [FromQuery] DateTime? ngayNhap = null)
        {
            try
            {
                DateTime? utcNgayNhap = ngayNhap.HasValue 
                    ? DateTime.SpecifyKind(ngayNhap.Value, DateTimeKind.Utc) 
                    : null;
                    
                var result = await _thuThapGiaThiTruongService.GetAllChildrenRecursiveAsync(parentId, utcNgayNhap);

                return Ok(ApiResponse<List<HHThiTruongTreeNodeDto>>.Success(
                    data: result,
                    title: THONGBAO,
                    message: "Lấy danh sách mặt hàng con thành công"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, $"Có lỗi xảy ra khi lấy danh sách mặt hàng con: {ex.Message}"));
            }
        }

        /// <summary>
        /// Tìm kiếm mặt hàng theo nhóm hàng hóa
        /// </summary>
        [HttpGet("search-mat-hang/{nhomHangHoaId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<HHThiTruongTreeNodeDto>>>> SearchMatHang(
            Guid nhomHangHoaId,
            [FromQuery] string q,
            [FromQuery] DateTime? ngayNhap = null,
            [FromQuery] int maxResults = 25)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            {
                return Ok(ApiResponse<List<HHThiTruongTreeNodeDto>>.Success(
                    data: new List<HHThiTruongTreeNodeDto>(),
                    title: THONGBAO,
                    message: "Vui lòng nhập ít nhất 2 ký tự để tìm kiếm"
                ));
            }

            try
            {
                DateTime? utcNgayNhap = ngayNhap.HasValue 
                    ? DateTime.SpecifyKind(ngayNhap.Value.Date, DateTimeKind.Utc) // Chỉ lấy phần Date
                    : null;
                    
                var result = await _thuThapGiaThiTruongService.SearchMatHangAsync(nhomHangHoaId, q, utcNgayNhap, maxResults);

                return Ok(ApiResponse<List<HHThiTruongTreeNodeDto>>.Success(
                    data: result,
                    title: THONGBAO,
                    message: $"Tìm thấy {result.Count} mặt hàng phù hợp"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, $"Lỗi khi tìm kiếm mặt hàng: {ex.Message}"));
            }
        }
        #endregion
    }
}
