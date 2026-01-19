using Dapper;
using ZurichApp.Api.Data;
using ZurichApp.Api.Data.Sql;
using ZurichApp.Api.Repositories.Interfaces;

namespace ZurichApp.Api.Repositories
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly DbConnectionFactory _db;

        public QuoteRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(object args)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(QuotesSql.Insert, args);
        }

        public async Task<IEnumerable<dynamic>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync(QuotesSql.GetAll);
        }

        public async Task<IEnumerable<dynamic>> GetByClientIdAsync(int clientId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync(QuotesSql.GetByClientId, new { ClientId = clientId });
        }

        public async Task<IEnumerable<dynamic>> GetMineAsync(int clientId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync(QuotesSql.GetByClientId, new { ClientId = clientId }); // reutiliza
        }
    }
}
