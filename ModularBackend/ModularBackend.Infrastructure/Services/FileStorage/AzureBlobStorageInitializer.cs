using Azure.Storage.Blobs;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Services.FileStorage
{
    public sealed class AzureBlobStorageInitializer : IHostedService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageInitializer(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
