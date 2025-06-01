using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.DTOs.DanhMuc.Helpers;
using Application.ServiceInterface.IDanhMuc;
using Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using server.Errors;
using server.Helpers;

namespace server.Controllers.DanhMuc
{
    public class Dm_HHThiTruongsController : BaseApiController
    {
        private readonly IHHThiTruongService _hhThiTruongService;   
        private readonly ILogger<Dm_HHThiTruongsController> _logger;

        public Dm_HHThiTruongsController(IHHThiTruongService hhThiTruongService, 
            ILogger<Dm_HHThiTruongsController> logger)
        {
            _hhThiTruongService = hhThiTruongService ?? throw new ArgumentNullException(nameof(hhThiTruongService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy thông tin mặt hàng thị trường theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HHThiTruongDto>> GetById(Guid id)
        {
            try
            {
                var matHang = await _hhThiTruongService.GetByIdAsync(id);
                return Ok(matHang);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse.NotFound(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving market product with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi lấy thông tin mặt hàng"));
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả mặt hàng thị trường có phân trang
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<HHThiTruongDto>>> GetAll([FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _hhThiTruongService.GetAllAsync(paginationParams));
        }

        /// <summary>
        /// Lấy danh sách các nhóm hàng hóa cha (không có mặt hàng cha)
        /// </summary>
        [HttpGet("parents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<HHThiTruongDto>>> GetAllParentCategories()
        {
            try
            {
                var parentCategories = await _hhThiTruongService.GetAllParentCategoriesAsync();
                return Ok(parentCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parent categories");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi lấy danh sách nhóm hàng hóa cha"));
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả các nhóm hàng hóa kèm thông tin có chứa con hay không
        /// </summary>
        [HttpGet("categories-with-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CategoryInfoDto>>> GetAllCategoriesWithChildInfo()
        {
            try
            {
                var categories = await _hhThiTruongService.GetAllCategoriesWithChildInfoAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories with child info");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi lấy danh sách nhóm hàng hóa"));
            }
        }

        /// <summary>
        /// Thêm mới mặt hàng thị trường
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<HHThiTruongDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<HHThiTruongDto>>> Create([FromBody] CreateHHThiTruongDto createDto)
        {
            if (createDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu đầu vào không hợp lệ"));

            try
            {
                var result = await _hhThiTruongService.CreateAsync(createDto);

                return CreatedAtAction(
                    nameof(GetById),
                    routeValues: new { id = result.Id },
                    value: ApiResponse<HHThiTruongDto>.Created(
                        data: result,
                        title: THONGBAO,
                        message: "Tạo mặt hàng thị trường thành công"
                    )
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating market product");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi thêm mới mặt hàng thị trường"));
            }
        }

        /// <summary>
        /// Cập nhật thông tin mặt hàng thị trường
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<HHThiTruongDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<HHThiTruongDto>>> Update(Guid id, [FromBody] UpdateHHThiTruongDto updateDto)
        {
            if (updateDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu cập nhật không hợp lệ"));

            try
            {
                updateDto.Id = id;
                var result = await _hhThiTruongService.UpdateAsync(updateDto);

                return Ok(ApiResponse<HHThiTruongDto>.Success(
                    data: result,
                    title: THONGBAO,
                    message: "Cập nhật mặt hàng thị trường thành công"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse.NotFound(THONGBAO, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating market product with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi cập nhật mặt hàng thị trường"));
            }
        }

        /// <summary>
        /// Xóa mặt hàng thị trường theo ID
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
        {
            try
            {
                var result = await _hhThiTruongService.DeleteAsync(id);
                if (result)
                {
                    return Ok(ApiResponse<Guid>.Success(
                        data: id,
                        title: THONGBAO,
                        message: "Xóa mặt hàng thị trường thành công"
                    ));
                }
                
                return NotFound(ApiResponse<Guid>.NotFound(
                    message: "Không tìm thấy mặt hàng thị trường"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting market product with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi xóa mặt hàng thị trường"));
            }
        }

        /// <summary>
        /// Xóa nhiều mặt hàng thị trường cùng lúc
        /// </summary>
        [HttpDelete("batch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<Guid>>>> DeleteMultiple([FromBody] List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Danh sách ID cần xóa không được để trống"));

            try
            {
                var result = await _hhThiTruongService.DeleteMultipleAsync(ids);
                if (result)
                {
                    return Ok(ApiResponse<List<Guid>>.Success(
                        data: ids,
                        title: THONGBAO,
                        message: $"Đã xóa thành công {ids.Count} mặt hàng thị trường"
                    ));
                }

                return BadRequest(ApiResponse<List<Guid>>.BadRequest(
                    title: THONGBAO,
                    message: "Có lỗi xảy ra khi xóa, một số mặt hàng không tồn tại"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple market products");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi xóa nhiều mặt hàng thị trường"));
            }
        }

        /// <summary>
        /// Thêm nhiều mặt hàng thị trường cùng lúc
        /// </summary>
        [HttpPost("batch")]
        [ProducesResponseType(typeof(ApiResponse<List<HHThiTruongDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<HHThiTruongDto>>>> CreateMany([FromBody] CreateManyHHThiTruongDto createDto)
        {
            if (createDto == null || !createDto.Items.Any())
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Danh sách mặt hàng không được để trống"));

            try
            {
                var result = await _hhThiTruongService.CreateManyAsync(createDto);

                return Created(
                    string.Empty,  // No specific URI for batch creation
                    ApiResponse<List<HHThiTruongDto>>.Created(
                        data: result,
                        title: THONGBAO,
                        message: $"Đã tạo thành công {result.Count} mặt hàng thị trường"
                    )
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating multiple market products");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi thêm nhiều mặt hàng thị trường"));
            }
        }

        /// <summary>
        /// Lấy danh sách mặt hàng con theo mặt hàng cha có phân trang
        /// </summary>
        [HttpGet("children/{parentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<HHThiTruongTreeNodeDto>>> GetChildrenByParent(
            Guid parentId, 
            [FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _hhThiTruongService.GetChildrenByParentIdPagedAsync(parentId, paginationParams));
        }

        /// <summary>
        /// Tìm kiếm phân cấp với phân trang (đã mở rộng các node chứa kết quả tìm kiếm)
        /// </summary>
        [HttpGet("search-hierarchical")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<HHThiTruongTreeNodeDto>>> SearchHierarchical(
            [FromQuery] string searchTerm,
            [FromQuery] PaginationParams paginationParams)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(ApiResponse.BadRequest(THONGBAO, "Cần nhập từ khóa tìm kiếm"));
                }

                // Giới hạn kích thước trang tối đa là 100 bản ghi 
                if (paginationParams.PageSize > 15)
                    paginationParams.PageSize = 15;

                var results = await _hhThiTruongService.SearchHierarchicalAsync(searchTerm, paginationParams);

                // Thiết lập header phân trang
                Response.AddPaginationHeader(
                    results.CurrentPage,
                    results.PageSize,
                    results.TotalCount,
                    results.TotalPages);

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing hierarchical search");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi tìm kiếm"));
            }
        }

        /// <summary>
        /// Lấy đường dẫn đầy đủ từ gốc đến node bao gồm các node con cần thiết
        /// </summary>
        [HttpGet("full-path/{targetNodeId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<HHThiTruongTreeNodeDto>>> GetFullPathWithChildren(
            Guid targetNodeId,
            [FromQuery] Guid? newItemId = null)
        {
            try
            {
                var result = await _hhThiTruongService.GetFullPathWithChildrenAsync(targetNodeId, newItemId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse.NotFound(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy đường dẫn đầy đủ đến node có ID: {Id}", targetNodeId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi lấy đường dẫn đầy đủ"));
            }
        }

        /// <summary>
        /// Import mặt hàng thị trường từ Excel
        /// </summary>
        [HttpPost("import-from-excel")]
        [ProducesResponseType(typeof(ApiResponse<List<HHThiTruongDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<HHThiTruongDto>>>> ImportFromExcel(
            [FromBody] HHThiTruongBatchImportDto importDto)
        {
            if (importDto == null || !importDto.Items.Any())
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Danh sách mặt hàng không được để trống"));

            if (!importDto.MatHangChaId.HasValue)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Cần chọn nhóm cha cho các mặt hàng import"));

            try
            {
                var (isSuccess, importedItems, errors) = await _hhThiTruongService.ImportFromExcelAsync(importDto);

                if (!isSuccess && errors.Any())
                {
                    return BadRequest(
                        ApiResponse<List<HHThiTruongDto>>.BadRequest(
                            title: THONGBAO,
                            message: "Có lỗi xảy ra khi import mặt hàng",
                            errors: new
                            {
                                ErrorMessages = errors,
                                SuccessfullyImported = importedItems.Count
                            }
                        )
                    );
                }

                _logger.LogInformation("Import Excel successful. Total items: {Count}", importedItems.Count);
                if (errors.Any() && importedItems.Any())
                {
                    return StatusCode(
                        StatusCodes.Status207MultiStatus,
                        ApiResponse<List<HHThiTruongDto>>.PartialSuccess(
                            data: importedItems,
                            title: "Import thành công một phần",
                            message: $"Đã import thành công {importedItems.Count} mặt hàng, {errors.Count} lỗi",
                            errors: errors
                        )
                    );
                }

                // Trường hợp thành công hoàn toàn
                return StatusCode(
                    StatusCodes.Status201Created,
                    ApiResponse<List<HHThiTruongDto>>.Created(
                        data: importedItems,
                        title: THONGBAO,
                        message: $"Đã import thành công {importedItems.Count} mặt hàng"
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
                    ApiResponse.ServerError(THONGBAO, "Đã có lỗi xảy ra khi import mặt hàng thị trường từ Excel")
                );
            }
        }

        /// <summary>
        /// Kiểm tra mã mặt hàng đã tồn tại trong cùng nhóm hay chưa
        /// </summary>
        [HttpGet("validate-code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<CodeValidationResult>>> ValidateCode(
            [FromQuery] string ma,
            [FromQuery] Guid? parentId = null,
            [FromQuery] Guid? exceptId = null)
        {
            if (string.IsNullOrWhiteSpace(ma))
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Mã không được để trống"));

            try
            {
                var result = await _hhThiTruongService.ValidateCodeAsync(ma, parentId, exceptId);
                
                return Ok(ApiResponse<CodeValidationResult>.Success(
                    data: result,
                    title: THONGBAO,
                    message: result.IsValid 
                        ? "Mã hợp lệ" 
                        : "Mã không hợp lệ"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kiểm tra mã '{Code}' cho nhóm ID: {ParentId}", ma, parentId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi kiểm tra mã mặt hàng"));
            }
        }

        /// <summary>
        /// Kiểm tra nhiều mã mặt hàng cùng lúc trong cùng nhóm
        /// </summary>
        [HttpPost("validate-multiple-codes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<CodeValidationResult>>>> ValidateMultipleCodes(
            [FromBody] MultipleCodeValidationRequestDto request)
        {
            if (request == null || request.Codes == null || !request.Codes.Any())
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Danh sách mã cần kiểm tra không được để trống"));
            }

            try
            {
                var results = await _hhThiTruongService.ValidateMultipleCodesAsync(request.Codes, request.ParentId);

                // Đếm số mã hợp lệ
                int validCount = results.Count(r => r.IsValid);

                return Ok(ApiResponse<List<CodeValidationResult>>.Success(
                    data: results,
                    title: THONGBAO,
                    message: validCount == results.Count
                        ? "Tất cả mã đều hợp lệ"
                        : $"Có {results.Count - validCount}/{results.Count} mã không hợp lệ"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kiểm tra nhiều mã cho nhóm ID: {ParentId}", request.ParentId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi kiểm tra mã mặt hàng"));
            }
        }
    }
}
