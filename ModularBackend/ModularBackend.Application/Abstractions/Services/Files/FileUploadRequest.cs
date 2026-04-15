using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Services.Files
{
    public sealed record FileUploadRequest(
        string StorageKey,
        Stream Content,
        string ContentType,
        long ContentLength,
        string? OriginalFileName);
}
