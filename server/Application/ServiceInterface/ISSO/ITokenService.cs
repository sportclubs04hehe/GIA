using Core.Entities.IdentityManagament;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.ServiceInterface.ISSO
{
    public interface ITokenService
    {
        /// <summary>
        /// Creates a JWT token for a user
        /// </summary>
        string CreateToken(AppUser user);
        string GenerateRefreshToken();    // Sinh refresh token
        ClaimsPrincipal? ValidateToken(string token); // Xác thực và parse token
    }
}
