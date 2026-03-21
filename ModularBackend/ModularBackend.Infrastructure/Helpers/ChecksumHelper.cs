using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ModularBackend.Infrastructure.Helpers
{
    public static class ChecksumHelper
    {
        public static async Task<string> ComputeSha256Async(Stream stream, CancellationToken ct)
        {
            using var sha256 = SHA256.Create();

            // IMPORTANT: no resetea stream automáticamente
            var hashBytes = await sha256.ComputeHashAsync(stream, ct);

            return ConvertToHex(hashBytes);
        }

        private static string ConvertToHex(byte[] bytes)
        {
            return Convert.ToHexString(bytes);
        }
    }
}
