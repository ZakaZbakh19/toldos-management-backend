
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ModularBackend.Infrastructure.Helpers;
using Azure;
using ModularBackend.Application.Abstractions.Services.Files;

namespace ModularBackend.Infrastructure.Services.FileStorage
{
    public sealed class AzureBlobFileStorageService : IFileStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobFileStorageService(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
        }

        public async Task DeleteIfExistsAsync(string storageKey, CancellationToken ct)
        {
            var blobClient = _containerClient.GetBlobClient(storageKey);
            await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
        }

        public async Task<bool> ExistsAsync(string storageKey, CancellationToken ct)
        {
            var blobClient = _containerClient.GetBlobClient(storageKey);
            var response = await blobClient.ExistsAsync(ct);
            return response.Value;
        }

        public async Task<Stream> OpenReadAsync(string storageKey, CancellationToken ct)
        {
            var blobClient = _containerClient.GetBlobClient(storageKey);
            return await blobClient.OpenReadAsync(cancellationToken: ct);
        }

        public async Task<StoredFileResult> UploadAsync(FileUploadRequest request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Content);

            if (!request.Content.CanRead)
                throw new InvalidOperationException("The provided stream is not readable.");

            var contentHash = await ChecksumHelper.ComputeSha256Async(request.Content, ct);

            if (!request.Content.CanSeek)
                throw new InvalidOperationException("The provided stream must be seekable to upload after hashing.");

            request.Content.Position = 0;

            var blobClient = _containerClient.GetBlobClient(request.StorageKey);

            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = request.ContentType
                },
                Conditions = new BlobRequestConditions
                {
                    IfNoneMatch = ETag.All
                }
            };

            await blobClient.UploadAsync(request.Content, uploadOptions, ct);

            return new StoredFileResult(
                request.StorageKey,
                contentHash,
                request.ContentType,
                request.ContentLength);
        }
    }
}
