namespace ZurichApp.Api.Dtos.Auth
{
    public class RegisterUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int? ClientId { get; set; }
        public string? DisplayName { get; set; }
    }
}