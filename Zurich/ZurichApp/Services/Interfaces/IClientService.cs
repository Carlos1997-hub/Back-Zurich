using ZurichApp.Api.Dtos.Clients;

namespace ZurichApp.Api.Services.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientResponse>> GetAllAsync();
        Task<ClientResponse?> GetByIdAsync(int clientId);
        Task<ClientResponse> CreateClientWithUserAsync(ClientCreateWithUserRequest request);
        Task UpdateAsync(int clientId, ClientUpdateRequest request);
        Task DeleteAsync(int clientId);
    }
}
