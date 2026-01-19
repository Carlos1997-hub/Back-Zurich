namespace ZurichApp.Api.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int? GetUserId();
        int? GetClientId();
        string? GetRole();
        bool IsAdmin();
    }
}
