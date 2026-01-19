namespace ZurichApp.Api.Dtos.Clients
{
    public class ClientResponse
    {
        public int ClientId { get; set; }
        public string IdentificationNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}