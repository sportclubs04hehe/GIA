using Core.Helpers;
using System.Text.Json;

namespace server.Helpers
{
    /// <summary>
    /// Các phương thức mở rộng cho HttpResponse để thêm thông tin phân trang và các header phổ biến.
    /// </summary>
    public static class HttpExtensions
    {
        // Tên header chứa thông tin phân trang
        private const string PaginationHeaderName = "Pagination";

        // Tên header để cho phép các header tùy chỉnh được hiển thị phía client
        private const string AccessControlExposeHeadersName = "Access-Control-Expose-Headers";

        /// <summary>
        /// Thêm metadata phân trang vào header của phản hồi HTTP, sử dụng dữ liệu từ PagedList.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của danh sách phân trang.</typeparam>
        /// <param name="response">Phản hồi HTTP cần thêm header.</param>
        /// <param name="data">Dữ liệu phân trang chứa thông tin metadata.</param>
        /// <exception cref="ArgumentNullException">Ném ra nếu response hoặc data bị null.</exception>
        public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            AddPaginationHeader(response, data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
        }

        /// <summary>
        /// Thêm metadata phân trang vào header của phản hồi HTTP bằng cách truyền giá trị trực tiếp.
        /// </summary>
        /// <param name="response">Phản hồi HTTP cần thêm header.</param>
        /// <param name="currentPage">Trang hiện tại.</param>
        /// <param name="pageSize">Kích thước trang.</param>
        /// <param name="totalCount">Tổng số phần tử.</param>
        /// <param name="totalPages">Tổng số trang.</param>
        /// <exception cref="ArgumentNullException">Ném ra nếu response bị null.</exception>
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int pageSize,
            int totalCount, int totalPages)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var paginationHeader = new PaginationHeader(currentPage, pageSize, totalCount, totalPages);

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            try
            {
                response.Headers.Append(PaginationHeaderName, JsonSerializer.Serialize(paginationHeader, jsonOptions));
                EnsureHeaderIsExposed(response, PaginationHeaderName);
            }
            catch (Exception ex)
            {
                // Nếu có logger thì ghi log, hiện tại thì ném lại exception
                throw new InvalidOperationException("Failed to serialize pagination header", ex);
            }
        }

        /// <summary>
        /// Đảm bảo header chỉ định được đưa vào danh sách Access-Control-Expose-Headers
        /// để client có thể truy cập được từ phía trình duyệt (CORS).
        /// </summary>
        /// <param name="response">Phản hồi HTTP.</param>
        /// <param name="headerName">Tên của header cần expose ra.</param>
        private static void EnsureHeaderIsExposed(HttpResponse response, string headerName)
        {
            if (!response.Headers.TryGetValue(AccessControlExposeHeadersName, out var exposeHeaders))
            {
                response.Headers.Append(AccessControlExposeHeadersName, headerName);
                return;
            }

            if (!exposeHeaders.ToString().Contains(headerName))
            {
                response.Headers.Remove(AccessControlExposeHeadersName);
                response.Headers.Append(AccessControlExposeHeadersName,
                    $"{exposeHeaders}, {headerName}");
            }
        }
    }
}
