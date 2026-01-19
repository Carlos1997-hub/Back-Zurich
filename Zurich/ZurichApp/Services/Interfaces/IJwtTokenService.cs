using ZurichApp.Api.Models.Auth;

namespace ZurichApp.Api.Services.Interfaces
{
    public interface IJwtTokenService
    {
        (string Token, int ExpiresInSeconds) CreateToken(User user, string roleName);
    }
}
