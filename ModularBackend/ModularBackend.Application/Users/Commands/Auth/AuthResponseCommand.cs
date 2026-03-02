namespace ModularBackend.Application.Users.Commands.Auth
{
    public record AuthResponseCommand(string AccessToken, string RefreshToken, DateTime ExpiresAt);
}
