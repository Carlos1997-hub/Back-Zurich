using ZurichApp.Api.Dtos.Quotes;

namespace ZurichApp.Api.Services.Interfaces
{
    public interface IQuoteService
    {
        Task<IEnumerable<QuoteResponse>> GetAllAsync();
        Task<IEnumerable<QuoteResponse>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<QuoteResponse>> GetMineAsync();
        Task<QuoteResponse> CreateAsync(QuoteCreateRequest request); 
    }
}
