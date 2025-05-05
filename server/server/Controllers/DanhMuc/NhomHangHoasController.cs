using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.ServiceInterface.IDanhMuc;
using Core.Helpers;
using server.Helpers;
using Application.DTOs.DanhMuc.NhomHangHoasDto;
using Application.ServiceImplement.DanhMuc;
using server.Errors;

namespace server.Controllers.DanhMuc
{ 
    public class NhomHangHoasController : BaseApiController
    {
        private readonly INhomHangHoaService _nhomHangHoaService;

        public NhomHangHoasController(INhomHangHoaService nhomHangHoaService)
        {
            _nhomHangHoaService = nhomHangHoaService;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<PagedList<NhomHangHoaDto>>> GetAll([FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _nhomHangHoaService.GetAllAsync(paginationParams));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NhomHangHoaDto>> GetById(Guid id)
        {
            try
            {
                var result = await _nhomHangHoaService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("maNhom/{maNhom}")]
        public async Task<ActionResult<NhomHangHoaDto>> GetByMaNhom(string maNhom)
        {
            try
            {
                var result = await _nhomHangHoaService.GetByMaNhomAsync(maNhom);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<NhomHangHoaDto>> Create([FromBody] CreateNhomHangHoaDto createDto)
        {
            try
            {
                var result = await _nhomHangHoaService.AddAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNhomHangHoaDto updateDto)
        //{
        //    if (updateDto == null)
        //        return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu cập nhật không hợp lệ"));

        //    updateDto.Id = id;
        //    var (isSuccess, errorMessage) = await _hangHoaService.UpdateAsync(updateDto);

        //    if (!isSuccess)
        //    {
        //        if (!string.IsNullOrEmpty(errorMessage))
        //            return BadRequest(ApiResponse.BadRequest(THONGBAO, errorMessage));

        //        return NotFound(ApiResponse.NotFound(THONGBAO, $"Mặt hàng này không tồn tại"));
        //    }

        //    var dto = await _hangHoaService.GetByIdAsync(id);
        //    if (dto == null)
        //        return NotFound(ApiResponse.NotFound(THONGBAO, $"Không tìm thấy sản phẩm sau khi cập nhật"));

        //    // Return success with ApiResponse format
        //    return Ok(ApiResponse<HangHoaDto>.Success(dto, THONGBAO, "Cập nhật mặt hàng thành công"));
        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
             => await ExecuteWithExistenceCheckAsync(
                id,
                 () => _nhomHangHoaService.ExistsAsync(id),
                 () => _nhomHangHoaService.DeleteAsync(id),
                 notFoundMessage: $"Không tìm thấy mặt hàng",
                 successMessage: $"Xóa mặt hàng thành công"
         );

        [HttpGet("root")]
        public async Task<ActionResult<List<NhomHangHoaDto>>> GetRootGroups()
        {
            var result = await _nhomHangHoaService.GetRootGroupsAsync();
            return Ok(result);
        }

        [HttpGet("{id}/children")]
        public async Task<ActionResult<List<NhomHangHoaDto>>> GetChildGroups(Guid id)
        {
            try
            {
                var result = await _nhomHangHoaService.GetChildGroupsAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedList<NhomHangHoaDto>>> Search([FromQuery] SearchParams searchParams)
        {
            return await ExecutePagedAsync(() => _nhomHangHoaService.SearchAsync(searchParams));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedList<NhomHangHoaDto>>> Filter([FromQuery] SpecificationParams specParams)
        {
            return await ExecutePagedAsync(() => _nhomHangHoaService.GetWithFilterAsync(specParams));
        }

        [HttpGet("hierarchy")]
        public async Task<ActionResult<List<NhomHangHoaDto>>> GetHierarchy()
        {
            var result = await _nhomHangHoaService.GetHierarchyAsync();
            return Ok(result);
        }

        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> Exists(Guid id)
        {
            var result = await _nhomHangHoaService.ExistsAsync(id);
            return Ok(result);
        }

        [HttpGet("exists/code/{maNhom}")]
        public async Task<ActionResult<bool>> ExistsByMaNhom(string maNhom)
        {
            var result = await _nhomHangHoaService.ExistsByMaNhomAsync(maNhom);
            return Ok(result);
        }
    }
}
