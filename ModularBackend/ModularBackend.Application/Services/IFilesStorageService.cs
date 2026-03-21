namespace ModularBackend.Application.Services
{
    public interface IFileStorageService
    {
        Task<StoredFileResult> UploadAsync(FileUploadRequest request, CancellationToken ct);
        Task DeleteIfExistsAsync(string storageKey, CancellationToken ct);
        Task<bool> ExistsAsync(string storageKey, CancellationToken ct);
        Task<Stream> OpenReadAsync(string storageKey, CancellationToken ct);
    }
}
