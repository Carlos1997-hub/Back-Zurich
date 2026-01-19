using System.Data;
using Dapper;
using ZurichApp.Api.Data;
using ZurichApp.Api.Data.Sql;
using ZurichApp.Api.Dtos.Clients;
using ZurichApp.Api.Models.Clients;
using ZurichApp.Api.Repositories.Interfaces;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Services
{
    public class ClientService : IClientService
    {
        private readonly DbConnectionFactory _db;
        private readonly IClientRepository _clientRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _current;

        public ClientService(
            DbConnectionFactory db,
            IClientRepository clientRepo,
            IPasswordHasher passwordHasher,
            ICurrentUserService current)
        {
            _db = db;
            _clientRepo = clientRepo;
            _passwordHasher = passwordHasher;
            _current = current;
        }

        public async Task<IEnumerable<ClientResponse>> GetAllAsync()
        {
            EnsureAdmin();

            var clients = await _clientRepo.GetAllAsync();
            return clients.Select(MapClient);
        }

        public async Task<ClientResponse?> GetByIdAsync(int clientId)
        {
            EnsureAdmin();

            var c = await _clientRepo.GetByIdAsync(clientId);
            return c == null ? null : MapClient(c);
        }

        // Cliente + Usuario (Transacción Dapper)
        public async Task<ClientResponse> CreateClientWithUserAsync(ClientCreateWithUserRequest request)
        {
            EnsureAdmin();

            using var conn = _db.CreateConnection();
            if (conn.State != ConnectionState.Open) conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                // 1) Insert Client
                int clientId = await conn.ExecuteScalarAsync<int>(
                    ClientsSql.Insert,
                    new
                    {
                        request.IdentificationNumber,
                        request.FullName,
                        request.Email,
                        request.Phone,
                        request.Address
                    },
                    tx
                );

                // 2) RoleId Cliente
                int roleIdCliente = await conn.ExecuteScalarAsync<int>(
                    AuthSql.GetRoleIdByName,
                    new { RoleName = "Cliente" },
                    tx
                );

                // 3) Password hash
                _passwordHasher.CreateHash(request.Password, out var hash, out var salt);

                // 4) Insert User linked to ClientId
                _ = await conn.ExecuteScalarAsync<int>(
                    AuthSql.InsertUser,
                    new
                    {
                        Username = request.Username,
                        Email = request.Email,
                        PasswordHash = hash,
                        PasswordSalt = salt,
                        RoleId = roleIdCliente,
                        ClientId = clientId,
                        DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? request.FullName : request.DisplayName,
                        IsActive = true
                    },
                    tx
                );

                tx.Commit();

                return new ClientResponse
                {
                    ClientId = clientId,
                    IdentificationNumber = request.IdentificationNumber,
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Address = request.Address
                };
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task UpdateAsync(int clientId, ClientUpdateRequest request)
        {
            EnsureAdmin();

            var existing = await _clientRepo.GetByIdAsync(clientId);
            if (existing == null) throw new KeyNotFoundException("Cliente no encontrado.");

            existing.FullName = request.FullName;
            existing.Email = request.Email;
            existing.Phone = request.Phone;
            existing.Address = request.Address;

            await _clientRepo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int clientId)
        {
            EnsureAdmin();
            await _clientRepo.SoftDeleteAsync(clientId);
        }

        private void EnsureAdmin()
        {
            if (!_current.IsAdmin())
                throw new UnauthorizedAccessException("Solo el administrador puede realizar esta operación.");
        }

        private static ClientResponse MapClient(Client c)
        {
            return new ClientResponse
            {
                ClientId = c.ClientId,
                IdentificationNumber = c.IdentificationNumber,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address
            };
        }
    }
}
