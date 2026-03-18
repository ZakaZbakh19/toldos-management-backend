namespace ModularBackend.Api.Features.Products
{
    public sealed class UploadProductPhotosRequest
    {
        public List<IFormFile> Files { get; init; } = [];
    }
}
