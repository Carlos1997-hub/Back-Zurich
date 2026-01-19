using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;

        public CurrentUserService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public int? GetUserId()
        {
            var sub = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? _http.HttpContext?.User?.FindFirstValue("sub")
                      ?? _http.HttpContext?.User?.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

            return int.TryParse(sub, out var id) ? id : null;
        }

        public int? GetClientId()
        {
            var v = _http.HttpContext?.User?.FindFirstValue("clientId");
            return int.TryParse(v, out var id) ? id : null;
        }

        public string? GetRole()
        {
            return _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)
                   ?? _http.HttpContext?.User?.FindFirstValue("role");
        }

        public bool IsAdmin()
        {
            var role = GetRole();
            return string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase);
        }
    }
}
