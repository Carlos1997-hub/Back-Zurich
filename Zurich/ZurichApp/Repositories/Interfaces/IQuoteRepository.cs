namespace ZurichApp.Api.Repositories.Interfaces
{
    public interface IQuoteRepository
    {
        Task<int> CreateAsync(object args);

        Task<IEnumerable<dynamic>> GetAllAsync();
        Task<IEnumerable<dynamic>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<dynamic>> GetMineAsync(int clientId);
    }
}
