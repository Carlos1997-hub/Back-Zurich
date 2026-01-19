using Dapper;
using ZurichApp.Api.Data;
using ZurichApp.Api.Data.Sql;
using ZurichApp.Api.Models.Auth;
using ZurichApp.Api.Repositories.Interfaces;

namespace ZurichApp.Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DbConnectionFactory _db;

        public AuthRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        private class UserRow : User
        {
            public string RoleName { get; set; } = string.Empty;
        }

        public async Task<(User? User, string? RoleName)> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            using var conn = _db.CreateConnection();
            var row = await conn.QueryFirstOrDefaultAsync<UserRow>(
                AuthSql.GetUserByUsernameOrEmail,
                new { UsernameOrEmail = usernameOrEmail }
            );

            if (row == null) return (null, null);

            User u = row;
            return (u, row.RoleName);
        }

        public async Task<int> CreateUserAsync(User user)
        {
            using var conn = _db.CreateConnection();
            var id = await conn.ExecuteScalarAsync<int>(AuthSql.InsertUser, new
            {
                user.Username,
                user.Email,
                user.PasswordHash,
                user.PasswordSalt,
                user.RoleId,
                user.ClientId,
                user.DisplayName,
                user.IsActive
            });
            return id;
        }

        public async Task UpdatePasswordAsync(int userId, byte[] passwordHash, byte[] passwordSalt)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync(AuthSql.UpdatePassword, new
            {
                UserId = userId,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });
        }
    }
}
