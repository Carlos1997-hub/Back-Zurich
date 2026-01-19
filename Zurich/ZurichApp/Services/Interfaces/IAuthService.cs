using ZurichApp.Api.Dtos.Auth;

namespace ZurichApp.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
