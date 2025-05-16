using Core.Entities.Domain.DanhMuc;
using Core.Interfaces.IGeneric;
using Core.Helpers;
namespace Core.Interfaces.IRepository.IDanhMuc
{
    public interface INhomHangHoaRepository : IGenericRepository<Dm_NhomHangHoa>
    {
        // Kiểm tra trùng mã nhóm trong cùng một nhóm cha
        Task<bool> IsMaNhomExistsInSameParentAsync(string maNhom, Guid? parentId, Guid? excludeId = null);
        
        // Lấy tất cả nhóm con (trực tiếp) của một nhóm
        Task<List<Dm_NhomHangHoa>> GetChildGroupsAsync(Guid parentId);
        
        // Lấy tất cả nhóm con (đệ quy) của một nhóm
        Task<List<Dm_NhomHangHoa>> GetAllDescendantsAsync(Guid parentId);
        
        // Lấy tất cả hàng hóa của một nhóm và các nhóm con
        Task<PagedList<Dm_HangHoa>> GetAllProductsInGroupAsync(Guid groupId, PaginationParams paginationParams);
        Task<List<Dm_NhomHangHoa>> GetRootNodesAsync();
    }
}
