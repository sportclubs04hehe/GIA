using Core.Entities.Domain;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using Core.Specifications;

namespace Core.Interfaces.IRepository
{
    /// <summary>
    /// Repository interface for NhomHangHoa (Product Category) operations
    /// </summary>
    public interface INhomHangHoaRepository : IRepository<NhomHangHoa>
    {
        // Additional methods not in generic repository
        Task<NhomHangHoa?> GetByMaNhomAsync(string maNhom);
        Task<bool> ExistsByMaNhomAsync(string maNhom);
        
        // Hierarchy-specific operations
        Task<IReadOnlyList<NhomHangHoa>> GetRootGroupsAsync();
        Task<IReadOnlyList<NhomHangHoa>> GetChildGroupsAsync(Guid parentId);
        Task<NhomHangHoa> GetWithChildrenAsync(Guid id, int levels = 1);
        Task<NhomHangHoa> GetWithProductsAsync(Guid id);
        
        // Advanced operations using specifications
        Task<NhomHangHoa> GetSingleBySpecAsync(ISpecification<NhomHangHoa> spec);
        Task<IReadOnlyList<NhomHangHoa>> GetListBySpecAsync(ISpecification<NhomHangHoa> spec);
        Task<PagedList<NhomHangHoa>> GetPagedBySpecAsync(ISpecification<NhomHangHoa> spec, PaginationParams paginationParams);
        Task<int> CountAsync(ISpecification<NhomHangHoa> spec);
        
        // Additional filtering methods
        Task<PagedList<NhomHangHoa>> GetFilteredAsync(SpecificationParams specParams);
    }
}
