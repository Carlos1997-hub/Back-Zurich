namespace ZurichApp.Api.Models.Quotes
{
    public class Quote
    {
        public int QuoteId { get; set; }
        public int ClientId { get; set; }
        public string PolicyType { get; set; } = string.Empty;
        public decimal InsuredAmount { get; set; }
        public int TermMonths { get; set; }
        public decimal MonthlyPremium { get; set; }
        public string QuoteStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
    }
}
