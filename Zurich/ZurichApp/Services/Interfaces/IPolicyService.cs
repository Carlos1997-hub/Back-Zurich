using ZurichApp.Api.Dtos.Policies;

namespace ZurichApp.Api.Services.Interfaces
{
    public interface IPolicyService
    {
        Task<PolicyResponse> CreateAsync(PolicyCreateRequest request);
        Task<IEnumerable<PolicyResponse>> GetAllAsync();
        Task<IEnumerable<PolicyResponse>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<PolicyResponse>> GetMineAsync();
        Task CancelAsync(int policyId);
    }
}
