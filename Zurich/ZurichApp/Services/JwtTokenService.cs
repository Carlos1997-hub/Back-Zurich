using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ZurichApp.Api.AppSettings;
using ZurichApp.Api.Models.Auth;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _opt;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _opt = options.Value;
        }

        public (string Token, int ExpiresInSeconds) CreateToken(User user, string roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("role", roleName),
                new Claim("username", user.Username ?? string.Empty),
                new Claim("email", user.Email ?? string.Empty)
            };

            if (user.ClientId.HasValue)
                claims.Add(new Claim("clientId", user.ClientId.Value.ToString()));

            var expires = DateTime.UtcNow.AddMinutes(_opt.ExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            var expiresIn = (int)TimeSpan.FromMinutes(_opt.ExpirationMinutes).TotalSeconds;

            return (tokenStr, expiresIn);
        }
    }
}
