using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces.IBase
{
    /// <summary>
    /// Giao diện repository tổng quát cho các thao tác CRUD trên thực thể.
    /// </summary>
    /// <typeparam name="T">Kiểu thực thể kế thừa từ BaseEntity.</typeparam>
    public interface IGenericRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Lấy một thực thể theo ID.
        /// </summary>
        /// <param name="id">ID của thực thể.</param>
        /// <returns>Thực thể có ID tương ứng.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Lấy toàn bộ danh sách thực thể.
        /// </summary>
        /// <returns>Danh sách chỉ đọc các thực thể.</returns>
        Task<IReadOnlyList<T>> ListAllAsync();

        /// <summary>
        /// Lấy một thực thể theo điều kiện từ specification.
        /// </summary>
        /// <param name="spec">Specification áp dụng để lọc dữ liệu.</param>
        /// <returns>Thực thể phù hợp với điều kiện.</returns>
        Task<T> GetEntityWithSpecAsync(ISpecification<T> spec);

        /// <summary>
        /// Lấy danh sách thực thể theo điều kiện từ specification.
        /// </summary>
        /// <param name="spec">Specification áp dụng để lọc dữ liệu.</param>
        /// <returns>Danh sách chỉ đọc các thực thể phù hợp.</returns>
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

        /// <summary>
        /// Đếm số lượng thực thể thỏa mãn điều kiện từ specification.
        /// </summary>
        /// <param name="spec">Specification áp dụng để lọc dữ liệu.</param>
        /// <returns>Số lượng thực thể phù hợp.</returns>
        Task<int> CountAsync(ISpecification<T> spec);

        /// <summary>
        /// Thêm một thực thể mới.
        /// </summary>
        /// <param name="entity">Thực thể cần thêm.</param>
        void Add(T entity);

        /// <summary>
        /// Cập nhật một thực thể đã tồn tại.
        /// </summary>
        /// <param name="entity">Thực thể cần cập nhật.</param>
        void Update(T entity);

        /// <summary>
        /// Xóa một thực thể.
        /// </summary>
        /// <param name="entity">Thực thể cần xóa.</param>
        void Delete(T entity);

        /// <summary>
        /// Thêm nhiều thực thể cùng lúc.
        /// </summary>
        /// <param name="entities">Danh sách thực thể cần thêm.</param>
        void AddRange(IEnumerable<T> entities);

        /// <summary>
        /// Kiểm tra xem có thực thể nào thỏa mãn điều kiện hay không.
        /// </summary>
        /// <param name="spec">Specification áp dụng để lọc dữ liệu.</param>
        /// <returns>True nếu có thực thể phù hợp, ngược lại là false.</returns>
        Task<bool> AnyAsync(ISpecification<T> spec);
    }

}
