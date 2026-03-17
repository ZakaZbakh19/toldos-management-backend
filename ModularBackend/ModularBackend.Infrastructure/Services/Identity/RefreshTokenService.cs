using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ModularBackend.Application.Abstractions.Identity;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Identity;
using ModularBackend.Infrastructure.Exceptions;
using ModularBackend.Infrastructure.Models.Identity;
using ModularBackend.Infrastructure.Persistance.Context;
using System.Security.Cryptography;

namespace ModularBackend.Infrastructure.Services.Identity
{
    public sealed class RefreshTokenService : IRefreshTokenService
    {
        private static readonly TimeSpan ReuseGraceWindow = TimeSpan.FromSeconds(2);

        private readonly IdentityUsersDbContext _ctx;
        private readonly IIdentityUnitOfWork _unitOfWork;
        private readonly UserManager<Users> _userManager;
        private readonly IRefreshTokenHasher _hasher;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _settings;

        public RefreshTokenService(
            IdentityUsersDbContext ctx,
            UserManager<Users> userManager,
            IRefreshTokenHasher hasher,
            ITokenService tokenService,
            IOptions<JwtSettings> settings,
            IIdentityUnitOfWork unitOfWork)
        {
            _ctx = ctx;
            _userManager = userManager;
            _hasher = hasher;
            _tokenService = tokenService;
            _settings = settings.Value;
            _unitOfWork = unitOfWork;
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
                TokenFamilyId = Guid.NewGuid(),
                RevokedAtUtc = null,
                RevokedReason = null,
                ReplacedByTokenId = null
            };

            _ctx.RefreshTokens.Add(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return raw;
        }

        public async Task<TokenAuth> RotateAsync(string refreshRaw, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(refreshRaw))
                throw new InvalidRefreshTokenException();

            var now = DateTime.UtcNow;
            var incomingHash = _hasher.Hash(refreshRaw);

            var current = await _ctx.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TokenHash == incomingHash, ct)
                ?? throw new InvalidRefreshTokenException();

            if (current.ExpiresAtUtc <= now)
                throw new ExpiredRefreshTokenException();

            // Ya está revocado: aquí distinguimos carrera legítima vs reuse sospechoso
            if (current.RevokedAtUtc is not null)
            {
                var elapsed = now - current.RevokedAtUtc.Value;

                if (current.ReplacedByTokenId is not null && elapsed > ReuseGraceWindow)
                {
                    await RevokeFamilyAsync(current.TokenFamilyId, now, "ReuseDetected", ct);
                    throw new RefreshTokenReuseDetectedException();
                }

                throw new RefreshTokenAlreadyConsumedException();
            }

            var user = await _userManager.FindByIdAsync(current.UserId)
                ?? throw new InvalidRefreshTokenException();

            var claims = await _userManager.GetClaimsAsync(user);

            await _unitOfWork.BeginTransactionAsync(ct);

            // Consumo atómico del refresh token actual
            var affectedRows = await _ctx.RefreshTokens
                .Where(x =>
                    x.RefreshTokenId == current.RefreshTokenId &&
                    x.RevokedAtUtc == null &&
                    x.ReplacedByTokenId == null &&
                    x.ExpiresAtUtc > now)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.RevokedAtUtc, now)
                    .SetProperty(x => x.RevokedReason, "Rotated"), ct);

            if (affectedRows == 0)
            {
                await _unitOfWork.RollbackAsync(ct);
                throw new RefreshTokenAlreadyConsumedException();
            }

            var newRaw = CreateRefreshTokenRaw();
            var newHash = _hasher.Hash(newRaw);
            var newRefreshTokenId = Guid.NewGuid();

            var newToken = new RefreshToken
            {
                RefreshTokenId = newRefreshTokenId,
                TokenHash = newHash,
                UserId = user.Id,
                TokenFamilyId = current.TokenFamilyId,
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddDays(_settings.RefreshTokenDays)
            };

            _ctx.RefreshTokens.Add(newToken);

            await _unitOfWork.SaveChangesAsync(ct);

            // Enlazar el token antiguo con el nuevo
            await _ctx.RefreshTokens
                .Where(x => x.RefreshTokenId == current.RefreshTokenId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ReplacedByTokenId, newRefreshTokenId), ct);

            await _unitOfWork.CommitAsync(ct);

            var accessExpiresAt = now.AddMinutes(_settings.AccessTokenMinutes);
            var accessToken = _tokenService.GenerateToken(
                user.Id,
                user.Email!,
                accessExpiresAt,
                claims);

            return new TokenAuth(accessToken, accessExpiresAt, newRaw);
        }

        private async Task RevokeFamilyAsync(
            Guid tokenFamilyId,
            DateTime now,
            string reason,
            CancellationToken ct)
        {
            await _ctx.RefreshTokens
                .Where(x =>
                    x.TokenFamilyId == tokenFamilyId &&
                    x.RevokedAtUtc == null &&
                    x.ExpiresAtUtc > now)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.RevokedAtUtc, now)
                    .SetProperty(x => x.RevokedReason, reason), ct);
        }

        public async Task RevokeAsync(string refreshRaw, string reason, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(refreshRaw))
                throw new InvalidRefreshTokenException();

            var hash = _hasher.Hash(refreshRaw);

            var current = await _ctx.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TokenHash == hash, ct)
                ?? throw new InvalidRefreshTokenException();

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
