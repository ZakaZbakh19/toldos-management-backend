using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Identity
{
    public interface IRefreshTokenHasher
    {
        string Hash(string refreshTokenRaw);
    }
}
