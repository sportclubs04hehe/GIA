using Core.Entities.IdentityManagament;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace server.Extensions
{
    /// <summary>
    /// Các phương thức mở rộng cho UserManager<AppUser> để tìm người dùng theo ClaimsPrincipal
    /// </summary>
    public static class UserManagerExtensions
    {
        /// <summary>
        /// Tìm người dùng theo ClaimsPrincipal và kèm theo thông tin địa chỉ
        /// </summary>
        /// <param name="userManager">Đối tượng UserManager để thao tác với người dùng</param>
        /// <param name="user">ClaimsPrincipal chứa thông tin yêu cầu người dùng</param>
        /// <returns>AppUser có thông tin địa chỉ nếu tìm thấy, null nếu không tìm thấy</returns>
        public static async Task<AppUser?> FindUserByClaimsPrincipalWithAddress(this UserManager<AppUser> userManager,
           ClaimsPrincipal user)
        {
            var email = GetEmailFromClaimsPrincipal(user);
            if (string.IsNullOrEmpty(email)) return null;

            // Tìm người dùng có email trùng với ClaimsPrincipal
            return await userManager.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        /// <summary>
        /// Tìm người dùng dựa trên email trong ClaimsPrincipal
        /// </summary>
        /// <param name="userManager">Đối tượng UserManager để thao tác với người dùng</param>
        /// <param name="user">ClaimsPrincipal chứa thông tin yêu cầu người dùng</param>
        /// <returns>AppUser nếu tìm thấy, null nếu không tìm thấy</returns>
        public static async Task<AppUser?> FindByEmailFromClaimsPrincipal(this UserManager<AppUser> userManager,
            ClaimsPrincipal user)
        {
            var email = GetEmailFromClaimsPrincipal(user);
            if (string.IsNullOrEmpty(email)) return null;

            // Tìm người dùng có email trùng với ClaimsPrincipal
            return await userManager.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        /// <summary>
        /// Phương thức hỗ trợ để lấy email từ ClaimsPrincipal
        /// </summary>
        /// <param name="user">ClaimsPrincipal chứa thông tin yêu cầu người dùng</param>
        /// <returns>Chuỗi email nếu tìm thấy, null nếu không có email</returns>
        private static string? GetEmailFromClaimsPrincipal(ClaimsPrincipal user)
        {
            return user?.FindFirstValue(ClaimTypes.Email);
        }
    }

}
