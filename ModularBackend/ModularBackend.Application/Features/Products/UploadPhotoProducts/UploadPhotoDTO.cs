using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Features.Products.UploadPhotoProducts
{
    public sealed record UploadedPhotoDto(
        Guid PhotoId,
        string StorageKey);
}
