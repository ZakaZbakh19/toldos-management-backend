using Microsoft.Extensions.Options;
using ModularBackend.Application.Abstractions.Services.Identity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ModularBackend.Infrastructure.Services.Identity
{
    public sealed class RefreshTokenHasher : IRefreshTokenHasher
    {
        private readonly byte[] _pepperKey;

        public RefreshTokenHasher(IOptions<JwtSettings> settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Value.SecretKey))
                throw new InvalidOperationException("Missing SecretKeyRefreshToken");

            _pepperKey = Encoding.UTF8.GetBytes(settings.Value.SecretKey);
        }

        public string Hash(string refreshTokenRaw)
        {
            using var hmac = new HMACSHA256(_pepperKey);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(refreshTokenRaw));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
