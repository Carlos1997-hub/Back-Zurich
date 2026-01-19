namespace ZurichApp.Api.Models.Policies
{
    public class Policy
    {
        public int PolicyId { get; set; }
        public int ClientId { get; set; }
        public int CreatedByUserId { get; set; }
        public string PolicyType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal InsuredAmount { get; set; }
        public string PolicyStatus { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
