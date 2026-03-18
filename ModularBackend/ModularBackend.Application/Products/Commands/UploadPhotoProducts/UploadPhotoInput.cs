namespace ModularBackend.Application.Products.Commands.UploadPhotoProducts
{
    public sealed record UploadPhotoInput(
        string FileName,
        string ContentType,
        long Length,
        Stream Content);
}
