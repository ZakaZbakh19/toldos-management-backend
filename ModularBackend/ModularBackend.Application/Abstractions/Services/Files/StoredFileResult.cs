using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Services.Files
{
    public sealed record StoredFileResult(
        string StorageKey,
        string ContentHash,
        string ContentType,
        long ContentLength);
}
