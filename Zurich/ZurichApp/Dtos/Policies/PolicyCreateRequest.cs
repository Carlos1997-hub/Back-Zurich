namespace ZurichApp.Api.Dtos.Policies
{
    public class PolicyCreateRequest
    {
        public int ClientId { get; set; }
        public string PolicyType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal InsuredAmount { get; set; }
    }
}