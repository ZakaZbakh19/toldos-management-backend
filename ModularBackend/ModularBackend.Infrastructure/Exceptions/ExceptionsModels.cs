using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Exceptions
{
    public sealed class InvalidRefreshTokenException : UnauthorizedAccessException
    {
        public InvalidRefreshTokenException() : base("Refresh token is invalid.") { }
    }

    public sealed class ExpiredRefreshTokenException : UnauthorizedAccessException
    {
        public ExpiredRefreshTokenException() : base("Refresh token expired.") { }
    }

    public sealed class RefreshTokenAlreadyConsumedException : UnauthorizedAccessException
    {
        public RefreshTokenAlreadyConsumedException() : base("Refresh token already consumed.") { }
    }

    public sealed class RefreshTokenReuseDetectedException : UnauthorizedAccessException
    {
        public RefreshTokenReuseDetectedException() : base("Refresh token reuse detected.") { }
    }

    public sealed class IdentityOperationException : ArgumentException
    {
        public IdentityOperationException() : base("Error identity.") { }
    }
}
