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
        Task<PagedList<HHThiTruongDto>> GetAllAsync(PaginationParams paginationParams);
        Task<List<HHThiTruongDto>> GetAllParentCategoriesAsync();
        Task<List<CategoryInfoDto>> GetAllCategoriesWithChildInfoAsync();
        Task<bool> IsValidCodeAsync(string ma, Guid? parentId, Guid? exceptId = null);
        Task<bool> ValidateCreateDtoAsync(CreateHHThiTruongDto createDto);
        Task<bool> ValidateUpdateDtoAsync(UpdateHHThiTruongDto updateDto);
        Task<List<HHThiTruongDto>> CreateManyAsync(CreateManyHHThiTruongDto createDto);
        Task<PagedList<HHThiTruongTreeNodeDto>> SearchHierarchicalAsync(string searchTerm, PaginationParams paginationParams);
        Task<PagedList<HHThiTruongTreeNodeDto>> GetChildrenByParentIdPagedAsync(Guid parentId, PaginationParams paginationParams);
        /// <summary>
        /// Lấy đường dẫn đầy đủ từ gốc đến node bao gồm các node con cần thiết
        /// </summary>
        Task<List<HHThiTruongTreeNodeDto>> GetFullPathWithChildrenAsync(Guid targetNodeId, Guid? newItemId = null);
        Task<(bool IsSuccess, List<HHThiTruongDto> ImportedItems, List<string> Errors)> ImportFromExcelAsync(
            HHThiTruongBatchImportDto importDto);
        Task<CodeValidationResult> ValidateCodeAsync(string ma, Guid? parentId = null, Guid? exceptId = null);
        /// <summary>
        /// Kiểm tra nhiều mã mặt hàng cùng lúc
        /// </summary>
        /// <param name="codes">Danh sách mã cần kiểm tra</param>
        /// <param name="parentId">ID nhóm cha (null nếu là cấp cao nhất)</param>
        /// <returns>Danh sách kết quả kiểm tra cho mỗi mã</returns>
        Task<List<CodeValidationResult>> ValidateMultipleCodesAsync(List<string> codes, Guid? parentId = null);
    }
}
