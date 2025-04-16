namespace Core.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Lớp đại diện cho một danh sách có phân trang.
    /// Dùng để chia dữ liệu thành nhiều trang nhỏ khi trả kết quả từ API.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của phần tử trong danh sách.</typeparam>
    public class PagedList<T> : IReadOnlyList<T>
    {
        // Danh sách các phần tử trong trang hiện tại
        private readonly List<T> _items;

        /// <summary>
        /// Khởi tạo đối tượng PagedList mới.
        /// </summary>
        /// <param name="items">Danh sách phần tử trong trang hiện tại.</param>
        /// <param name="count">Tổng số phần tử (không phân trang).</param>
        /// <param name="pageNumber">Số trang hiện tại (bắt đầu từ 1).</param>
        /// <param name="pageSize">Số phần tử trên mỗi trang.</param>
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            // Kiểm tra dữ liệu đầu vào hợp lệ
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalCount = count;

            // Tính tổng số trang bằng cách làm tròn lên
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Lưu lại danh sách item của trang hiện tại
            _items = items.ToList();
        }

        /// <summary>Số trang hiện tại.</summary>
        public int CurrentPage { get; }

        /// <summary>Tổng số trang có thể có.</summary>
        public int TotalPages { get; }

        /// <summary>Số phần tử mỗi trang.</summary>
        public int PageSize { get; }

        /// <summary>Tổng số phần tử (không phân trang).</summary>
        public int TotalCount { get; }

        /// <summary>Truy cập phần tử theo chỉ số.</summary>
        public T this[int index] => _items[index];

        /// <summary>Số lượng phần tử trong trang hiện tại.</summary>
        public int Count => _items.Count;

        /// <summary>Duyệt các phần tử trong danh sách.</summary>
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        /// <summary>
        /// Tạo một đối tượng PagedList từ một IQueryable, xử lý bất đồng bộ.
        /// </summary>
        /// <param name="source">Nguồn dữ liệu (thường là DbSet trong Entity Framework).</param>
        /// <param name="pageNumber">Trang cần lấy.</param>
        /// <param name="pageSize">Số phần tử mỗi trang.</param>
        /// <returns>Trả về danh sách có phân trang.</returns>
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync(); // Tổng số phần tử
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); // Lấy phần tử tương ứng
            return new PagedList<T>(items, count, pageNumber, pageSize); // Tạo đối tượng PagedList
        }

        /// <summary>Kiểm tra có trang trước không.</summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>Kiểm tra có trang sau không.</summary>
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
