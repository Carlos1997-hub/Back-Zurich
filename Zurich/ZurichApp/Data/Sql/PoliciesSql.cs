namespace ZurichApp.Api.Data.Sql
{
    public static class PoliciesSql
    {
        public const string Insert = @"INSERT INTO dbo.Policies
        (
            ClientId, CreatedByUserId,
            PolicyType, StartDate, ExpirationDate, InsuredAmount,
            PolicyStatus
        )
        VALUES
        (
            @ClientId, @CreatedByUserId,
            @PolicyType, @StartDate, @ExpirationDate, @InsuredAmount,
            @PolicyStatus
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        public const string GetAllWithClient = @"SELECT
            p.PolicyId, p.ClientId, p.CreatedByUserId, p.PolicyType, p.StartDate, p.ExpirationDate,
            p.InsuredAmount, p.PolicyStatus, p.AssignedAt, p.CancelledAt, p.CreatedAt, p.UpdatedAt,
            c.FullName AS ClientFullName, c.Email AS ClientEmail
            FROM dbo.Policies p
            INNER JOIN dbo.Clients c ON c.ClientId = p.ClientId
            WHERE c.IsDeleted = 0
            ORDER BY p.PolicyId ASC;";

        public const string GetMineWithClient = @"SELECT
            p.PolicyId, p.ClientId, p.CreatedByUserId, p.PolicyType, p.StartDate, p.ExpirationDate,
            p.InsuredAmount, p.PolicyStatus, p.AssignedAt, p.CancelledAt, p.CreatedAt, p.UpdatedAt,
            c.FullName AS ClientFullName, c.Email AS ClientEmail
            FROM dbo.Policies p
            INNER JOIN dbo.Clients c ON c.ClientId = p.ClientId
            WHERE p.ClientId = @ClientId AND c.IsDeleted = 0
            ORDER BY p.PolicyId ASC;";

        public const string GetByClientIdWithClient = @"SELECT
            p.PolicyId, p.ClientId, p.CreatedByUserId, p.PolicyType, p.StartDate, p.ExpirationDate,
            p.InsuredAmount, p.PolicyStatus, p.AssignedAt, p.CancelledAt, p.CreatedAt, p.UpdatedAt,
            c.FullName AS ClientFullName, c.Email AS ClientEmail
            FROM dbo.Policies p
            INNER JOIN dbo.Clients c ON c.ClientId = p.ClientId
            WHERE p.ClientId = @ClientId AND c.IsDeleted = 0
            ORDER BY p.PolicyId ASC;";

        public const string Cancel = @"UPDATE dbo.Policies SET
        PolicyStatus = 'Cancelada',
        CancelledAt = SYSUTCDATETIME(),
        UpdatedAt = SYSUTCDATETIME()
        WHERE PolicyId = @PolicyId
        AND PolicyStatus = 'Activa';";


    }
}
