using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Identity
{
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; }

        public string TokenHash { get; set; } = default!;

        public string UserId { get; set; } = default!;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? RevokedAtUtc { get; set; }

        public string? RevokedReason { get; set; }

        public Guid? ReplacedByTokenId { get; set; }

        public RefreshToken? ReplacedByToken { get; set; }

        public bool IsActive =>
            RevokedAtUtc is null &&
            ExpiresAtUtc > DateTime.UtcNow;
    }
}
