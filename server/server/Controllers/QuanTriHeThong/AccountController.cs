using Core.Entities.IdentityManagament;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.AuthenticationDto;
using server.Errors;
using server.Extensions;
using Core.ServiceInterface.ISSO;

namespace server.Controllers.QuanTriHeThong
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Lấy người dùng hiện đang được xác thực
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailFromClaimsPrincipal(User);

            if (user == null)
                return Unauthorized("User not found");

            return Ok(CreateUserDto(user));
        }

        /// <summary>
        /// Xác thực người dùng
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest(ApiResponse.BadRequest("Email and password are required"));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return Unauthorized("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Invalid email or password");

            return Ok(CreateUserDto(user));
        }

        /// <summary>
        /// Đăng ký người dùng mới
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
            {
                return BadRequest(new ApiValidationErrorResponse { Errors = new[] { "Email address is already in use" } });
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                RefreshToken = _tokenService.GenerateRefreshToken(),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(30)
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) 
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return BadRequest(new ApiValidationErrorResponse { Errors = errors });
            }

            return Ok(CreateUserDto(user));
        }

        /// <summary>
        /// Phương pháp trợ giúp để tạo UserDto từ AppUser
        /// </summary>
        private UserDto CreateUserDto(AppUser user)
        {
            return new UserDto
            {
                Email = user.Email ?? string.Empty,
                Token = _tokenService.CreateToken(user),
                RefreshToken = _tokenService.GenerateRefreshToken(),
                DisplayName = user.DisplayName
            };
        }

        /// <summary>
        /// Kiểm tra xem email có tồn tại trong hệ thống không
        /// </summary>
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
    }
}
