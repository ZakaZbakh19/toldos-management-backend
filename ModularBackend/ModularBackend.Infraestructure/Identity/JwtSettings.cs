using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Identity
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
    }
}
