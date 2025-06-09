using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Application.DTOs.DanhMuc.Dm_ThuocTinhDto;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterface.IDanhMuc
{
    public interface IDm_ThuocTinhService
    {
        Task<Dm_ThuocTinhDto> GetByIdAsync(Guid id);
        Task<Dm_ThuocTinhDto> CreateAsync(Dm_ThuocTinhCreateDto createDto);
        Task<Dm_ThuocTinhDto> UpdateAsync(Dm_ThuocTinhUpdateDto updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteMultipleAsync(List<Guid> ids);
        Task<List<Dm_ThuocTinhDto>> GetAllParentCategoriesAsync();
        Task<List<Dm_ThuocTinhCategoryInfoDto>> GetAllCategoriesWithChildInfoAsync();
        Task<bool> IsValidCodeAsync(string ma, Guid? parentId, Guid? exceptId = null);
        Task<bool> ValidateCreateDtoAsync(Dm_ThuocTinhCreateDto createDto);
        Task<bool> ValidateUpdateDtoAsync(Dm_ThuocTinhUpdateDto updateDto);
        Task<List<Dm_ThuocTinhDto>> CreateManyAsync(Dm_ThuocTinhCreateManyDto createDto);
        Task<PagedList<Dm_ThuocTinhTreeNodeDto>> SearchHierarchicalAsync(string searchTerm, PaginationParams paginationParams);
        Task<PagedList<Dm_ThuocTinhTreeNodeDto>> GetChildrenByParentIdPagedAsync(Guid parentId, PaginationParams paginationParams);
        Task<List<Dm_ThuocTinhTreeNodeDto>> GetFullPathWithChildrenAsync(Guid targetNodeId, Guid? newItemId = null);
        Task<CodeValidationResult> ValidateCodeAsync(string ma, Guid? parentId = null, Guid? exceptId = null);
        Task<List<CodeValidationResult>> ValidateMultipleCodesAsync(List<string> codes, Guid? parentId = null);
    }
}
