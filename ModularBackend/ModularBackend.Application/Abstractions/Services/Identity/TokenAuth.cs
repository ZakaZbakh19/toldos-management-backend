using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Services.Identity
{
    public sealed record TokenAuth(string Token, DateTime ExpirateAt, string? RefreshToken);
}
