using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Identity;
using ModularBackend.Infrastructure.Models.Identity;
using ModularBackend.Infrastructure.Persistance;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(IOptions<JwtSettings> options,)
    {
        _settings = options.Value;
    }

    public string GenerateToken(string userId, string email, DateTime expirationAt, IList<Claim> claimsDb)
    {
        var baseClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        baseClaims.AddRange(claimsDb);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: baseClaims,
            notBefore: DateTime.UtcNow,
            expires: expirationAt,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
