using ZurichApp.Api.Dtos.Quotes;
using ZurichApp.Api.Repositories.Interfaces;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _repo;
        private readonly ICurrentUserService _current;

        public QuoteService(IQuoteRepository repo, ICurrentUserService current)
        {
            _repo = repo;
            _current = current;
        }

        public async Task<IEnumerable<QuoteResponse>> GetAllAsync()
        {
            if (!_current.IsAdmin())
                throw new UnauthorizedAccessException("Solo el administrador puede consultar todas las cotizaciones.");

            var rows = await _repo.GetAllAsync();
            return rows.Select(MapQuoteRow);
        }

        public async Task<IEnumerable<QuoteResponse>> GetByClientIdAsync(int clientId)
        {
            if (!_current.IsAdmin())
                throw new UnauthorizedAccessException("Solo el administrador puede consultar cotizaciones por cliente.");

            var rows = await _repo.GetByClientIdAsync(clientId);
            return rows.Select(MapQuoteRow);
        }

        public async Task<IEnumerable<QuoteResponse>> GetMineAsync()
        {
            var clientId = _current.GetClientId();
            if (!clientId.HasValue)
                throw new UnauthorizedAccessException("El usuario no está asociado a un cliente.");

            var rows = await _repo.GetMineAsync(clientId.Value);
            return rows.Select(MapQuoteRow);
        }

        public async Task<QuoteResponse> CreateAsync(QuoteCreateRequest request)
        {
            if (!_current.IsAdmin())
                throw new UnauthorizedAccessException("Solo el administrador puede crear cotizaciones.");

            var args = new
            {
                ClientId = request.ClientId,
                PolicyType = request.PolicyType,
                InsuredAmount = request.InsuredAmount,
                TermMonths = request.TermMonths,
                MonthlyPremium = request.MonthlyPremium,
                QuoteStatus = "Generada",
                Notes = request.Notes
            };

            int quoteId = await _repo.CreateAsync(args);

            return new QuoteResponse
            {
                QuoteId = quoteId,
                ClientId = request.ClientId,
                PolicyType = request.PolicyType,
                InsuredAmount = request.InsuredAmount,
                TermMonths = request.TermMonths,
                MonthlyPremium = request.MonthlyPremium,
                QuoteStatus = "Generada",
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static QuoteResponse MapQuoteRow(dynamic row)
        {
            return new QuoteResponse
            {
                QuoteId = (int)row.QuoteId,
                ClientId = (int)row.ClientId,
                PolicyType = (string)row.PolicyType,
                InsuredAmount = (decimal)row.InsuredAmount,
                TermMonths = (int)row.TermMonths,
                MonthlyPremium = (decimal)row.MonthlyPremium,
                QuoteStatus = (string)row.QuoteStatus,
                Notes = (string?)row.Notes,
                CreatedAt = (DateTime)row.CreatedAt
            };
        }
    }
}
