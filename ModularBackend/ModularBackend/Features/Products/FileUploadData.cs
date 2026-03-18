namespace ModularBackend.Api.Features.Products
{
    public sealed record FileUploadData(
        string FileName,
        string ContentType,
        long Length,
        Stream Content);
}
