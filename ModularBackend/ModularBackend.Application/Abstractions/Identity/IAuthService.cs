using ModularBackend.Application.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Identity
{
    public interface IAuthService
    {
        Task<TokenAuth> RegisterAsync(string name, string email, string password, CancellationToken ct);
        Task<TokenAuth> LoginAsync(string email, string password, CancellationToken ct);
        Task<TokenAuth> RefreshAsync(string refreshRaw, CancellationToken ct);
        Task LogoutAsync(string refreshRaw, CancellationToken ct);
    }
}
