using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using ModularBackend.Application.Abstractions.Services.Files;

namespace ModularBackend.Infrastructure.Services.FileStorage;

public sealed class AzureBlobAccessUrlService : IFileAccessUrlService
{
    private readonly BlobContainerClient _containerClient;
    private readonly AzureBlobStorageOptions _options;

    public AzureBlobAccessUrlService(
        BlobContainerClient containerClient,
        IOptions<AzureBlobStorageOptions> options)
    {
        _containerClient = containerClient;
        _options = options.Value;
    }

    public Task<Uri> GetReadUrlAsync(string storageKey, CancellationToken ct)
    {
        if (!_options.UsePrivateContainer)
        {
            if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
                return Task.FromResult(new Uri($"{_options.PublicBaseUrl.TrimEnd('/')}/{storageKey}"));

            var publicUri = _containerClient.GetBlobClient(storageKey).Uri;
            return Task.FromResult(publicUri);
        }

        return GetReadUrlAsync(storageKey, TimeSpan.FromMinutes(15), ct);
    }

    public Task<Uri> GetReadUrlAsync(string storageKey, TimeSpan ttl, CancellationToken ct)
    {
        if (!_options.UsePrivateContainer)
        {
            if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
                return Task.FromResult(new Uri($"{_options.PublicBaseUrl.TrimEnd('/')}/{storageKey}"));

            var publicUri = _containerClient.GetBlobClient(storageKey).Uri;
            return Task.FromResult(publicUri);
        }

        var blobClient = _containerClient.GetBlobClient(storageKey);

        if (!blobClient.CanGenerateSasUri)
            throw new InvalidOperationException("Cannot generate SAS URI with the current BlobClient configuration.");

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerClient.Name,
            BlobName = storageKey,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(ttl)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return Task.FromResult(sasUri);
    }
}
