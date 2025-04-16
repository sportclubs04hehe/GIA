using System.Security.Claims;

namespace server.Extensions
{
    using System.Security.Claims;

    namespace YourNamespace.Extensions
    {
        /// <summary>
        /// Lớp mở rộng cho ClaimsPrincipal giúp truy xuất thông tin người dùng từ JWT token hoặc các loại claims.
        /// </summary>
        public static class ClaimsPrincipalExtensions
        {
            /// <summary>
            /// Lấy địa chỉ email của người dùng từ claims.
            /// </summary>
            /// <param name="user">Đối tượng ClaimsPrincipal đại diện cho người dùng hiện tại.</param>
            /// <returns>Email nếu tồn tại, ngược lại trả về chuỗi rỗng.</returns>
            public static string RetrieveEmailFromPrincipal(this ClaimsPrincipal user)
            {
                if (user == null) return string.Empty;
                return user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            }

            /// <summary>
            /// Lấy mã định danh (ID) của người dùng từ claims.
            /// </summary>
            /// <param name="user">Đối tượng ClaimsPrincipal đại diện cho người dùng hiện tại.</param>
            /// <returns>User ID nếu tồn tại, ngược lại trả về chuỗi rỗng.</returns>
            public static string GetUserId(this ClaimsPrincipal user)
            {
                if (user == null) return string.Empty;
                return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            }

            /// <summary>
            /// Lấy tên người dùng (username) từ claims.
            /// </summary>
            /// <param name="user">Đối tượng ClaimsPrincipal đại diện cho người dùng hiện tại.</param>
            /// <returns>Username nếu tồn tại, ngược lại trả về chuỗi rỗng.</returns>
            public static string GetUserName(this ClaimsPrincipal user)
            {
                if (user == null) return string.Empty;
                return user.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            }
        }
    }

}
