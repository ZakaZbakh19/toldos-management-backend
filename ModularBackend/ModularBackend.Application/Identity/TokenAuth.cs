using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Identity
{
    public record TokenAuth(string Token, DateTime ExpirateAt, string? RefreshToken);
}
