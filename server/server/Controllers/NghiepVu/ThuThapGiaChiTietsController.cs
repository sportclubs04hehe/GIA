using Application.DTOs.NghiepVu.ThuThapGiaChiTiet;
using Application.ServiceInterface.INghiepVu;
using Microsoft.AspNetCore.Mvc;
using server.Errors;

namespace server.Controllers.NghiepVu
{
    public class ThuThapGiaChiTietsController : BaseApiController
    {
        private readonly IThuThapGiaChiTietService _thuThapGiaChiTietService;
        private readonly IThuThapGiaThiTruongService _thuThapGiaThiTruongService;

        public ThuThapGiaChiTietsController(
            IThuThapGiaChiTietService thuThapGiaChiTietService,
            IThuThapGiaThiTruongService thuThapGiaThiTruongService)
        {
            _thuThapGiaChiTietService = thuThapGiaChiTietService;
            _thuThapGiaThiTruongService = thuThapGiaThiTruongService;
        }

        /// <summary>
        /// Lấy danh sách chi tiết giá của một phiếu thu thập giá
        /// </summary>
        [HttpGet("by-phieu/{thuThapGiaId}")]
        public async Task<ActionResult<IEnumerable<ThuThapGiaChiTietDto>>> GetByThuThapGiaId(Guid thuThapGiaId)
        {
            // Kiểm tra phiếu có tồn tại không
            var phieu = await _thuThapGiaThiTruongService.GetByIdAsync(thuThapGiaId);
            if (phieu == null)
                return NotFound(ApiResponse.NotFound("Không tìm thấy phiếu thu thập giá"));

            var result = await _thuThapGiaChiTietService.GetByThuThapGiaIdAsync(thuThapGiaId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết giá theo id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ThuThapGiaChiTietDto>> GetById(Guid id)
        {
            var result = await _thuThapGiaChiTietService.GetByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse.NotFound("Không tìm thấy chi tiết giá"));

            return Ok(result);
        }

        /// <summary>
        /// Lấy lịch sử giá của một mặt hàng
        /// </summary>
        [HttpGet("history/{hangHoaId}")]
        public async Task<ActionResult<IEnumerable<ThuThapGiaChiTietDto>>> GetLichSuGia(Guid hangHoaId)
        {
            var result = await _thuThapGiaChiTietService.GetLichSuGiaByHangHoaAsync(hangHoaId);
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới một chi tiết giá
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ThuThapGiaChiTietDto>>> Create(
            [FromBody] ThuThapGiaChiTietCreateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ThuThapGiaChiTietDto>.BadRequest(errors: ModelState));

            // Kiểm tra phiếu có tồn tại không
            var phieu = await _thuThapGiaThiTruongService.GetByIdAsync(model.ThuThapGiaThiTruongId);
            if (phieu == null)
                return BadRequest(ApiResponse<ThuThapGiaChiTietDto>.BadRequest(
                    "Phiếu thu thập giá không tồn tại"));

            var result = await _thuThapGiaChiTietService.CreateAsync(model);

            return Ok(ApiResponse<ThuThapGiaChiTietDto>.Created(
                result, THONGBAO, "Tạo mới chi tiết giá thành công"));
        }

        /// <summary>
        /// Cập nhật một chi tiết giá
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> Update(
            Guid id, [FromBody] ThuThapGiaChiTietUpdateDto model)
        {
            if (id != model.Id)
                return BadRequest(ApiResponse<Guid>.BadRequest("Id trong route và body không khớp"));

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.BadRequest(errors: ModelState));

            var exists = await _thuThapGiaChiTietService.GetByIdAsync(id);
            if (exists == null)
                return NotFound(ApiResponse<Guid>.NotFound("Không tìm thấy chi tiết giá"));

            var result = await _thuThapGiaChiTietService.UpdateAsync(model);

            if (result)
                return Ok(ApiResponse<Guid>.Success(
                    id, THONGBAO, "Cập nhật chi tiết giá thành công"));
            else
                return BadRequest(ApiResponse<Guid>.BadRequest("Cập nhật chi tiết giá thất bại"));
        }

        /// <summary>
        /// Xóa một chi tiết giá
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
        {
            var exists = await _thuThapGiaChiTietService.GetByIdAsync(id);
            if (exists == null)
                return NotFound(ApiResponse<Guid>.NotFound("Không tìm thấy chi tiết giá"));

            var result = await _thuThapGiaChiTietService.DeleteAsync(id);

            if (result)
                return Ok(ApiResponse<Guid>.Success(
                    id, THONGBAO, "Xóa chi tiết giá thành công"));
            else
                return BadRequest(ApiResponse<Guid>.BadRequest("Xóa chi tiết giá thất bại"));
        }

        /// <summary>
        /// Lưu nhiều chi tiết giá cùng một lúc
        /// </summary>
        [HttpPost("batch")]
        public async Task<ActionResult<ApiResponse<bool>>> SaveMany(
            [FromBody] List<ThuThapGiaChiTietCreateDto> models)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.BadRequest(errors: ModelState));

            if (models.Count == 0)
                return BadRequest(ApiResponse<bool>.BadRequest("Danh sách chi tiết giá trống"));

            // Kiểm tra tất cả chi tiết giá thuộc cùng một phiếu
            var thuThapGiaId = models.First().ThuThapGiaThiTruongId;
            if (models.Any(m => m.ThuThapGiaThiTruongId != thuThapGiaId))
                return BadRequest(ApiResponse<bool>.BadRequest(
                    "Tất cả chi tiết giá phải thuộc cùng một phiếu thu thập giá"));

            // Kiểm tra phiếu có tồn tại không
            var phieu = await _thuThapGiaThiTruongService.GetByIdAsync(thuThapGiaId);
            if (phieu == null)
                return BadRequest(ApiResponse<bool>.BadRequest(
                    "Phiếu thu thập giá không tồn tại"));

            var result = await _thuThapGiaChiTietService.SaveManyAsync(models);

            return Ok(ApiResponse<bool>.Success(
                result, THONGBAO, "Lưu danh sách chi tiết giá thành công"));
        }

        /// <summary>
        /// Tính toán lại tỷ lệ tăng giảm cho các chi tiết giá của một phiếu
        /// </summary>
        [HttpPost("{thuThapGiaId}/calculate")]
        public async Task<ActionResult<ApiResponse<bool>>> TinhToanTyLeTangGiam(Guid thuThapGiaId)
        {
            // Kiểm tra phiếu có tồn tại không
            var phieu = await _thuThapGiaThiTruongService.GetByIdAsync(thuThapGiaId);
            if (phieu == null)
                return NotFound(ApiResponse<bool>.NotFound("Không tìm thấy phiếu thu thập giá"));

            var result = await _thuThapGiaChiTietService.TinhToanTyLeTangGiamAsync(thuThapGiaId);

            return Ok(ApiResponse<bool>.Success(
                result, THONGBAO, "Tính toán tỷ lệ tăng giảm thành công"));
        }
    }
}
