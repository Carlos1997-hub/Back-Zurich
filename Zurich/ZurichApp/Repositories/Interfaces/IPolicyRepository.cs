namespace ZurichApp.Api.Repositories.Interfaces
{
    public interface IPolicyRepository
    {
        Task<int> CreateAsync(object args);
        Task<IEnumerable<dynamic>> GetAllWithClientAsync();
        Task<IEnumerable<dynamic>> GetByClientIdWithClientAsync(int clientId);
        Task<IEnumerable<dynamic>> GetMineWithClientAsync(int clientId);
        Task CancelAsync(int policyId);

    }
}
