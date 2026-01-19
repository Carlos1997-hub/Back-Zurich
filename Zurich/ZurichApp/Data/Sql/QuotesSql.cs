namespace ZurichApp.Api.Data.Sql
{
    public static class QuotesSql
    {
        public const string Insert = @"INSERT INTO dbo.Quotes
        (
            ClientId, PolicyType, InsuredAmount, TermMonths, MonthlyPremium,
            QuoteStatus, Notes
        )
        VALUES
        (
            @ClientId, @PolicyType, @InsuredAmount, @TermMonths, @MonthlyPremium,
            @QuoteStatus, @Notes
        );
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        public const string GetAll = @"
            SELECT QuoteId, ClientId, PolicyType, InsuredAmount, TermMonths, MonthlyPremium,
            QuoteStatus, Notes, CreatedAt, UpdatedAt
            FROM dbo.Quotes
            ORDER BY QuoteId DESC;";

        public const string GetMine = @"
            SELECT QuoteId, ClientId, PolicyType, InsuredAmount, TermMonths, MonthlyPremium,
            QuoteStatus, Notes, CreatedAt, UpdatedAt
            FROM dbo.Quotes
            WHERE ClientId = @ClientId
            ORDER BY QuoteId DESC;";

        public const string GetByClientId = @"SELECT QuoteId, ClientId, PolicyType, InsuredAmount, TermMonths, MonthlyPremium, QuoteStatus, Notes, CreatedAt
        FROM dbo.Quotes
        WHERE ClientId = @ClientId
        ORDER BY QuoteId DESC;";

    }
}
