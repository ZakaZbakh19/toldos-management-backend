using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ModularBackend.Application.Abstractions.Identity
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string email, DateTime expirationAt, IList<Claim> claimsDb);
    }
}
