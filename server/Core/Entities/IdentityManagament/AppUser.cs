using Microsoft.AspNetCore.Identity;

namespace Core.Entities.IdentityManagament
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
