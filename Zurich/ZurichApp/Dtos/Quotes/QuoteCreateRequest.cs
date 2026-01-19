namespace ZurichApp.Api.Dtos.Quotes
{
    public class QuoteCreateRequest
    {
        public int ClientId { get; set; }
        public string PolicyType { get; set; } = string.Empty;
        public decimal InsuredAmount { get; set; }
        public int TermMonths { get; set; }
        public decimal MonthlyPremium { get; set; }
        public string? Notes { get; set; }
    }
}