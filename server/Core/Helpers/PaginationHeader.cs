using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    /// <summary>
    /// Đại diện cho thông tin phân trang được đính kèm trong header của phản hồi HTTP.
    /// </summary>
    /// <param name="currentPage">Trang hiện tại.</param>
    /// <param name="itemsPerPage">Số phần tử mỗi trang.</param>
    /// <param name="totalItems">Tổng số phần tử (không phân trang).</param>
    /// <param name="totalPages">Tổng số trang.</param>
    public class PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
        /// <summary>
        /// Số trang hiện tại.
        /// </summary>
        public int CurrentPage { get; } = currentPage;

        /// <summary>
        /// Số phần tử trên mỗi trang.
        /// </summary>
        public int ItemsPerPage { get; } = itemsPerPage;

        /// <summary>
        /// Tổng số phần tử của tất cả trang.
        /// </summary>
        public int TotalItems { get; } = totalItems;

        /// <summary>
        /// Tổng số trang.
        /// </summary>
        public int TotalPages { get; } = totalPages;

        /// <summary>
        /// Cho biết có tồn tại trang trước không.
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Cho biết có tồn tại trang sau không.
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;
    }

}
