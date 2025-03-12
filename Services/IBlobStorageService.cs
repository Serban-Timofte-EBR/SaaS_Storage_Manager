namespace SaaS_Storage_Manger.Services;

public interface IBlobStorageService
{
    Task UploadFileAsync(Stream fileStream, string blobName);
    Task<Stream> DownloadFileAsync(string blobName);
    Task<List<string>> ListFilesAsync();
    Task DeleteFileAsync(string blobName);
}