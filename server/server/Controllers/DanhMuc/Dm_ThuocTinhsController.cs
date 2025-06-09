using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.DTOs.DanhMuc.Dm_ThuocTinhDto;
using Application.DTOs.DanhMuc.Helpers;
using Application.ServiceInterface.IDanhMuc;
using Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using server.Errors;
using server.Helpers;

namespace server.Controllers.DanhMuc
{
    public class Dm_ThuocTinhsController : BaseApiController
    {
        private readonly IDm_ThuocTinhService _thuocTinhService;   
        private readonly ILogger<Dm_ThuocTinhsController> _logger;

        public Dm_ThuocTinhsController(
            IDm_ThuocTinhService thuocTinhService, 
            ILogger<Dm_ThuocTinhsController> logger)
        {
            _thuocTinhService = thuocTinhService ?? throw new ArgumentNullException(nameof(thuocTinhService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy thông tin thuộc tính theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Dm_ThuocTinhDto>> GetById(Guid id)
        {
            try
            {
                var thuocTinh = await _thuocTinhService.GetByIdAsync(id);
                return Ok(thuocTinh);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse.NotFound(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thuộc tính ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi lấy thông tin thuộc tính"));
            }
        }

        /// <summary>
        /// Lấy danh sách các thuộc tính cha (không có thuộc tính cha)
        /// </summary>
        [HttpGet("parents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Dm_ThuocTinhDto>>> GetAllParentCategories()
        {
            try
            {
                var parentCategories = await _thuocTinhService.GetAllParentCategoriesAsync();
                return Ok(parentCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thuộc tính cha");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi lấy danh sách thuộc tính cha"));
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả các thuộc tính kèm thông tin có chứa con hay không
        /// </summary>
        [HttpGet("categories-with-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Dm_ThuocTinhCategoryInfoDto>>> GetAllCategoriesWithChildInfo()
        {
            try
            {
                var categories = await _thuocTinhService.GetAllCategoriesWithChildInfoAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thuộc tính với thông tin con");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi lấy danh sách thuộc tính"));
            }
        }

        /// <summary>
        /// Thêm mới thuộc tính
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Dm_ThuocTinhDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Dm_ThuocTinhDto>>> Create([FromBody] Dm_ThuocTinhCreateDto createDto)
        {
            if (createDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu đầu vào không hợp lệ"));

            try
            {
                var result = await _thuocTinhService.CreateAsync(createDto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    ApiResponse<Dm_ThuocTinhDto>.Created(
                        data: result,
                        title: THONGBAO,
                        message: "Tạo thuộc tính thành công"
                    )
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thuộc tính");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi thêm thuộc tính"));
            }
        }

        /// <summary>
        /// Cập nhật thông tin thuộc tính
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<Dm_ThuocTinhDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Dm_ThuocTinhDto>>> Update(Guid id, [FromBody] Dm_ThuocTinhUpdateDto updateDto)
        {
            if (updateDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu cập nhật không hợp lệ"));

            try
            {
                updateDto.Id = id;
                var result = await _thuocTinhService.UpdateAsync(updateDto);

                return Ok(ApiResponse<Dm_ThuocTinhDto>.Success(
                    data: result,
                    title: THONGBAO,
                    message: "Cập nhật thuộc tính thành công"
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
                _logger.LogError(ex, "Lỗi khi cập nhật thuộc tính ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi cập nhật thuộc tính"));
            }
        }

        /// <summary>
        /// Xóa thuộc tính theo ID
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
        {
            try
            {
                var result = await _thuocTinhService.DeleteAsync(id);
                if (result)
                {
                    return Ok(ApiResponse<Guid>.Success(
                        data: id,
                        title: THONGBAO,
                        message: "Xóa thuộc tính thành công"
                    ));
                }
                
                return NotFound(ApiResponse<Guid>.NotFound(message: "Không tìm thấy thuộc tính"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa thuộc tính ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi xóa thuộc tính"));
            }
        }

        /// <summary>
        /// Xóa nhiều thuộc tính cùng lúc
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
                var result = await _thuocTinhService.DeleteMultipleAsync(ids);
                if (result)
                {
                    return Ok(ApiResponse<List<Guid>>.Success(
                        data: ids,
                        title: THONGBAO,
                        message: $"Đã xóa thành công {ids.Count} thuộc tính"
                    ));
                }

                return BadRequest(ApiResponse<List<Guid>>.BadRequest(
                    title: THONGBAO,
                    message: "Có lỗi xảy ra khi xóa, một số thuộc tính không tồn tại"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhiều thuộc tính");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi xóa nhiều thuộc tính"));
            }
        }

        /// <summary>
        /// Thêm nhiều thuộc tính cùng lúc
        /// </summary>
        [HttpPost("batch")]
        [ProducesResponseType(typeof(ApiResponse<List<Dm_ThuocTinhDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<Dm_ThuocTinhDto>>>> CreateMany([FromBody] Dm_ThuocTinhCreateManyDto createDto)
        {
            if (createDto == null || !createDto.ThuocTinhs.Any())
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Danh sách thuộc tính không được để trống"));

            try
            {
                var result = await _thuocTinhService.CreateManyAsync(createDto);

                return Created(
                    string.Empty,
                    ApiResponse<List<Dm_ThuocTinhDto>>.Created(
                        data: result,
                        title: THONGBAO,
                        message: $"Đã tạo thành công {result.Count} thuộc tính"
                    )
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo nhiều thuộc tính");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi thêm nhiều thuộc tính"));
            }
        }

        /// <summary>
        /// Lấy danh sách thuộc tính con theo thuộc tính cha có phân trang
        /// </summary>
        [HttpGet("children/{parentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<Dm_ThuocTinhTreeNodeDto>>> GetChildrenByParent(
            Guid parentId, 
            [FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _thuocTinhService.GetChildrenByParentIdPagedAsync(parentId, paginationParams));
        }

        /// <summary>
        /// Tìm kiếm phân cấp với phân trang
        /// </summary>
        [HttpGet("search-hierarchical")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<Dm_ThuocTinhTreeNodeDto>>> SearchHierarchical(
            [FromQuery] string searchTerm,
            [FromQuery] PaginationParams paginationParams)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(ApiResponse.BadRequest(THONGBAO, "Cần nhập từ khóa tìm kiếm"));
                }

                // Giới hạn kích thước trang
                if (paginationParams.PageSize > 15)
                    paginationParams.PageSize = 15;

                var results = await _thuocTinhService.SearchHierarchicalAsync(searchTerm, paginationParams);

                // Thiết lập header phân trang
                Response.AddPaginationHeader(results);

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm phân cấp thuộc tính");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi tìm kiếm"));
            }
        }

        /// <summary>
        /// Lấy đường dẫn đầy đủ từ gốc đến node bao gồm các node con
        /// </summary>
        [HttpGet("full-path/{targetNodeId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Dm_ThuocTinhTreeNodeDto>>> GetFullPathWithChildren(
            Guid targetNodeId,
            [FromQuery] Guid? newItemId = null)
        {
            try
            {
                var result = await _thuocTinhService.GetFullPathWithChildrenAsync(targetNodeId, newItemId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse.NotFound(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy đường dẫn đến thuộc tính ID: {Id}", targetNodeId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi lấy đường dẫn đầy đủ"));
            }
        }

        /// <summary>
        /// Kiểm tra mã thuộc tính đã tồn tại trong cùng nhóm hay chưa
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
                var result = await _thuocTinhService.ValidateCodeAsync(ma, parentId, exceptId);
                
                return Ok(ApiResponse<CodeValidationResult>.Success(
                    data: result,
                    title: THONGBAO,
                    message: result.IsValid ? "Mã hợp lệ" : "Mã không hợp lệ"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kiểm tra mã '{Code}' cho nhóm ID: {ParentId}", ma, parentId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi kiểm tra mã thuộc tính"));
            }
        }

        /// <summary>
        /// Kiểm tra nhiều mã thuộc tính cùng lúc trong cùng nhóm
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
                var results = await _thuocTinhService.ValidateMultipleCodesAsync(request.Codes, request.ParentId);
                
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
                _logger.LogError(ex, "Lỗi kiểm tra nhiều mã thuộc tính cho nhóm ID: {ParentId}", request.ParentId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi kiểm tra mã thuộc tính"));
            }
        }
    }
}
