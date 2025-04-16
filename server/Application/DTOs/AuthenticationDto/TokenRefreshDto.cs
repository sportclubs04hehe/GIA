namespace Application.DTOs.AuthenticationDto
{
    /// <summary>
    /// DTO cho kết quả làm mới token
    /// </summary>
    public class TokenRefreshDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
