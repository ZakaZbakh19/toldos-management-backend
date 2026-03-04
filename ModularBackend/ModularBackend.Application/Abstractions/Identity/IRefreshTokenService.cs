using ModularBackend.Application.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Identity
{
    public interface IRefreshTokenService
    {
        Task<string> IssueAsync(string userId, CancellationToken ct);        
        Task<TokenAuth> RotateAsync(string refreshRaw, CancellationToken ct);     
        Task RevokeAsync(string refreshRaw, string reason, CancellationToken ct);  
    }
}
