using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Commands.UploadPhotoProducts
{
    public sealed record UploadedPhotoDto(
        Guid PhotoId,
        string StorageKey);
}
