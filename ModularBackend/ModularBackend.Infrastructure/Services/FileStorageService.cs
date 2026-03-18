
using ModularBackend.Api.Features.Products;
using ModularBackend.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        public Task DeleteMedia(string? path, string? blob, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<FileStoredResult> UploadAsync(Stream content, string fileName, string contentType, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
