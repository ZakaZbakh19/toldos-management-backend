using ModularBackend.Application.Abstractions.Messaging.Mediator;
using ModularBackend.Application.Products.Commands.CreateProduct;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Products.Commands.UploadPhotoProducts
{
    public sealed record UploadProductPhotosCommand(
        Guid ProductId,
        IReadOnlyCollection<UploadPhotoInput> Files) 
        : ITransactionalCommandRequest<IReadOnlyCollection<UploadedPhotoDto>>;
}
