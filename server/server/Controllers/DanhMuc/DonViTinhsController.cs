using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.ServiceInterface.IDanhMuc;
using Application.DTOs.DanhMuc.DonViTinhDto;
using Core.Helpers;
using System.Net;
using server.Errors;
using Application.ServiceImplement.DanhMuc;

namespace server.Controllers.DanhMuc
{
    public class DonViTinhsController : BaseApiController
    {
        private readonly IDonViTinhService _donViTinhService;

        public DonViTinhsController(IDonViTinhService donViTinhService)
        {
            _donViTinhService = donViTinhService;
        }

        /// <summary>
        /// Lấy danh sách đơn vị tính có phân trang
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<DonViTinhsDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<DonViTinhsDto>>> GetAll([FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _donViTinhService.GetAllAsync(paginationParams));
        }

        [HttpGet("get-all-select")]
        [ProducesResponseType(typeof(PagedList<DonViTinhSelectDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<DonViTinhSelectDto>>> GetAllSelect(
            [FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _donViTinhService.GetAllSelectAsync(paginationParams));
        }

        /// <summary>
        /// Tìm kiếm đơn vị tính
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<DonViTinhsDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<DonViTinhsDto>>> Search([FromQuery] SearchParams searchParams)
        {
            return await ExecutePagedAsync(() => _donViTinhService.SearchAsync(searchParams));
        }

        /// <summary>
        /// Lấy đơn vị tính theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DonViTinhsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<DonViTinhsDto>> GetById(Guid id)
        {
            var donViTinh = await _donViTinhService.GetByIdAsync(id);
            
            if (donViTinh == null)
                return NotFound($"Không tìm thấy đơn vị tính với ID: {id}");
                
            return Ok(donViTinh);
        }

        /// <summary>
        /// Lấy đơn vị tính theo mã
        /// </summary>
        [HttpGet("ma/{ma}")]
        [ProducesResponseType(typeof(DonViTinhsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<DonViTinhsDto>> GetByMa(string ma)
        {
            var donViTinh = await _donViTinhService.GetByMaAsync(ma);
            
            if (donViTinh == null)
                return NotFound($"Không tìm thấy đơn vị tính với mã: {ma}");
                
            return Ok(donViTinh);
        }

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
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Mã không được để trống"));

            try
            {
                var exists = await _donViTinhService
                    .ExistsByMaAsync(maMatHang, excludeId, cancellationToken);

                return Ok(exists);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Đã có lỗi khi kiểm tra tồn tại hàng hóa")
                );
            }
        }

        /// <summary>
        /// Thêm mới đơn vị tính
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<DonViTinhsDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DonViTinhsDto>>> Add([FromBody] DonViTinhCreateDto createDto)
        {
            if (createDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu đầu vào không hợp lệ"));

            try
            {
                var validation = await _donViTinhService.ValidateCreateAsync(createDto);
                if (!validation.IsValid)
                    return BadRequest(ApiResponse<DonViTinhsDto>.BadRequest(
                        title: THONGBAO,
                        message: validation.ErrorMessage
                    ));

                var result = await _donViTinhService.CreateAsync(createDto);

                return CreatedAtAction(
                    nameof(GetById),
                    routeValues: new { id = result.Id },
                    value: ApiResponse<DonViTinhsDto>.Created(
                        data: result,
                        title: THONGBAO,
                        message: "Tạo đơn vị tính thành công"
                    )
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception ex)
            {
                // Log ex nếu cần
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi tạo mới đơn vị tính")
                );
            }
        }

        /// <summary>
        /// Tạo nhiều đơn vị tính
        /// </summary>
        [HttpPost("many")]
        [ProducesResponseType(typeof(IEnumerable<DonViTinhsDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<DonViTinhsDto>>> CreateMany(
            [FromBody] IEnumerable<DonViTinhCreateDto> createDtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var results = await _donViTinhService.CreateManyAsync(createDtos);
            return Ok(results);
        }

        /// <summary>
        /// Tạo hoặc lấy nhiều đơn vị tính (nếu đã tồn tại thì không thêm mới)
        /// </summary>
        [HttpPost("create-or-get-many")]
        [ProducesResponseType(typeof(IEnumerable<DonViTinhsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DonViTinhsDto>>> CreateOrGetMany(
            [FromBody] IEnumerable<DonViTinhCreateDto> createDtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var results = new List<DonViTinhsDto>();
            
            foreach (var dto in createDtos)
            {
                try
                {
                    // Try to get existing unit
                    var existingUnit = await _donViTinhService.GetByMaAsync(dto.Ma);
                    
                    if (existingUnit != null)
                    {
                        // Unit already exists, add it to results
                        results.Add(existingUnit);
                    }
                    else
                    {
                        // Unit doesn't exist, create it
                        var validation = await _donViTinhService.ValidateCreateAsync(dto);
                        if (validation.IsValid)
                        {
                            var newUnit = await _donViTinhService.CreateAsync(dto);
                            results.Add(newUnit);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log exception if needed
                    continue;
                }
            }
            
            return Ok(results);
        }

        /// <summary>
        /// Cập nhật đơn vị tính
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<DonViTinhsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] DonViTinhUpdateDto updateDto)
        {
            if (updateDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu cập nhật không hợp lệ"));

            updateDto.Id = id;

            var (isSuccess, errorMessage) = await _donViTinhService.UpdateAsync(updateDto);

            if (!isSuccess)
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(ApiResponse.BadRequest(THONGBAO, errorMessage));

                return NotFound(ApiResponse.NotFound(THONGBAO, $"Đơn vị tính này không tồn tại"));
            }

            var dto = await _donViTinhService.GetByIdAsync(id);
            if (dto == null)
                return NotFound(ApiResponse.NotFound(THONGBAO, "Không tìm thấy đơn vị tính sau khi cập nhật"));

            return Ok(ApiResponse<DonViTinhsDto>.Success(
                dto,
                THONGBAO,
                $"Đơn vị tính '{dto.Ten}' đã được cập nhật thành công"));
        }

        /// <summary>
        /// Xóa đơn vị tính
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
            => await ExecuteWithExistenceCheckAsync(
                id,
                () => _donViTinhService.ExistsAsync(id),
                () => _donViTinhService.DeleteAsync(id),
                notFoundMessage: $"Không tìm thấy mặt hàng",
                successMessage: $"Xóa mặt hàng thành công"
        );

        /// <summary>
        /// Lấy đơn vị tính theo tên
        /// </summary>
        [HttpGet("ten/{ten}")]
        [ProducesResponseType(typeof(DonViTinhsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DonViTinhsDto>> GetByTen(string ten)
        {
            if (string.IsNullOrWhiteSpace(ten))
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Tên không được để trống"));

            try
            {
                var donViTinh = await _donViTinhService.GetByTenAsync(ten);
                
                if (donViTinh == null)
                    return NotFound(ApiResponse.NotFound(THONGBAO, $"Không tìm thấy đơn vị tính với tên: {ten}"));
                    
                return Ok(ApiResponse<DonViTinhsDto>.Success(donViTinh, THONGBAO, "Lấy đơn vị tính thành công"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi tìm đơn vị tính")
                );
            }
        }

        /// <summary>
        /// Thêm mới đơn vị tính nếu chưa tồn tại
        /// </summary>
        [HttpPost("add-if-not-exists")]
        [ProducesResponseType(typeof(ApiResponse<DonViTinhsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DonViTinhsDto>>> AddIfNotExists([FromBody] string ten)
        {
            if (string.IsNullOrWhiteSpace(ten))
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Tên không được để trống"));

            try
            {
                var result = await _donViTinhService.AddIfNotExistsAsync(ten);
                var statusCode = StatusCodes.Status200OK;
                var message = "Đơn vị tính được lấy thành công";
                
                // Fix: Handle nullable CreatedDate properly
                if (result.CreatedDate.HasValue && 
                    DateTime.UtcNow.Subtract(result.CreatedDate.Value).TotalMinutes < 1)
                {
                    statusCode = StatusCodes.Status201Created;
                    message = "Đơn vị tính được tạo thành công";
                }

                return StatusCode(
                    statusCode,
                    ApiResponse<DonViTinhsDto>.Success(
                        data: result, 
                        title: THONGBAO, 
                        message: message
                    )
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi thêm đơn vị tính")
                );
            }
        }

        /// <summary>
        /// Thêm nhiều đơn vị tính một lúc
        /// </summary>
        [HttpPost("bulk-add")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DonViTinhsDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<DonViTinhsDto>>>> BulkAdd(
            [FromBody] IEnumerable<DonViTinhCreateDto> createDtos)
        {
            if (createDtos == null || !createDtos.Any())
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu đầu vào không hợp lệ hoặc trống"));

            try
            {
                // Validate all DTOs first to prevent partial insertions
                foreach (var dto in createDtos)
                {
                    var validation = await _donViTinhService.ValidateCreateAsync(dto);
                    if (!validation.IsValid)
                        return BadRequest(ApiResponse.BadRequest(THONGBAO, 
                            $"Lỗi với đơn vị '{dto.Ten}': {validation.ErrorMessage}"));
                }

                var results = await _donViTinhService.BulkAddAsync(createDtos);
                
                return StatusCode(
                    StatusCodes.Status201Created,
                    ApiResponse<IEnumerable<DonViTinhsDto>>.Created(
                        data: results,
                        title: THONGBAO,
                        message: $"Đã thêm thành công {results.Count()} đơn vị tính"
                    )
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi thêm hàng loạt đơn vị tính")
                );
            }
        }

        /// <summary>
        /// Tìm hoặc tạo nhiều đơn vị tính theo tên
        /// </summary>
        [HttpPost("get-or-create-by-names")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<string, Guid>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Dictionary<string, Guid>>>> GetOrCreateManyByNames(
            [FromBody] List<string> tenDonViTinhs)
        {
            if (tenDonViTinhs == null || !tenDonViTinhs.Any())
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Danh sách tên đơn vị tính không được để trống"));

            try
            {
                var result = await _donViTinhService.GetOrCreateManyByNameAsync(tenDonViTinhs);

                return Ok(ApiResponse<Dictionary<string, Guid>>.Success(
                    data: result,
                    title: THONGBAO,
                    message: $"Đã xử lý {result.Count} đơn vị tính thành công"
                ));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi xử lý đơn vị tính")
                );
            }
        }

    }
}
