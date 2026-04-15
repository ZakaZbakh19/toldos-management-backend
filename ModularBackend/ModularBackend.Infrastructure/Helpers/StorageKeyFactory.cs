using ModularBackend.Application.Abstractions.Services.Files;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Helpers
{
    public sealed class StorageKeyFactory : IStorageKeyFactory
    {
        public string CreateProductPhotoOriginal(Guid productId, Guid photoId, string extension)
        {
            var normalizedExtension = NormalizeExtension(extension);
            return $"products/{productId:D}/photos/{photoId:D}/original{normalizedExtension}";
        }

        public string CreateProductPhotoVariant(Guid productId, Guid photoId, string variantName, string extension)
        {
            var normalizedExtension = NormalizeExtension(extension);
            return $"products/{productId:D}/photos/{photoId:D}/{variantName}{normalizedExtension}";
        }

        private static string NormalizeExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return string.Empty;

            extension = extension.Trim().ToLowerInvariant();
            return extension.StartsWith('.') ? extension : "." + extension;
        }
    }
}
