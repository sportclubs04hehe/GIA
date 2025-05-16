using Application.DTOs.DanhMuc.HangHoasDto;
using Application.DTOs.DanhMuc.NhomHangHoasDto;
using Core.Helpers;

namespace Application.ServiceInterface.IDanhMuc
{
    public interface INhomHangHoaService
    {
        Task<NhomHangHoaDto> CreateNhomHangHoaAsync(CreateNhomHangHoaDto createDto);
        Task<NhomHangHoaDto> UpdateNhomHangHoaAsync(Guid id, UpdateNhomHangHoaDto updateDto);
        Task<NhomHangHoaDto> GetNhomHangHoaByIdAsync(Guid id);
        Task<PagedList<NhomHangHoaDto>> GetAllNhomHangHoasAsync(PaginationParams paginationParams);
        Task<PagedList<NhomHangHoaDto>> SearchNhomHangHoasAsync(SearchParams searchParams);
        Task<List<NhomHangHoaDto>> GetChildNhomHangHoasAsync(Guid parentId);
        Task<NhomHangHoaDetailDto> GetNhomHangHoaWithChildrenAsync(Guid id);
        Task<PagedList<HangHoaDto>> GetAllProductsInGroupAsync(Guid groupId, PaginationParams paginationParams);
        Task<List<NhomHangHoaDto>> GetRootNodesAsync();
        Task LoadChildrenRecursivelyAsync(NhomHangHoaDto parentDto);
    }
}
