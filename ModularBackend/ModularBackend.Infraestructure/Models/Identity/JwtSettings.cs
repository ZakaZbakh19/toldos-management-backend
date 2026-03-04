using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Models.Identity
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = default!;
        public string SecretKeyRefreshToken { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
    }
}
