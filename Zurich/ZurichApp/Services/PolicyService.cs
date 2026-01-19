using ZurichApp.Api.Dtos.Policies;
using ZurichApp.Api.Repositories.Interfaces;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _repo;
        private readonly ICurrentUserService _current;

        public PolicyService(IPolicyRepository repo, ICurrentUserService current)
        {
            _repo = repo;
            _current = current;
        }

        public async Task<PolicyResponse> CreateAsync(PolicyCreateRequest request)
        {
            if (!_current.IsAdmin())
                throw new UnauthorizedAccessException("Solo el administrador puede crear pólizas.");

            var userId = _current.GetUserId();
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("Token inválido.");

            var args = new
            {
                ClientId = request.ClientId,
                CreatedByUserId = userId.Value,
                PolicyType = request.PolicyType,
                StartDate = request.StartDate.Date,
                ExpirationDate = request.ExpirationDate.Date,
                InsuredAmount = request.InsuredAmount,
                PolicyStatus = "Activa"
            };

            int policyId = await _repo.CreateAsync(args);
            var list = await _repo.GetMineWithClientAsync(request.ClientId);
            var row = list.FirstOrDefault(x => (int)x.PolicyId == policyId);

            if (row == null)
            {
                return new PolicyResponse
                {
                    PolicyId = policyId,
                    ClientId = request.ClientId,
                    ClientFullName = "",
                    ClientEmail = "",
                    PolicyType = request.PolicyType,
                    StartDate = request.StartDate.Date,
                    ExpirationDate = request.ExpirationDate.Date,
                    InsuredAmount = request.InsuredAmount,
                    PolicyStatus = "Activa",
                    AssignedAt = DateTime.UtcNow
                };
            }

            return MapPolicyRow(row);
        }

        public async Task<IEnumerable<PolicyResponse>> GetAllAsync()
        {
            if (!_current.IsAdmin())
                throw new UnauthorizedAccessException("Solo el administrador puede consultar todas las pólizas.");

            var rows = await _repo.GetAllWithClientAsync();
            return rows.Select(MapPolicyRow);
        }

        public async Task<IEnumerable<PolicyResponse>> GetByClientIdAsync(int clientId)
        {
            if (!_current.IsAdmin())
                throw new UnauthorizedAccessException("Solo el administrador puede consultar pólizas por cliente.");
            var rows = await _repo.GetMineWithClientAsync(clientId);
            return rows.Select(MapPolicyRow);
        }

        public async Task<IEnumerable<PolicyResponse>> GetMineAsync()
        {
            var clientId = _current.GetClientId();
            if (!clientId.HasValue)
                throw new UnauthorizedAccessException("El usuario no está asociado a un cliente.");

            var rows = await _repo.GetMineWithClientAsync(clientId.Value);
            return rows.Select(MapPolicyRow);
        }

        private static PolicyResponse MapPolicyRow(dynamic row)
        {
            return new PolicyResponse
            {
                PolicyId = (int)row.PolicyId,
                ClientId = (int)row.ClientId,
                ClientFullName = (string)row.ClientFullName,
                ClientEmail = (string)row.ClientEmail,
                PolicyType = (string)row.PolicyType,
                StartDate = ((DateTime)row.StartDate).Date,
                ExpirationDate = ((DateTime)row.ExpirationDate).Date,
                InsuredAmount = (decimal)row.InsuredAmount,
                PolicyStatus = (string)row.PolicyStatus,
                AssignedAt = (DateTime)row.AssignedAt
            };
        }

        public async Task CancelAsync(int policyId)
        {
            await _repo.CancelAsync(policyId);
        }
    }
}
