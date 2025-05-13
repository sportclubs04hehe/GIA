using Microsoft.AspNetCore.Mvc;
using Application.ServiceInterface.IDanhMuc;
using Core.Helpers;
using Application.DTOs.DanhMuc.NhomHangHoasDto;
using Application.DTOs.DanhMuc.HangHoasDto;
using server.Errors;

namespace server.Controllers.DanhMuc
{ 
    public class NhomHangHoasController : BaseApiController
    {
        private readonly INhomHangHoaService _nhomHangHoaService;
        private readonly ILogger<NhomHangHoasController> _logger;

        public NhomHangHoasController(INhomHangHoaService nhomHangHoaService, ILogger<NhomHangHoasController> logger)
        {
            _nhomHangHoaService = nhomHangHoaService ?? throw new ArgumentNullException(nameof(nhomHangHoaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy nhóm hàng hóa theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<NhomHangHoaDto>> GetById(Guid id)
        {
            try
            {
                var nhomHangHoa = await _nhomHangHoaService.GetNhomHangHoaByIdAsync(id);

                if (nhomHangHoa == null)
                    return NotFound(ApiResponse.NotFound(THONGBAO, $"Không tìm thấy nhóm hàng hóa với ID: {id}"));

                return Ok(nhomHangHoa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy nhóm hàng hóa với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse.ServerError(THONGBAO, "Đã xảy ra lỗi khi lấy thông tin nhóm hàng hóa"));
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả nhóm hàng hóa có phân trang
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<NhomHangHoaDto>>> GetAll([FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _nhomHangHoaService.GetAllNhomHangHoasAsync(paginationParams));
        }

        /// <summary>
        /// Tìm kiếm nhóm hàng hóa
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<NhomHangHoaDto>>> Search([FromQuery] SearchParams searchParams)
        {
            return await ExecutePagedAsync(() => _nhomHangHoaService.SearchNhomHangHoasAsync(searchParams));
        }

        /// <summary>
        /// Lấy tất cả nhóm con của một nhóm hàng hóa
        /// </summary>
        [HttpGet("{parentId}/children")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<NhomHangHoaDto>>> GetChildGroups(Guid parentId)
        {
            try
            {
                var children = await _nhomHangHoaService.GetChildNhomHangHoasAsync(parentId);
                return Ok(children);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách nhóm con của nhóm: {Id}", parentId);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse.ServerError(THONGBAO, "Đã xảy ra lỗi khi lấy danh sách nhóm con"));
            }
        }

        /// <summary>
        /// Lấy nhóm hàng hóa với các nhóm con
        /// </summary>
        [HttpGet("{id}/with-children")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<NhomHangHoaDetailDto>> GetWithChildren(Guid id)
        {
            try
            {
                var nhomDetail = await _nhomHangHoaService.GetNhomHangHoaWithChildrenAsync(id);

                if (nhomDetail == null)
                    return NotFound(ApiResponse.NotFound(THONGBAO, $"Không tìm thấy nhóm hàng hóa với ID: {id}"));

                return Ok(nhomDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy nhóm hàng hóa và nhóm con với ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse.ServerError(THONGBAO, "Đã xảy ra lỗi khi lấy thông tin nhóm hàng hóa và nhóm con"));
            }
        }

        /// <summary>
        /// Lấy cấu trúc cây các nhóm hàng hóa gốc và các nhóm con của chúng
        /// </summary>
        [HttpGet("root-nodes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<NhomHangHoaDetailDto>> GetRootNodes()
        {
            try
            {
                var rootNodes = await _nhomHangHoaService.GetRootNodesAsync();
                
                // For each root node, load its complete hierarchy
                foreach (var node in rootNodes)
                {
                    await _nhomHangHoaService.LoadChildrenRecursivelyAsync(node);
                }

                var rootContainer = new NhomHangHoaDetailDto
                {
                    Id = Guid.Empty,
                    MaNhom = "ROOT",
                    TenNhom = "Tất cả nhóm hàng hóa",
                    NhomCon = rootNodes
                };

                return Ok(rootContainer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy cấu trúc cây nhóm hàng hóa");
                return StatusCode(500, ApiResponse.ServerError(
                    THONGBAO,
                    "Đã xảy ra lỗi khi lấy cấu trúc cây nhóm hàng hóa"
                ));
            }
        }

        /// <summary>
        /// Lấy tất cả hàng hóa thuộc một nhóm và các nhóm con
        /// </summary>
        [HttpGet("{groupId}/products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<HangHoaDto>>> GetProductsInGroup(
            Guid groupId, 
            [FromQuery] PaginationParams paginationParams)
        {
            return await ExecutePagedAsync(() => _nhomHangHoaService.GetAllProductsInGroupAsync(groupId, paginationParams));
        }

        /// <summary>
        /// Tạo mới nhóm hàng hóa
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<NhomHangHoaDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<NhomHangHoaDto>>> Create([FromBody] CreateNhomHangHoaDto createDto)
        {
            if (createDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu đầu vào không hợp lệ"));

            try
            {
                var result = await _nhomHangHoaService.CreateNhomHangHoaAsync(createDto);

                return CreatedAtAction(
                    nameof(GetById),
                    routeValues: new { id = result.Id },
                    value: ApiResponse<NhomHangHoaDto>.Created(
                        data: result,
                        title: THONGBAO,
                        message: "Tạo nhóm hàng hóa thành công"
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo nhóm hàng hóa");
                return ex is ArgumentException 
                    ? BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message))
                    : StatusCode(500, ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi tạo nhóm hàng hóa"));
            }
        }

        /// <summary>
        /// Cập nhật nhóm hàng hóa
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<NhomHangHoaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<NhomHangHoaDto>>> Update(Guid id, [FromBody] UpdateNhomHangHoaDto updateDto)
        {
            if (updateDto == null)
                return BadRequest(ApiResponse.BadRequest(THONGBAO, "Dữ liệu cập nhật không hợp lệ"));

            updateDto.Id = id;

            try
            {
                var result = await _nhomHangHoaService.UpdateNhomHangHoaAsync(id, updateDto);
                
                return Ok(ApiResponse<NhomHangHoaDto>.Success(
                    data: result,
                    title: THONGBAO,
                    message: "Cập nhật nhóm hàng hóa thành công"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật nhóm hàng hóa với ID: {Id}", id);
                
                if (ex.Message.Contains("không tồn tại"))
                    return NotFound(ApiResponse.NotFound(THONGBAO, ex.Message));
                    
                return ex is ArgumentException 
                    ? BadRequest(ApiResponse.BadRequest(THONGBAO, ex.Message))
                    : StatusCode(500, ApiResponse.ServerError(THONGBAO, "Có lỗi xảy ra khi cập nhật nhóm hàng hóa"));
            }
        }

        /// <summary>
        /// Xóa nhóm hàng hóa và các đối tượng liên quan
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Guid>>> Delete(Guid id)
        {
            try
            {
                // Kiểm tra tồn tại
                var nhomHangHoa = await _nhomHangHoaService.GetNhomHangHoaByIdAsync(id);
                if (nhomHangHoa == null)
                    return NotFound(ApiResponse<Guid>.NotFound(
                        message: $"Không tìm thấy nhóm hàng hóa với ID: {id}"
                    ));

                // Thực hiện xóa
                var result = await _nhomHangHoaService.DeleteNhomHangHoaAsync(id);
                if (result)
                {
                    return Ok(ApiResponse<Guid>.Success(
                        data: id,
                        title: THONGBAO,
                        message: "Xóa nhóm hàng hóa và dữ liệu liên quan thành công"
                    ));
                }
                
                return BadRequest(ApiResponse<Guid>.BadRequest(
                    message: "Không thể xóa nhóm hàng hóa"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhóm hàng hóa với ID: {Id}", id);
                return StatusCode(500, ApiResponse<Guid>.ServerError(
                    message: "Có lỗi xảy ra khi xóa nhóm hàng hóa"
                ));
            }
        }
    }
}
