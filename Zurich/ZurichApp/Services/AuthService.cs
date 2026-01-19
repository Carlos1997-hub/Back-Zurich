using ZurichApp.Api.Dtos.Auth;
using ZurichApp.Api.Repositories.Interfaces;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenService _jwt;

        public AuthService(IAuthRepository repo, IPasswordHasher hasher, IJwtTokenService jwt)
        {
            _repo = repo;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var (user, roleName) = await _repo.GetByUsernameOrEmailAsync(request.UsernameOrEmail);

            if (user == null || string.IsNullOrWhiteSpace(roleName) || !user.IsActive)
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            if (!_hasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            var (token, expiresIn) = _jwt.CreateToken(user, roleName);

            return new LoginResponse
            {
                AccessToken = token,
                ExpiresIn = expiresIn,
                UserId = user.UserId,
                Role = roleName,
                ClientId = user.ClientId,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Username = user.Username
            };
        }
    }
}
