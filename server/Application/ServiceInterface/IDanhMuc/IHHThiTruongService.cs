using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterface.IDanhMuc
{
    public interface IHHThiTruongService
    {
        Task<HHThiTruongDto> GetByIdAsync(Guid id);
        Task<HHThiTruongDto> CreateAsync(CreateHHThiTruongDto createDto);
        Task<HHThiTruongDto> UpdateAsync(UpdateHHThiTruongDto updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteMultipleAsync(List<Guid> ids);
        Task<List<HHThiTruongDto>> GetAllParentCategoriesAsync();
        Task<List<CategoryInfoDto>> GetAllCategoriesWithChildInfoAsync();
        Task<bool> IsValidCodeAsync(string ma, Guid? parentId, Guid? exceptId = null);
        Task<bool> ValidateCreateDtoAsync(CreateHHThiTruongDto createDto);
        Task<bool> ValidateUpdateDtoAsync(UpdateHHThiTruongDto updateDto);
        Task<List<HHThiTruongDto>> CreateManyAsync(CreateManyHHThiTruongDto createDto);
        Task<PagedList<HHThiTruongTreeNodeDto>> SearchHierarchicalAsync(string searchTerm, PaginationParams paginationParams);
        Task<PagedList<HHThiTruongTreeNodeDto>> GetChildrenByParentIdPagedAsync(Guid parentId, PaginationParams paginationParams);
        Task<List<HHThiTruongTreeNodeDto>> GetFullPathWithChildrenAsync(Guid targetNodeId, Guid? newItemId = null);
        Task<(bool IsSuccess, List<HHThiTruongDto> ImportedItems, List<string> Errors)> ImportFromExcelAsync(
            HHThiTruongBatchImportDto importDto);
        Task<CodeValidationResult> ValidateCodeAsync(string ma, Guid? parentId = null, Guid? exceptId = null);
        Task<List<CodeValidationResult>> ValidateMultipleCodesAsync(List<string> codes, Guid? parentId = null);
        Task<PagedList<HHThiTruongDto>> GetAllDescendantsByParentIdPagedAsync(
            Guid parentId,
            PaginationParams paginationParams);
    }
}
