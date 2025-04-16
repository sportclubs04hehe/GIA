using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    /// <summary>
    /// Triển khai cơ bản của mô hình Specification để truy vấn dữ liệu.
    /// </summary>
    /// <typeparam name="T">Kiểu thực thể mà điều kiện truy vấn áp dụng.</typeparam>
    public class BaseSpecification<T> : ISpecification<T>
    {
        /// <summary>
        /// Khởi tạo một specification không có điều kiện lọc.
        /// </summary>
        public BaseSpecification()
        {
        }

        /// <summary>
        /// Khởi tạo một specification với điều kiện lọc cụ thể.
        /// </summary>
        /// <param name="criteria">Biểu thức điều kiện để lọc dữ liệu.</param>
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
        }

        /// <summary>
        /// Biểu thức điều kiện lọc chính.
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// Danh sách các thực thể liên quan cần được include khi truy vấn.
        /// </summary>
        public List<Expression<Func<T, object>>> Includes { get; } =
            new List<Expression<Func<T, object>>>();

        /// <summary>
        /// Biểu thức sắp xếp tăng dần.
        /// </summary>
        public Expression<Func<T, object>> OrderBy { get; private set; }

        /// <summary>
        /// Biểu thức sắp xếp giảm dần.
        /// </summary>
        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        /// <summary>
        /// Số lượng bản ghi tối đa sẽ lấy.
        /// </summary>
        public int Take { get; private set; }

        /// <summary>
        /// Số lượng bản ghi sẽ bỏ qua.
        /// </summary>
        public int Skip { get; private set; }

        /// <summary>
        /// Cờ cho biết có đang áp dụng phân trang hay không.
        /// </summary>
        public bool IsPagingEnabled { get; private set; }

        /// <summary>
        /// Thêm biểu thức include để lấy thực thể liên quan.
        /// </summary>
        /// <param name="includeExpression">Biểu thức xác định thực thể liên quan cần include.</param>
        /// <returns>Trả về chính instance hiện tại để có thể chaining phương thức.</returns>
        protected BaseSpecification<T> AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression ?? throw new ArgumentNullException(nameof(includeExpression)));
            return this;
        }

        /// <summary>
        /// Thêm biểu thức sắp xếp tăng dần.
        /// </summary>
        /// <param name="orderByExpression">Biểu thức sắp xếp theo thuộc tính nào đó.</param>
        /// <returns>Trả về chính instance hiện tại để có thể chaining phương thức.</returns>
        protected BaseSpecification<T> AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression ?? throw new ArgumentNullException(nameof(orderByExpression));
            return this;
        }

        /// <summary>
        /// Thêm biểu thức sắp xếp giảm dần.
        /// </summary>
        /// <param name="orderByDescExpression">Biểu thức sắp xếp theo thuộc tính nào đó.</param>
        /// <returns>Trả về chính instance hiện tại để có thể chaining phương thức.</returns>
        protected BaseSpecification<T> AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression ?? throw new ArgumentNullException(nameof(orderByDescExpression));
            return this;
        }

        /// <summary>
        /// Áp dụng phân trang cho truy vấn.
        /// </summary>
        /// <param name="skip">Số lượng bản ghi cần bỏ qua.</param>
        /// <param name="take">Số lượng bản ghi tối đa cần lấy.</param>
        /// <returns>Trả về chính instance hiện tại để có thể chaining phương thức.</returns>
        /// <exception cref="ArgumentException">Ném lỗi nếu skip < 0 hoặc take <= 0.</exception>
        protected BaseSpecification<T> ApplyPaging(int skip, int take)
        {
            if (skip < 0)
                throw new ArgumentException("Skip value cannot be negative", nameof(skip));

            if (take <= 0)
                throw new ArgumentException("Take value must be greater than zero", nameof(take));

            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
            return this;
        }
    }

}
