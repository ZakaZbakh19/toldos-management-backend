using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Services
{
    public sealed record StoredFileResult(
        string StorageKey,
        string ContentHash,
        string ContentType,
        long ContentLength);
}
