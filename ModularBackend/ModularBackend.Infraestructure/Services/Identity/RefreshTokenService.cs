using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
            JwtSettings settings)
        {
            _ctx = ctx;
            _userManager = userManager;
            _hasher = hasher;
            _tokenService = tokenService;
            _settings = settings;
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
                ExpiresAtUtc = now.AddDays(7),
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
                throw new ForbiddenAccessException();

            var incomingHash = _hasher.Hash(refreshRaw);

            var current = await _ctx.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == incomingHash, ct);

            if (current is null)
                throw new ForbiddenAccessException();

            // Expirado => re-login
            if (current.ExpiresAtUtc <= DateTime.UtcNow)
                throw new ForbiddenAccessException();

            // Revocado => si fue reemplazado, es reuse/replay => revocar todo lo activo
            if (current.RevokedAtUtc is not null)
            {
                if (current.ReplacedByTokenId is not null)
                {
                    var actives = await _ctx.RefreshTokens
                        .Where(x => x.UserId == current.UserId &&
                                    x.RevokedAtUtc == null &&
                                    x.ExpiresAtUtc > DateTime.UtcNow)
                        .ToListAsync(ct);

                    foreach (var rt in actives)
                    {
                        rt.RevokedAtUtc = DateTime.UtcNow;
                        rt.RevokedReason = "ReuseDetected";
                    }

                    await _ctx.SaveChangesAsync(ct);
                }

                throw new ForbiddenAccessException();
            }

            var user = await _userManager.FindByIdAsync(current.UserId);
            if (user is null)
                throw new ForbiddenAccessException();

            // Revocar actual
            current.RevokedAtUtc = DateTime.UtcNow;
            current.RevokedReason = "Rotated";

            // Crear nuevo refresh (nueva fila)
            var newRefreshRaw = CreateRefreshTokenRaw();
            var newRefreshHash = _hasher.Hash(newRefreshRaw);

            var now = DateTime.UtcNow;

            var newEntity = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                TokenHash = newRefreshHash,
                UserId = user.Id,
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddDays(7),
                RevokedAtUtc = null,
                RevokedReason = null
            };

            _ctx.RefreshTokens.Add(newEntity);

            current.ReplacedByTokenId = newEntity.RefreshTokenId;

            var claims = await _userManager.GetClaimsAsync(user);

            var accessExp = DateTime.UtcNow.AddMinutes(15);
            var accessToken = _tokenService.GenerateToken(user.Id, user.Email!, accessExp, claims);

            // Un solo SaveChanges: atómico para rotación normal
            await _ctx.SaveChangesAsync(ct);

            return new TokenAuth(accessToken, accessExp, newRefreshRaw);
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
