namespace Application.DTOs.AuthenticationDto
{
    /// <summary>
    /// DTO cho yêu cầu làm mới token
    /// </summary>
    public class TokenRefreshRequestDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
