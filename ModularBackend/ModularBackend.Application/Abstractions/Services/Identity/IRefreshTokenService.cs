namespace ModularBackend.Application.Abstractions.Services.Identity
{
    public interface IRefreshTokenService
    {
        Task<string> IssueAsync(string userId, CancellationToken ct);        
        Task<TokenAuth> RotateAsync(string refreshRaw, CancellationToken ct);     
        Task RevokeAsync(string refreshRaw, string reason, CancellationToken ct);  
    }
}
