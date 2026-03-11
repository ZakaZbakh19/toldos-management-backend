using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Exceptions
{
    public sealed class InvalidRefreshTokenException : Exception
    {
        public InvalidRefreshTokenException() : base("Refresh token is invalid.") { }
    }

    public sealed class ExpiredRefreshTokenException : Exception
    {
        public ExpiredRefreshTokenException() : base("Refresh token expired.") { }
    }

    public sealed class RefreshTokenAlreadyConsumedException : Exception
    {
        public RefreshTokenAlreadyConsumedException() : base("Refresh token already consumed.") { }
    }

    public sealed class RefreshTokenReuseDetectedException : Exception
    {
        public RefreshTokenReuseDetectedException() : base("Refresh token reuse detected.") { }
    }
}
