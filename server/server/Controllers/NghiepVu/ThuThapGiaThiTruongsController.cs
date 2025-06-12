using Application.DTOs.NghiepVu.ThuThapGiaThiTruong;
using Application.ServiceInterface.INghiepVu;
using Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.Errors;
using System;
using System.Threading.Tasks;

namespace server.Controllers.NghiepVu
{
    public class ThuThapGiaThiTruongsController : BaseApiController
    {
        private readonly IThuThapGiaThiTruongService _service;

        public ThuThapGiaThiTruongsController(IThuThapGiaThiTruongService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách giá thị trường có phân trang
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<ThuThapGiaThiTruongDto>>> GetAll(
            [FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _service.GetAllAsync(paginationParams));
        }

        /// <summary>
        /// Tìm kiếm giá thị trường với phân trang
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<ThuThapGiaThiTruongDto>>> Search(
            [FromQuery] SearchParams searchParams)
        {
            return await ExecutePagedAsync(() => _service.SearchAsync(searchParams));
        }

        /// <summary>
        /// Lấy thông tin chi tiết giá thị trường theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ThuThapGiaThiTruongDto>>> GetById(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            
            if (entity == null)
                return NotFound(ApiResponse<ThuThapGiaThiTruongDto>.NotFound(
                    message: $"Không tìm thấy dữ liệu giá thị trường với ID: {id}"));
            
            return Ok(ApiResponse<ThuThapGiaThiTruongDto>.Success(entity));
        }

        /// <summary>
        /// Tạo mới dữ liệu giá thị trường
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ThuThapGiaThiTruongDto>>> Create(
            [FromBody] ThuThapGiaThiTruongCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ThuThapGiaThiTruongDto>.BadRequest(
                    message: "Dữ liệu không hợp lệ", 
                    errors: ModelState));
            
            var result = await _service.CreateAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                ApiResponse<ThuThapGiaThiTruongDto>.Created(
                    result,
                    message: "Tạo mới dữ liệu giá thị trường thành công"));
        }

        /// <summary>
        /// Cập nhật dữ liệu giá thị trường
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> Update(
            [FromBody] ThuThapGiaThiTruongUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.BadRequest(
                    message: "Dữ liệu không hợp lệ", 
                    errors: ModelState));
            
            return await ExecuteWithExistenceCheckAsync(
                updateDto.Id,
                () => _service.ExistsAsync(updateDto.Id),
                () => _service.UpdateAsync(updateDto),
                notFoundMessage: $"Không tìm thấy dữ liệu giá thị trường với ID: {updateDto.Id}",
                successMessage: "Cập nhật dữ liệu giá thị trường thành công",
                failureMessage: "Không thể cập nhật dữ liệu giá thị trường");
        }

        /// <summary>
        /// Xóa dữ liệu giá thị trường
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
        {
            return await ExecuteWithExistenceCheckAsync(
                id,
                () => _service.ExistsAsync(id),
                () => _service.DeleteAsync(id),
                notFoundMessage: $"Không tìm thấy dữ liệu giá thị trường với ID: {id}",
                successMessage: "Xóa dữ liệu giá thị trường thành công",
                failureMessage: "Không thể xóa dữ liệu giá thị trường");
        }
    }
}
