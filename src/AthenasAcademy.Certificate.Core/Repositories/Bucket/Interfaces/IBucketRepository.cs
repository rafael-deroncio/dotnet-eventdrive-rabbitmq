namespace AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;

public interface IBucketRepository
{
    Stream GetFile(string key);
    Task<Stream> GetFileAsync(string key);
    void UploadFile(string key, Stream stream);
    Task UploadFileAsync(string key, Stream stream);
    void DeleteFile(string key);
    Task DeleteFileAsync(string key);
    string GetDownloadLink(string key);
    Task InitializeBucketAsync();
}
