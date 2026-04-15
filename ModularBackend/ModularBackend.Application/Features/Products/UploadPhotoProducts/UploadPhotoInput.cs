namespace ModularBackend.Application.Features.Products.UploadPhotoProducts
{
    public sealed record UploadPhotoInput(
        string FileName,
        string ContentType,
        long Length,
        Stream Content);
}
