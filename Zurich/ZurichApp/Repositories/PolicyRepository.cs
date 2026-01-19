using Dapper;
using ZurichApp.Api.Data;
using ZurichApp.Api.Data.Sql;
using ZurichApp.Api.Repositories.Interfaces;

namespace ZurichApp.Api.Repositories
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly DbConnectionFactory _db;

        public PolicyRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(object args)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(PoliciesSql.Insert, args);
        }

        public async Task<IEnumerable<dynamic>> GetAllWithClientAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync(PoliciesSql.GetAllWithClient);
        }

        public async Task<IEnumerable<dynamic>> GetMineWithClientAsync(int clientId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync(PoliciesSql.GetMineWithClient, new { ClientId = clientId });
        }

        public async Task<IEnumerable<dynamic>> GetByClientIdWithClientAsync(int clientId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync(PoliciesSql.GetByClientIdWithClient, new { ClientId = clientId });
        }

        public async Task CancelAsync(int policyId)
        {
            using var conn = _db.CreateConnection();
            var rows = await conn.ExecuteAsync(PoliciesSql.Cancel, new { PolicyId = policyId });
            if (rows == 0)
                throw new InvalidOperationException("No se pudo cancelar la póliza (no existe o ya está cancelada).");
        }

    }
}
