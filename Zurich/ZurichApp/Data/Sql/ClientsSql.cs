namespace ZurichApp.Api.Data.Sql
{
    public static class ClientsSql
    {
        public const string GetAll = @"SELECT ClientId, IdentificationNumber, FullName, Email, Phone, Address, IsDeleted, CreatedAt, UpdatedAt
            FROM dbo.Clients
            WHERE IsDeleted = 0
            ORDER BY ClientId ASC;";

        public const string GetById = @"SELECT ClientId, IdentificationNumber, FullName, Email, Phone, Address, IsDeleted, CreatedAt, UpdatedAt
            FROM dbo.Clients
            WHERE ClientId = @ClientId AND IsDeleted = 0;";

        public const string Insert = @"INSERT INTO dbo.Clients
            (IdentificationNumber, FullName, Email, Phone, Address)
            VALUES (@IdentificationNumber, @FullName, @Email, @Phone, @Address);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        public const string Update = @"UPDATE dbo.Clients
            SET FullName = @FullName,
            Email = @Email,
            Phone = @Phone,
            Address = @Address,
            UpdatedAt = SYSUTCDATETIME()
            WHERE ClientId = @ClientId AND IsDeleted = 0;";

        public const string SoftDelete = @"UPDATE dbo.Clients
            SET IsDeleted = 1,
            UpdatedAt = SYSUTCDATETIME()
            WHERE ClientId = @ClientId AND IsDeleted = 0;

            UPDATE dbo.Users SET
            IsActive = 0,
            UpdatedAt = SYSUTCDATETIME()
            WHERE ClientId = @ClientId;";
    }
}
