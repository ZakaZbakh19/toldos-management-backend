using ModularBackend.Application.Mediator;

namespace ModularBackend.Application.Features.Products.UploadPhotoProducts
{
    public sealed record UploadProductPhotosCommand(
        Guid ProductId,
        IReadOnlyCollection<UploadPhotoInput> Files) 
        : ITransactionalCommandRequest<IReadOnlyCollection<UploadedPhotoDto>>;
}
