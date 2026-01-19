namespace ZurichApp.Api.Dtos.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public string? DisplayName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
