using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Identity;
using ModularBackend.Infrastructure.Models.Identity;
using ModularBackend.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;

namespace ModularBackend.Infrastructure.Services.Identity
{
    public sealed class RefreshTokenService : IRefreshTokenService
    {
        private readonly IdentityUsersDbContext _ctx;
        private readonly UserManager<Users> _userManager;
        private readonly IRefreshTokenHasher _hasher;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _settings;

        public RefreshTokenService(
            IdentityUsersDbContext ctx,
            UserManager<Users> userManager,
            IRefreshTokenHasher hasher,
            ITokenService tokenService,
            IOptions<JwtSettings> settings)
        {
            _ctx = ctx;
            _userManager = userManager;
            _hasher = hasher;
            _tokenService = tokenService;
            _settings = settings.Value;
        }

        public async Task<string> IssueAsync(string userId, CancellationToken ct)
        {
            var raw = CreateRefreshTokenRaw();
            var hash = _hasher.Hash(raw);

            var now = DateTime.UtcNow;

            var entity = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                TokenHash = hash,
                UserId = userId,
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddDays(_settings.RefreshTokenDays),
                RevokedAtUtc = null,
                RevokedReason = null,
                ReplacedByTokenId = null
            };

            _ctx.RefreshTokens.Add(entity);
            await _ctx.SaveChangesAsync(ct);

            return raw;
        }

        public async Task<TokenAuth> RotateAsync(string refreshRaw, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(refreshRaw))
            {
                throw new UnauthorizedAccessException("Refresh token is required.");
            }

            var incomingHash = _hasher.Hash(refreshRaw);

            var current = await _ctx.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == incomingHash, ct)
                ?? throw new UnauthorizedAccessException("Refresh token is invalid.");

            if (current.ExpiresAtUtc <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token expired.");
            }

            if (current.RevokedAtUtc is not null)
            {
                if (current.ReplacedByTokenId is not null)
                {
                    var activeTokens = await _ctx.RefreshTokens
                        .Where(x => x.UserId == current.UserId && x.RevokedAtUtc == null && x.ExpiresAtUtc > DateTime.UtcNow)
                        .ToListAsync(ct);

                    foreach (var token in activeTokens)
                    {
                        token.RevokedAtUtc = DateTime.UtcNow;
                        token.RevokedReason = "ReuseDetected";
                    }

                    await _ctx.SaveChangesAsync(ct);
                }

                throw new UnauthorizedAccessException("Refresh token already revoked.");
            }

            var user = await _userManager.FindByIdAsync(current.UserId)
                ?? throw new UnauthorizedAccessException("User not found for refresh token.");

            current.RevokedAtUtc = DateTime.UtcNow;
            current.RevokedReason = "Rotated";

            var newRaw = CreateRefreshTokenRaw();
            var newHash = _hasher.Hash(newRaw);
            var now = DateTime.UtcNow;

            var newToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                TokenHash = newHash,
                UserId = user.Id,
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddDays(_settings.RefreshTokenDays)
            };

            _ctx.RefreshTokens.Add(newToken);
            current.ReplacedByTokenId = newToken.RefreshTokenId;

            var claims = await _userManager.GetClaimsAsync(user);
            var accessExpiration = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes);
            var accessToken = _tokenService.GenerateToken(user.Id, user.Email!, accessExpiration, claims);

            await _ctx.SaveChangesAsync(ct);

            return new TokenAuth(accessToken, accessExpiration, newRaw);
        }

        public async Task RevokeAsync(string refreshRaw, string reason, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(refreshRaw))
                return;

            var hash = _hasher.Hash(refreshRaw);

            var current = await _ctx.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

            if (current is null)
                return;

            if (current.RevokedAtUtc is null)
            {
                current.RevokedAtUtc = DateTime.UtcNow;
                current.RevokedReason = reason;
                await _ctx.SaveChangesAsync(ct);
            }
        }

        private static string CreateRefreshTokenRaw()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
