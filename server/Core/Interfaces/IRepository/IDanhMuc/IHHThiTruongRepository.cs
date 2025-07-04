﻿using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using System.Linq.Expressions;

namespace Core.Interfaces.IRepository.IDanhMuc
{
    public interface IHHThiTruongRepository : IGenericRepository<Dm_HangHoaThiTruong>
    {
        Task<List<Dm_HangHoaThiTruong>> GetAllParentCategoriesAsync();
        Task<Dm_HangHoaThiTruong?> GetWithChildrenAsync(Guid id);
        Task<bool> DeleteWithChildrenAsync(Guid id);
        Task<bool> ExistsByMaInSameLevelAsync(string ma, Guid? parentId, Guid? exceptId = null);
        Task<IEnumerable<Dm_HangHoaThiTruong>> AddRangeWithValidationAsync(IEnumerable<Dm_HangHoaThiTruong> entities);
        Task<PagedList<Dm_HangHoaThiTruong>> GetChildrenByParentIdPagedAsync(Guid parentId, PaginationParams paginationParams);
        Task<PagedList<Dm_HangHoaThiTruong>> SearchAllPagedAsync(
            string searchTerm,
            PaginationParams paginationParams,
            params Expression<Func<Dm_HangHoaThiTruong, string>>[] searchFields);
        Task<List<Dm_HangHoaThiTruong>> GetRootItemsForSearchAsync(HashSet<Guid> parentIds, List<Guid> matchingItemIds);
        Task<List<(Dm_HangHoaThiTruong Category, bool HasChildren)>> GetAllCategoriesWithChildInfoAsync();
        Task<Dm_HangHoaThiTruong> GetByIdWithRelationsAsync(Guid id);
        Task<List<string>> GetExistingCodesInSameLevelAsync(List<string> codes, Guid? parentId);
        Task<List<Dm_HangHoaThiTruong>> GetHierarchicalPathAsync(Guid itemId);
    }
}
