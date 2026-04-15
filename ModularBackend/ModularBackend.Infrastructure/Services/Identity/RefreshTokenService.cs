using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ModularBackend.Application.Abstractions.Services.Identity;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Infrastructure.Exceptions;
using ModularBackend.Infrastructure.Persistence;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;

namespace ModularBackend.Infrastructure.Services.Identity
{
    public sealed class RefreshTokenService : IRefreshTokenService
    {
        private TimeSpan ReuseGraceWindow = TimeSpan.FromMilliseconds(100);
        private readonly ApplicationDbContext _ctx;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Users> _userManager;
        private readonly IRefreshTokenHasher _hasher;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _settings;

        public RefreshTokenService(
            ApplicationDbContext ctx,
            UserManager<Users> userManager,
            IRefreshTokenHasher hasher,
            ITokenService tokenService,
            IOptions<JwtSettings> settings,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _ctx = ctx;
            _userManager = userManager;
            _hasher = hasher;
            _tokenService = tokenService;
            _settings = settings.Value;
            _unitOfWork = unitOfWork;

            ReuseGraceWindow = TimeSpan.FromMilliseconds(int.Parse(configuration.GetSection("Settings:ReuseGraceWindow").Value ?? string.Empty));
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
            ValidateRefreshRaw(refreshRaw);

            var now = DateTime.UtcNow;
            var current = await GetCurrentRefreshTokenAsync(refreshRaw, ct);

            await EnsureRefreshTokenCanBeRotatedAsync(current, now, ct);

            var rotation = await RotateRefreshTokenTransactionallyAsync(current, now, ct);

            var accessToken = await GenerateAccessTokenAsync(rotation.UserId, now, ct);

            return new TokenAuth(accessToken.Item1, accessToken.Item2, rotation.NewRefreshTokenRaw);
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

            var now = DateTime.UtcNow;
            var hash = _hasher.Hash(refreshRaw);

            await _ctx.RefreshTokens
            .Where(x => x.TokenHash == hash && x.RevokedAtUtc == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.RevokedAtUtc, now)
                .SetProperty(x => x.RevokedReason, reason), ct);
        }

        private static string CreateRefreshTokenRaw()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private static void ValidateRefreshRaw(string refreshRaw)
        {
            if (string.IsNullOrWhiteSpace(refreshRaw))
                throw new InvalidRefreshTokenException();
        }

        private async Task<RefreshToken> GetCurrentRefreshTokenAsync(string refreshRaw, CancellationToken ct)
        {
            var incomingHash = _hasher.Hash(refreshRaw);
            var current = await _ctx.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TokenHash == incomingHash, ct)
                ?? throw new InvalidRefreshTokenException();
            return current;
        }

        private async Task EnsureRefreshTokenCanBeRotatedAsync(RefreshToken current, DateTime now, CancellationToken ct)
        {
            if (current.ExpiresAtUtc <= now)
                throw new ExpiredRefreshTokenException();

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
        }

        private async Task<(string NewRefreshTokenRaw, string UserId)> RotateRefreshTokenTransactionallyAsync(RefreshToken current, DateTime now, CancellationToken ct)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(ct);

                var newRefreshTokenId = Guid.NewGuid();
                var affectedRows = await _ctx.RefreshTokens
                    .Where(x =>
                        x.RefreshTokenId == current.RefreshTokenId &&
                        x.RevokedAtUtc == null &&
                        x.ReplacedByTokenId == null &&
                        x.ExpiresAtUtc > now)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.RevokedAtUtc, now)
                        .SetProperty(x => x.RevokedReason, "Rotated")
                        .SetProperty(x => x.ReplacedByTokenId, newRefreshTokenId), ct);
                if (affectedRows == 0)
                {
                    throw new RefreshTokenAlreadyConsumedException();
                }
                var newRaw = CreateRefreshTokenRaw();
                var newHash = _hasher.Hash(newRaw);
                var newToken = new RefreshToken
                {
                    RefreshTokenId = newRefreshTokenId,
                    TokenHash = newHash,
                    UserId = current.UserId,
                    TokenFamilyId = current.TokenFamilyId,
                    CreatedAtUtc = now,
                    ExpiresAtUtc = now.AddDays(_settings.RefreshTokenDays)
                };
                _ctx.RefreshTokens.Add(newToken);
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitAsync(ct);
                return (newRaw, current.UserId);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }

        private async Task<(string, DateTime)> GenerateAccessTokenAsync(string userId, DateTime now, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidRefreshTokenException();
            var claims = await _userManager.GetClaimsAsync(user);
            var accessExpiresAt = now.AddMinutes(_settings.AccessTokenMinutes);
            var accessToken = _tokenService.GenerateToken(
                user.Id,
                user.Email!,
                accessExpiresAt,
                claims);
            return (accessToken, accessExpiresAt);
        }
    }
}
