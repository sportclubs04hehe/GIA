using Core.Entities.IdentityManagament;
using Core.ServiceInterface.ISSO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.ServiceImplement.SSO
{
    /// <summary>
    /// Lớp TokenService chịu trách nhiệm tạo ra JSON Web Token (JWT) cho việc xác thực người dùng.
    /// Implement giao diện ITokenService.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly ILogger<TokenService> _logger;

        /// <summary>
        /// Constructor khởi tạo TokenService với cấu hình và logger.
        /// Thiết lập khóa bảo mật đối xứng dùng để ký JWT.
        /// </summary>
        public TokenService(IConfiguration config, ILogger<TokenService> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Tạo khóa bảo mật đối xứng từ khóa cấu hình trong file cấu hình.
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"] ??
                throw new InvalidOperationException("Thiếu cấu hình Token:Key")));
        }

        /// <summary>
        /// Tạo JWT cho người dùng đã cho với các claim và thiết lập thời gian hết hạn.
        /// </summary>
        /// <param name="user">Đối tượng người dùng để tạo token.</param>
        /// <returns>Chuỗi đại diện cho token JWT đã tạo.</returns>
        public string CreateToken(AppUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                // Tạo các claim chứa thông tin về người dùng.
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.DisplayName ?? string.Empty)
            };

                // Tạo đối tượng SigningCredentials để ký token bằng khóa bảo mật và thuật toán HMACSHA512.
                var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

                // Kiểm tra và lấy giá trị ngày hết hạn từ cấu hình, mặc định là 7 ngày.
                if (!int.TryParse(_config["Token:ExpirationDays"], out int expirationDays))
                {
                    expirationDays = 7;
                }

                // Khởi tạo SecurityTokenDescriptor với các thông tin như claim, thời gian hết hạn, và nhà phát hành.
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(expirationDays),
                    SigningCredentials = creds,
                    Issuer = _config["Token:Issuer"] ??
                        throw new InvalidOperationException("Thiếu cấu hình Token:Issuer")
                };

                // Sử dụng JwtSecurityTokenHandler để tạo token JWT từ tokenDescriptor.
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Log thông tin người dùng và việc tạo token thành công.
                _logger.LogInformation("Token đã được tạo thành công cho người dùng: {Email}", user.Email);

                // Trả về token dưới dạng chuỗi.
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có vấn đề khi tạo token.
                _logger.LogError(ex, "Lỗi khi tạo token cho người dùng: {Email}", user.Email);
                throw;
            }
        }

        /// <summary>
        /// Tạo một refresh token ngẫu nhiên và bảo mật
        /// </summary>
        /// <returns>Chuỗi base64 đại diện cho refresh token</returns>
        public string GenerateRefreshToken()
        {
            try
            {
                // Tạo mảng byte ngẫu nhiên bảo mật (64 bytes = 512 bits)
                var randomBytes = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);
                
                // Chuyển đổi mảng byte sang chuỗi base64 để lưu trữ/truyền tải
                var refreshToken = Convert.ToBase64String(randomBytes);
                _logger.LogInformation("Đã tạo refresh token mới");
                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo refresh token");
                throw;
            }
        }

        /// <summary>
        /// Xác thực và phân tích cú pháp của JWT token
        /// </summary>
        /// <param name="token">Chuỗi JWT token cần xác thực</param>
        /// <returns>ClaimsPrincipal nếu token hợp lệ, null nếu không hợp lệ</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token trống hoặc null được cung cấp cho phương thức ValidateToken");
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = true,
                    ValidIssuer = _config["Token:Issuer"],
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Để kiểm tra thời gian chính xác
                };

                // Xác thực và phân tích cú pháp token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
                
                // Kiểm tra bổ sung để đảm bảo token là JwtSecurityToken với thuật toán đúng
                if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogWarning("Token không hợp lệ hoặc sử dụng thuật toán không đúng");
                    return null;
                }

                _logger.LogInformation("Token đã được xác thực thành công");
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác thực token");
                return null;
            }
        }
    }
}
