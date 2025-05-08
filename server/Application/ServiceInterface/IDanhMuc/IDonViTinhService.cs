using Application.DTOs.DanhMuc.DonViTinhDto;
using Application.DTOs.DanhMuc.HangHoasDto;
using Core.Helpers;

namespace Application.ServiceInterface.IDanhMuc
{
    public interface IDonViTinhService
    {
        Task<DonViTinhsDto> GetByIdAsync(Guid id);
        Task<DonViTinhsDto> GetByMaAsync(string ma);
        Task<PagedList<DonViTinhsDto>> GetAllAsync(PaginationParams paginationParams);
        Task<PagedList<DonViTinhSelectDto>> GetAllSelectAsync(PaginationParams paginationParams);
        Task<PagedList<DonViTinhsDto>> SearchAsync(SearchParams searchParams);
        Task<DonViTinhsDto> CreateAsync(DonViTinhCreateDto createDto);
        Task<IEnumerable<DonViTinhsDto>> CreateManyAsync(IEnumerable<DonViTinhCreateDto> createDtos);
        Task<(bool isSuccess, string? errorMessage)> UpdateAsync(DonViTinhUpdateDto updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByMaAsync(string maMatHang, Guid? excludeId, CancellationToken cancellationToken = default);
        Task<(bool IsValid, string ErrorMessage)> ValidateCreateAsync(DonViTinhCreateDto dto);
    }
}
