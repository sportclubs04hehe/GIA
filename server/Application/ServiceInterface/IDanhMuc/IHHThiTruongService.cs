using Application.DTOs.DanhMuc.Dm_HangHoaThiTruongsDto;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterface.IDanhMuc
{
    public interface IHHThiTruongService
    {
        Task<HHThiTruongDto> CreateAsync(CreateHHThiTruongDto createDto);
        Task<List<HHThiTruongDto>> CreateManyAsync(CreateManyHHThiTruongDto createDto);
        Task<HHThiTruongDto> UpdateAsync(UpdateHHThiTruongDto updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteMultipleAsync(List<Guid> ids);
        Task<PagedList<HHThiTruongDto>> GetAllAsync(PaginationParams paginationParams);
        Task<List<HHThiTruongDto>> GetAllParentCategoriesAsync();
        Task<List<HHThiTruongTreeNodeDto>> GetHierarchicalCategoriesAsync();
        Task<HHThiTruongTreeNodeDto> GetWithChildrenAsync(Guid id);
        Task<PagedList<HHThiTruongDto>> SearchAsync(SearchParams searchParams);
        Task<bool> IsValidCodeAsync(string ma, Guid? parentId, Guid? exceptId = null);
        Task<bool> ValidateCreateDtoAsync(CreateHHThiTruongDto createDto);
        Task<bool> ValidateUpdateDtoAsync(UpdateHHThiTruongDto updateDto);
        Task<List<HHThiTruongTreeNodeDto>> GetChildrenByParentIdAsync(Guid parentId);
    }
}
