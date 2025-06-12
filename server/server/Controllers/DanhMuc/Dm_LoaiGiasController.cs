using Application.DTOs.DanhMuc.Dm_LoaiGia;
using Application.ServiceInterface.IDanhMuc;
using Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.Errors;
using System;
using System.Threading.Tasks;

namespace server.Controllers.DanhMuc
{
    public class Dm_LoaiGiasController : BaseApiController
    {
        private readonly IDm_LoaiGiaService _service;

        public Dm_LoaiGiasController(IDm_LoaiGiaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách loại giá có phân trang
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<LoaiGiaDto>>> GetAll(
            [FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _service.GetAllAsync(paginationParams));
        }

        /// <summary>
        /// Tìm kiếm loại giá với phân trang
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<LoaiGiaDto>>> Search(
            [FromQuery] SearchParams searchParams)
        {
            return await ExecutePagedAsync(() => _service.SearchAsync(searchParams));
        }

        /// <summary>
        /// Lấy thông tin chi tiết loại giá theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<LoaiGiaDto>>> GetById(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            
            if (entity == null)
                return NotFound(ApiResponse<LoaiGiaDto>.NotFound(
                    message: $"Không tìm thấy loại giá với ID: {id}"));
            
            return Ok(ApiResponse<LoaiGiaDto>.Success(entity, 
                message: "Lấy thông tin loại giá thành công"));
        }

        /// <summary>
        /// Tạo mới loại giá
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<LoaiGiaDto>>> Create(
            [FromBody] LoaiGiaCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<LoaiGiaDto>.BadRequest(
                    message: "Dữ liệu không hợp lệ", 
                    errors: ModelState));
            
            var result = await _service.CreateAsync(createDto);
            
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                ApiResponse<LoaiGiaDto>.Created(
                    result, 
                    message: "Tạo mới loại giá thành công"));
        }

        /// <summary>
        /// Cập nhật loại giá
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> Update(
            Guid id,
            [FromBody] LoaiGiaUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.BadRequest(
                    message: "Dữ liệu không hợp lệ", 
                    errors: ModelState));
            
            return await ExecuteWithExistenceCheckAsync(
                id,
                () => _service.ExistsAsync(id),
                () => _service.UpdateAsync(id, updateDto),
                notFoundMessage: $"Không tìm thấy loại giá với ID: {id}",
                successMessage: "Cập nhật loại giá thành công",
                failureMessage: "Không thể cập nhật loại giá");
        }

        /// <summary>
        /// Xóa loại giá
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
                notFoundMessage: $"Không tìm thấy loại giá với ID: {id}",
                successMessage: "Xóa loại giá thành công",
                failureMessage: "Không thể xóa loại giá");
        }
    }
}
