using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Entities
{
    public sealed class ProductFile
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; private set; }
        public string StorageKey { get; private set; } = default!;
        public string ContentType { get; private set; } = default!;
        public string ContentHash { get; private set; } = default!; // SHA-256
        public long ContentLength { get; private set; }
        public string? OriginalFileName { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private ProductFile() { }

        private ProductFile(
            Guid id,
            Guid productId,
            string storageKey,
            string contentType,
            string contentHash,
            long contentLength,
            string? originalFileName)
        {
            if(id == Guid.Empty) { throw new ArgumentNullException("id"); }

            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId is required.", nameof(productId));

            if (string.IsNullOrWhiteSpace(storageKey))
                throw new ArgumentException("StorageKey is required.", nameof(storageKey));

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("ContentType is required.", nameof(contentType));

            if (string.IsNullOrWhiteSpace(contentHash))
                throw new ArgumentException("ContentHash is required.", nameof(contentHash));

            if (string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentException("File name is required.", nameof(originalFileName));

            if (contentLength > 0)
                throw new ArgumentException("ContentLength is required.", nameof(contentLength));

            Id = id;
            ProductId = productId;
            StorageKey = storageKey;
            ContentType = contentType;
            ContentHash = contentHash;
            OriginalFileName = originalFileName;    
            ContentLength = contentLength;
            CreatedAtUtc =DateTime.UtcNow;
        }

        public static ProductFile Create(
            Guid id,
            Guid productId,
            string storageKey,
            string contentType,
            string contentHash,
            long contentLenght,
            string? originalFileName)
            => new(id, productId, storageKey, contentType, contentHash, contentLenght, originalFileName);
    }
}
