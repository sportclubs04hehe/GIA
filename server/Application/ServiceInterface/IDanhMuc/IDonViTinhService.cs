using Application.DTOs.DanhMuc.DonViTinhDto;
using Core.Helpers;

namespace Application.ServiceInterface.IDanhMuc
{
    public interface IDonViTinhService
    {
        Task<DonViTinhDto> GetByIdAsync(Guid id);
        Task<DonViTinhDto> GetByMaAsync(string ma);
        Task<PagedList<DonViTinhDto>> GetAllAsync(PaginationParams paginationParams);
        Task<PagedList<DonViTinhDto>> SearchAsync(SearchParams searchParams);
        Task<DonViTinhDto> CreateAsync(DonViTinhCreateDto createDto);
        Task<IEnumerable<DonViTinhDto>> CreateManyAsync(IEnumerable<DonViTinhCreateDto> createDtos);
        Task<(bool isSuccess, string? errorMessage)> UpdateAsync(DonViTinhUpdateDto updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
