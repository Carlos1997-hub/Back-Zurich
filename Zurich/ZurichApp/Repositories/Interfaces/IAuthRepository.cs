using ZurichApp.Api.Models.Auth;

namespace ZurichApp.Api.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<(User? User, string? RoleName)> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<int> CreateUserAsync(User user);
        Task UpdatePasswordAsync(int userId, byte[] passwordHash, byte[] passwordSalt);
    }
}
