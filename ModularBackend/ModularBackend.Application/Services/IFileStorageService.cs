using ModularBackend.Api.Features.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Services
{
    public interface IFileStorageService
    {
        Task<FileStoredResult> UploadAsync(Stream content, string fileName, string contentType, CancellationToken ct);
        Task DeleteMedia(string? path, string? blob, CancellationToken ct);
    }
}
