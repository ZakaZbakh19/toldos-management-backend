using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Services.Files
{
    public interface IStorageKeyFactory
    {
        string CreateProductPhotoOriginal(Guid productId, Guid photoId, string extension);
        string CreateProductPhotoVariant(Guid productId, Guid photoId, string variantName, string extension);
    }
}
