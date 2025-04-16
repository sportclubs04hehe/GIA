using Application.DTOs.HangHoaDto;
using Application.DTOs.NhomHangHoaDto;
using Application.ServiceInterface.IDanhMuc;
using Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using server.Helpers;

namespace server.Controllers.DanhMuc
{
    public class HangHoasController : BaseApiController
    {
        private readonly IHangHoaService _hangHoaService;
        private readonly ILogger<HangHoasController> _logger;

        public HangHoasController(IHangHoaService hangHoaService, ILogger<HangHoasController> logger)
        {
            _hangHoaService = hangHoaService ?? throw new ArgumentNullException(nameof(hangHoaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy thông tin hàng hóa theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HangHoaDto>> GetById(Guid id)
        {
            try
            {
                var hangHoa = await _hangHoaService.GetByIdAsync(id);

                if (hangHoa == null)
                    return NotFound($"Không tìm thấy hàng hóa với ID: {id}");

                return Ok(hangHoa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the product");
            }
        }

        /// <summary>
        /// Lấy thông tin hàng hóa theo mã mặt hàng
        /// </summary>
        [HttpGet("ma/{maMatHang}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HangHoaDto>> GetByMaMatHang(string maMatHang)
        {
            if (string.IsNullOrEmpty(maMatHang))
                return BadRequest("Mã mặt hàng không được để trống");

            try
            {
                var hangHoa = await _hangHoaService.GetByMaMatHangAsync(maMatHang);

                if (hangHoa == null)
                    return NotFound($"Không tìm thấy hàng hóa với mã: {maMatHang}");

                return Ok(hangHoa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with code: {Code}", maMatHang);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the product");
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả hàng hóa có phân trang
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<HangHoaDto>>> GetAll([FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _hangHoaService.GetAllAsync(paginationParams));
        }

        /// <summary>
        /// Lấy danh sách hàng hóa theo nhóm hàng hóa
        /// </summary>
        [HttpGet("nhom/{nhomHangHoaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<HangHoaDto>>> GetByNhomHangHoa(Guid nhomHangHoaId,
            [FromQuery] PaginationParams paginationParams)
        {
            if (nhomHangHoaId == Guid.Empty)
                return BadRequest("ID nhóm hàng hóa không hợp lệ");

            try
            {
                var hangHoas = await _hangHoaService.GetByNhomHangHoaAsync(nhomHangHoaId, paginationParams);

                Response.AddPaginationHeader(hangHoas);

                return Ok(hangHoas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products by category ID: {CategoryId}", nhomHangHoaId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving products");
            }
        }

        /// <summary>
        /// Tìm kiếm hàng hóa theo từ khóa
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<HangHoaDto>>> Search([FromQuery] SearchParams searchParams)
        {
            try
            {
                var hangHoas = await _hangHoaService.SearchAsync(searchParams);

                Response.AddPaginationHeader(hangHoas);

                return Ok(hangHoas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with parameters: {Params}", searchParams);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching products");
            }
        }

        /// <summary>
        /// Lọc và sắp xếp danh sách hàng hóa theo nhiều tiêu chí
        /// </summary>
        [HttpGet("filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<HangHoaDto>>> GetWithFilter([FromQuery] SpecificationParams specParams)
        {
            try
            {
                var hangHoas = await _hangHoaService.GetWithFilterAsync(specParams);

                Response.AddPaginationHeader(hangHoas);

                return Ok(hangHoas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering products with specification parameters: {Params}", specParams);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while filtering products");
            }
        }

        /// <summary>
        /// Lấy danh sách hàng hóa đang có hiệu lực
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<HangHoaDto>>> GetActiveHangHoa([FromQuery] PaginationParams paginationParams)
        {
            try
            {
                var hangHoas = await _hangHoaService.GetActiveHangHoaAsync(paginationParams);

                Response.AddPaginationHeader(hangHoas);

                return Ok(hangHoas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active products");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving active products");
            }
        }

        /// <summary>
        /// Thêm mới hàng hóa
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HangHoaDto>> Add([FromBody] HangHoaCreateDto createDto)
        {
            if (createDto == null)
                return BadRequest("Request body cannot be empty");

            try
            {
                // Validate input
                var validation = await _hangHoaService.ValidateCreateHangHoaAsync(createDto);
                if (!validation.IsValid)
                    return BadRequest(validation.ErrorMessage);

                var result = await _hangHoaService.AddAsync(createDto);

                _logger.LogInformation("Product created successfully: {ProductId}", result.Id);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument when adding product: {Product}", createDto);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new product: {Product}", createDto);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the product");
            }
        }

        /// <summary>
        /// Cập nhật thông tin hàng hóa
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] HangHoaUpdateDto updateDto)
        {
            if (updateDto == null)
                return BadRequest("Request body cannot be empty");

            // Set the ID from the route parameter
            updateDto.Id = id;

            return await ExecuteValidatedUpdateAsync(
                id: id,
                updateDto: updateDto,
                existsCheck: () => _hangHoaService.ExistsAsync(id),
                validator: () => _hangHoaService.ValidateUpdateHangHoaAsync(updateDto),
                updateOperation: () => _hangHoaService.UpdateAsync(updateDto),
                notFoundMessage: $"Product with ID {id} not found",
                successMessage: $"Product updated successfully: {id}",
                failureMessage: "Failed to update the product"
            );
        }

        /// <summary>
        /// Xóa hàng hóa theo ID
        /// </summary>
        /// <summary>
        /// Xóa hàng hóa theo ID
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await ExecuteWithExistenceCheckAsync(
                id: id,
                existsCheck: () => _hangHoaService.ExistsAsync(id),
                operation: () => _hangHoaService.DeleteAsync(id),
                notFoundMessage: $"Product with ID {id} not found",
                successMessage: $"Product deleted successfully: {id}"
            );
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của hàng hóa theo ID
        /// </summary>
        [HttpGet("exists/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Exists(Guid id)
        {
            try
            {
                var exists = await _hangHoaService.ExistsAsync(id);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if product exists: {ProductId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while checking product existence");
            }
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của hàng hóa theo mã mặt hàng
        /// </summary>
        [HttpGet("exists-by-ma/{maMatHang}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> ExistsByMaMatHang(string maMatHang)
        {
            if (string.IsNullOrEmpty(maMatHang))
                return BadRequest("Product code cannot be empty");

            try
            {
                var exists = await _hangHoaService.ExistsByMaMatHangAsync(maMatHang);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if product exists by code: {ProductCode}", maMatHang);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while checking product existence");
            }
        }

        /// <summary>
        /// Đếm số lượng hàng hóa trong hệ thống
        /// </summary>
        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                var count = await _hangHoaService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting products");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while counting products");
            }
        }
    }
}
