using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Services
{
    public sealed class AzureBlobStorageOptions
    {
        public const string SectionName = "AzureBlobStorage";

        public string ConnectionString { get; init; } = string.Empty;
        public string ContainerName { get; init; } = string.Empty;

        public bool UsePrivateContainer { get; init; } = true;

        public string? PublicBaseUrl { get; init; }
    }
}
