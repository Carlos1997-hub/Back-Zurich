namespace ZurichApp.Api.Data.Sql
{
    public static class AuthSql
    {
        public const string GetUserByUsernameOrEmail = @"SELECT
            u.UserId, u.Username, u.Email, u.PasswordHash, u.PasswordSalt,
            u.RoleId, u.ClientId, u.DisplayName, u.IsActive, u.CreatedAt, u.UpdatedAt,r.RoleName
            FROM dbo.Users u INNER JOIN dbo.Roles r ON r.RoleId = u.RoleId
            WHERE (u.Username = @UsernameOrEmail OR u.Email = @UsernameOrEmail) AND u.IsActive = 1;";

        public const string GetRoleIdByName = @"SELECT RoleId
        FROM dbo.Roles
        WHERE RoleName = @RoleName;";

        public const string InsertUser = @"INSERT INTO dbo.Users(
            Username, Email, PasswordHash, PasswordSalt,
            RoleId, ClientId, DisplayName, IsActive
        )
            VALUES
        (
            @Username, @Email, @PasswordHash, @PasswordSalt,
            @RoleId, @ClientId, @DisplayName, @IsActive
        );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        public const string UpdatePassword = @"UPDATE dbo.Users
            SET PasswordHash = @PasswordHash,
            PasswordSalt = @PasswordSalt,
            UpdatedAt = SYSUTCDATETIME()
            WHERE UserId = @UserId;";

        public const string GetRoleNameByRoleId = @"
            SELECT RoleName
            FROM dbo.Roles
            WHERE RoleId = @RoleId;";
    }
}
