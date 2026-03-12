namespace ModularBackend.Application.Users.Commands.Auth
{
    public record AuthResponseDTO(string AccessToken, string RefreshToken, DateTime ExpiresAt);
}
