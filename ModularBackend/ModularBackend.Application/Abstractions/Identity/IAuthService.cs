using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Identity
{
    public interface IAuthService
    {
        Task<(string, string ,DateTime)> CreateUserAsync(string name, string email, string password);
        Task<(string, string, DateTime)> LoginUserAsync(string email, string password);
        Task<(string, string, DateTime)> RefreshTokenAsync(string refreshToken);
    }
}
