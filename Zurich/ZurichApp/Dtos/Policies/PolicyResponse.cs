namespace ZurichApp.Api.Dtos.Policies
{
    public class PolicyResponse
    {
        public int PolicyId { get; set; }
        public int ClientId { get; set; }
        public string ClientFullName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string PolicyType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal InsuredAmount { get; set; }
        public string PolicyStatus { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }
}