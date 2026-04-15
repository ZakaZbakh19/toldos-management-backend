using System.Security.Claims;

namespace ModularBackend.Application.Abstractions.Services.Identity
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string email, DateTime expirationAt, IList<Claim> claimsDb);
    }
}
