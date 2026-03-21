using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Services
{
    public interface IFileAccessUrlService
    {
        Task<Uri> GetReadUrlAsync(string storageKey, CancellationToken ct);

        //Temporal Access
        Task<Uri> GetReadUrlAsync(string storageKey, TimeSpan ttl, CancellationToken ct);
    }
}
