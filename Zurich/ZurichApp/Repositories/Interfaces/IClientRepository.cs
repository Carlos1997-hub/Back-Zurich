using ZurichApp.Api.Models.Clients;

namespace ZurichApp.Api.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int clientId);
        Task<int> CreateAsync(Client client);
        Task UpdateAsync(Client client);
        Task SoftDeleteAsync(int clientId);
    }
}
