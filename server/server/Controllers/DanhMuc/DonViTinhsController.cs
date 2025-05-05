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
        [ProducesResponseType(typeof(PagedList<DonViTinhDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<DonViTinhDto>>> GetAll([FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _donViTinhService.GetAllAsync(paginationParams));
        }

        /// <summary>
        /// Tìm kiếm đơn vị tính
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<DonViTinhDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<DonViTinhDto>>> Search([FromQuery] SearchParams searchParams)
        {
            return await ExecutePagedAsync(() => _donViTinhService.SearchAsync(searchParams));
        }

        /// <summary>
        /// Lấy đơn vị tính theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DonViTinhDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<DonViTinhDto>> GetById(Guid id)
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
        [ProducesResponseType(typeof(DonViTinhDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<DonViTinhDto>> GetByMa(string ma)
        {
            var donViTinh = await _donViTinhService.GetByMaAsync(ma);
            
            if (donViTinh == null)
                return NotFound($"Không tìm thấy đơn vị tính với mã: {ma}");
                
            return Ok(donViTinh);
        }

        /// <summary>
        /// Thêm mới đơn vị tính
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<DonViTinhDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DonViTinhDto>>> Add([FromBody] DonViTinhCreateDto createDto)
        {
            if (createDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu đầu vào không hợp lệ"));

            try
            {
                var validation = await _donViTinhService.ValidateCreateAsync(createDto);
                if (!validation.IsValid)
                    return BadRequest(ApiResponse<DonViTinhDto>.BadRequest(
                        title: THONGBAO,
                        message: validation.ErrorMessage
                    ));

                var result = await _donViTinhService.CreateAsync(createDto);

                return CreatedAtAction(
                    nameof(GetById),
                    routeValues: new { id = result.Id },
                    value: ApiResponse<DonViTinhDto>.Created(
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
        [ProducesResponseType(typeof(IEnumerable<DonViTinhDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<DonViTinhDto>>> CreateMany(
            [FromBody] IEnumerable<DonViTinhCreateDto> createDtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var results = await _donViTinhService.CreateManyAsync(createDtos);
            return Ok(results);
        }

        /// <summary>
        /// Cập nhật đơn vị tính
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<DonViTinhDto>), StatusCodes.Status200OK)]
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

            return Ok(ApiResponse<DonViTinhDto>.Success(
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
    }
}
