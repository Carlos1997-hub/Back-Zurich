using Dapper;
using ZurichApp.Api.Data;
using ZurichApp.Api.Data.Sql;
using ZurichApp.Api.Models.Clients;
using ZurichApp.Api.Repositories.Interfaces;

namespace ZurichApp.Api.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly DbConnectionFactory _db;

        public ClientRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Client>(ClientsSql.GetAll);
        }

        public async Task<Client?> GetByIdAsync(int clientId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Client>(ClientsSql.GetById, new { ClientId = clientId });
        }

        public async Task<int> CreateAsync(Client client)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(ClientsSql.Insert, new
            {
                client.IdentificationNumber,
                client.FullName,
                client.Email,
                client.Phone,
                client.Address
            });
        }

        public async Task UpdateAsync(Client client)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync(ClientsSql.Update, new
            {
                client.ClientId,
                client.FullName,
                client.Email,
                client.Phone,
                client.Address
            });
        }

        public async Task SoftDeleteAsync(int clientId)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync(ClientsSql.SoftDelete, new { ClientId = clientId });
        }
    }
}
