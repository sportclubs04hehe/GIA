using Core.Entities.Domain.DanhMuc;
using Core.Helpers;
using Core.Interfaces.IGeneric;
using System.Linq.Expressions;

namespace Core.Interfaces.IRepository.IDanhMuc
{
    public interface IDm_ThuocTinhRepository: IGenericRepository<Dm_ThuocTinh>
    {
        // Lấy tất cả các thuộc tính cha (không có cha)
        Task<List<Dm_ThuocTinh>> GetAllParentCategoriesAsync();
        
        // Lấy thuộc tính kèm các thuộc tính con
        Task<Dm_ThuocTinh?> GetWithChildrenAsync(Guid id);
        
        // Xóa thuộc tính và tất cả thuộc tính con
        Task<bool> DeleteWithChildrenAsync(Guid id);
        
        // Kiểm tra mã đã tồn tại ở cùng cấp không
        Task<bool> ExistsByMaInSameLevelAsync(string ma, Guid? parentId, Guid? exceptId = null);
        
        // Thêm nhiều thuộc tính với xác thực mã trùng lặp
        Task<IEnumerable<Dm_ThuocTinh>> AddRangeWithValidationAsync(IEnumerable<Dm_ThuocTinh> entities);
        
        // Lấy danh sách con phân trang theo ID cha
        Task<PagedList<Dm_ThuocTinh>> GetChildrenByParentIdPagedAsync(Guid parentId, PaginationParams paginationParams);
        
        // Tìm kiếm tất cả với phân trang
        Task<PagedList<Dm_ThuocTinh>> SearchAllPagedAsync(
            string searchTerm,
            PaginationParams paginationParams,
            params Expression<Func<Dm_ThuocTinh, string>>[] searchFields);
        
        // Lấy các node gốc cho tìm kiếm
        Task<List<Dm_ThuocTinh>> GetRootItemsForSearchAsync(HashSet<Guid> parentIds, List<Guid> matchingItemIds);
        
        // Lấy tất cả thuộc tính với thông tin có con hay không
        Task<List<(Dm_ThuocTinh Category, bool HasChildren)>> GetAllCategoriesWithChildInfoAsync();
        
        // Lấy thuộc tính theo ID với các quan hệ
        Task<Dm_ThuocTinh> GetByIdWithRelationsAsync(Guid id);
        
        // Lấy đường dẫn từ node tới gốc
        Task<List<Guid>> GetPathToRootAsync(Guid nodeId);
        
        // Lấy các node gốc với các node con theo đường dẫn
        Task<List<Dm_ThuocTinh>> GetRootNodesWithRequiredChildrenAsync(List<Guid> pathIds, Guid? newItemId = null);
        
        // Lấy danh sách mã đã tồn tại ở cùng cấp
        Task<List<string>> GetExistingCodesInSameLevelAsync(List<string> codes, Guid? parentId);
    }
}
