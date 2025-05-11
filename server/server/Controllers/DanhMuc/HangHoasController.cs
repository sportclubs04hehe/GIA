using Application.DTOs.DanhMuc.HangHoasDto;
using Application.ServiceInterface.IDanhMuc;
using Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using server.Errors;
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
        [ProducesResponseType(typeof(ApiResponse<HangHoaDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<HangHoaDto>>> Add([FromBody] HangHoaCreateDto createDto)
        {
            if (createDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu đầu vào không hợp lệ"));

            try
            {
                var validation = await _hangHoaService.ValidateCreateHangHoaAsync(createDto);
                if (!validation.IsValid)
                    return BadRequest(ApiResponse<HangHoaDto>.BadRequest(
                        title: THONGBAO,
                        message: validation.ErrorMessage
                    ));

                var result = await _hangHoaService.AddAsync(createDto);

                return CreatedAtAction(
                    nameof(GetById),
                    routeValues: new { id = result.Id },
                    value: ApiResponse<HangHoaDto>.Created(
                        data: result,
                        title: THONGBAO,
                        message: "Tạo mặt hàng thành công"
                    )
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi thêm mới mặt hàng"));
            }
        }


        /// <summary>
        /// Thêm nhiều hàng hóa cùng lúc
        /// </summary>
        [HttpPost("batch")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<HangHoaDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMany([FromBody] List<HangHoaCreateDto> createDtos)
        {
            if (createDtos == null || !createDtos.Any())
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Danh sách mặt hàng không được để trống"));

            try
            {
                var (isSuccess, data, errors) = await _hangHoaService.CreateManyAsync(createDtos);

                if (!isSuccess)
                {
                    return BadRequest(
                        ApiResponse<IEnumerable<HangHoaDto>>
                           .BadRequest(
                               title: THONGBAO,
                               message: "Có một số mục không hợp lệ, vui lòng kiểm tra chi tiết bên dưới",
                               errors: new
                               {
                                   InvalidItems = errors,
                                   Inserted = data
                               }
                           )
                    );
                }

                _logger.LogInformation("Batch create successful. Total items: {Count}", data.Count);
                return Created(
                    uri: Url.Action(nameof(CreateMany), "HangHoas"),
                    value: ApiResponse<IEnumerable<HangHoaDto>>.Created(
                        data,
                        title: "Batch create successful",
                        message: $"{data.Count()} mặt hàng đã được thêm thành công"
                    )
                );
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument when batch creating products");
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating multiple items");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError("Server error", "Đã có lỗi xảy ra khi thêm nhiều mặt hàng")
                );
            }
        }


        /// <summary>
        /// Cập nhật thông tin hàng hóa, trả về DTO bản ghi đã được sửa
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<HangHoaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] HangHoaUpdateDto updateDto)
        {
            if (updateDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu cập nhật không hợp lệ"));

            updateDto.Id = id;
            var (isSuccess, errorMessage) = await _hangHoaService.UpdateAsync(updateDto);

            if (!isSuccess)
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(ApiResponse.BadRequest(THONGBAO, errorMessage));

                return NotFound(ApiResponse.NotFound(THONGBAO, $"Mặt hàng này không tồn tại"));
            }

            var dto = await _hangHoaService.GetByIdAsync(id);
            if (dto == null)
                return NotFound(ApiResponse.NotFound(THONGBAO, $"Không tìm thấy sản phẩm sau khi cập nhật"));

            // Return success with ApiResponse format
            return Ok(ApiResponse<HangHoaDto>.Success(dto, THONGBAO, "Cập nhật mặt hàng thành công"));
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
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
            => await ExecuteWithExistenceCheckAsync(
                id,
                () => _hangHoaService.ExistsAsync(id),
                () => _hangHoaService.DeleteAsync(id),
                notFoundMessage: $"Không tìm thấy mặt hàng",
                successMessage: $"Xóa mặt hàng thành công"
        );

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
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> ExistsByMaMatHang(
            [FromRoute] string maMatHang,
            [FromQuery] Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(maMatHang))
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Mã mặt hàng không được để trống"));

            try
            {
                var exists = await _hangHoaService
                    .ExistsByMaMatHangAsync(maMatHang, excludeId, cancellationToken);

                return Ok(exists);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra tồn tại mã hàng {MaMatHang}", maMatHang);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Đã có lỗi khi kiểm tra tồn tại hàng hóa")
                );
            }
        }

        /// <summary>
        /// Kiểm tra danh sách mã mặt hàng đã tồn tại
        /// </summary>
        [HttpPost("check-existing-codes")]
        [ProducesResponseType(typeof(Dictionary<string, bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Dictionary<string, bool>>> CheckExistingCodes([FromBody] List<string> maCodes)
        {
            if (maCodes == null || !maCodes.Any())
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Danh sách mã cần kiểm tra không được để trống"));

            try
            {
                var result = new Dictionary<string, bool>();

                foreach (var maCode in maCodes)
                {
                    if (!string.IsNullOrWhiteSpace(maCode))
                    {
                        var exists = await _hangHoaService.ExistsByMaMatHangAsync(maCode);
                        result[maCode] = exists;
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking multiple product codes");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Đã có lỗi khi kiểm tra mã mặt hàng ở hàng hóa thị trường")
                );
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

        /// <summary>
        /// Import hàng hóa từ Excel
        /// </summary>
        [HttpPost("import-from-excel")]
        [ProducesResponseType(typeof(ApiResponse<List<HangHoaDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<HangHoaDto>>>> ImportFromExcel(
            [FromBody] List<HangHoaImportDto> importDtos)
        {
            if (importDtos == null || !importDtos.Any())
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Danh sách hàng hóa không được để trống"));

            try
            {
                var (isSuccess, importedItems, errors) = await _hangHoaService.ImportFromExcelAsync(importDtos);

                if (!isSuccess && errors.Any())
                {
                    return BadRequest(
                        ApiResponse<List<HangHoaDto>>.BadRequest(
                            title: THONGBAO,
                            message: "Có lỗi xảy ra khi import hàng hóa",
                            errors: new
                            {
                                ErrorMessages = errors,
                                SuccessfullyImported = importedItems.Count
                            }
                        )
                    );
                }

                _logger.LogInformation("Import Excel successful. Total items: {Count}", importedItems.Count);

                // Nếu có một số items thành công, một số lỗi, trả về 207 MultiStatus
                if (errors.Any() && importedItems.Any())
                {
                    return StatusCode(
                        StatusCodes.Status207MultiStatus,
                        ApiResponse<List<HangHoaDto>>.PartialSuccess(
                            data: importedItems,
                            title: "Import thành công một phần",
                            message: $"Đã import thành công {importedItems.Count} hàng hóa, {errors.Count} lỗi",
                            errors: errors
                        )
                    );
                }

                // Trường hợp thành công hoàn toàn
                return StatusCode(
                    StatusCodes.Status201Created,
                    ApiResponse<List<HangHoaDto>>.Created(
                        data: importedItems,
                        title: THONGBAO,
                        message: $"Đã import thành công {importedItems.Count} hàng hóa"
                    )
                );
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument when importing products from Excel");
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing items from Excel");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Đã có lỗi xảy ra khi import hàng hóa từ Excel")
                );
            }
        }
    }
}
