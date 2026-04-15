namespace ModularBackend.Application.Features.Users
{
    public record AuthResponseDTO(string AccessToken, string RefreshToken, DateTime ExpiresAt);
}
