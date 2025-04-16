using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    /// <summary>
    /// Định nghĩa giao diện cho mô hình Specification nhằm truy vấn dữ liệu với nhiều tùy chọn lọc và sắp xếp.
    /// </summary>
    /// <typeparam name="T">Kiểu thực thể mà điều kiện truy vấn áp dụng.</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Biểu thức điều kiện lọc chính.
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// Danh sách các thực thể liên quan cần được include khi truy vấn.
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }

        /// <summary>
        /// Biểu thức sắp xếp tăng dần.
        /// </summary>
        Expression<Func<T, object>> OrderBy { get; }

        /// <summary>
        /// Biểu thức sắp xếp giảm dần.
        /// </summary>
        Expression<Func<T, object>> OrderByDescending { get; }

        /// <summary>
        /// Số lượng bản ghi tối đa sẽ lấy.
        /// </summary>
        int Take { get; }

        /// <summary>
        /// Số lượng bản ghi sẽ bỏ qua.
        /// </summary>
        int Skip { get; }

        /// <summary>
        /// Flag cho biết có đang áp dụng phân trang hay không.
        /// </summary>
        bool IsPagingEnabled { get; }
    }

}
