namespace Core.Storage;

public interface IBlobStorageService
{
    /// <summary>Uploads content to the given container/blob name, creating the container if needed. Returns the stored blob name.</summary>
    Task<string> UploadAsync(string containerName, string blobName, Stream content, string contentType, CancellationToken ct = default);

    /// <summary>Opens a readable stream for the given blob.</summary>
    Task<Stream> DownloadAsync(string containerName, string blobName, CancellationToken ct = default);

    /// <summary>Deletes the given blob if it exists.</summary>
    Task DeleteAsync(string containerName, string blobName, CancellationToken ct = default);
}
