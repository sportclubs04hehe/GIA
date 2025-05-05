using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using Core.Specifications;

namespace Core.Interfaces.IRepository.IDanhMuc
{
    /// <summary>
    /// Repository interface for NhomHangHoa (Product Category) operations
    /// </summary>
    public interface INhomHangHoaRepository : IGenericRepository<Dm_NhomHangHoa>
    {
        // Additional methods not in generic repository
        Task<Dm_NhomHangHoa?> GetByMaNhomAsync(string maNhom);
        Task<bool> ExistsByMaNhomAsync(string maNhom);
        
        // Hierarchy-specific operations
        Task<IReadOnlyList<Dm_NhomHangHoa>> GetRootGroupsAsync();
        Task<IReadOnlyList<Dm_NhomHangHoa>> GetChildGroupsAsync(Guid parentId);
        Task<Dm_NhomHangHoa> GetWithChildrenAsync(Guid id, int levels = 1);
        Task<Dm_NhomHangHoa> GetWithProductsAsync(Guid id);
        
        // Advanced operations using specifications
        Task<Dm_NhomHangHoa> GetSingleBySpecAsync(ISpecification<Dm_NhomHangHoa> spec);
        Task<IReadOnlyList<Dm_NhomHangHoa>> GetListBySpecAsync(ISpecification<Dm_NhomHangHoa> spec);
        Task<PagedList<Dm_NhomHangHoa>> GetPagedBySpecAsync(ISpecification<Dm_NhomHangHoa> spec, PaginationParams paginationParams);
        Task<int> CountAsync(ISpecification<Dm_NhomHangHoa> spec);
        
        // Additional filtering methods
        Task<PagedList<Dm_NhomHangHoa>> GetFilteredAsync(SpecificationParams specParams);
    }
}
