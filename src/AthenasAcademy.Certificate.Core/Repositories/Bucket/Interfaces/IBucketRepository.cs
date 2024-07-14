using AthenasAcademy.Certificate.Core.Models;

namespace AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;

/// <summary>
/// Interface for Bucket Repository
/// </summary>
public interface IBucketRepository
{
    /// <summary>
    /// Retrieves a file from the specified bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket.</param>
    /// <param name="key">The key of the file.</param>
    /// <returns>A stream representing the file content.</returns>
    Stream GetFile(string bucket, string key);

    /// <summary>
    /// Asynchronously retrieves a file from the specified bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket.</param>
    /// <param name="key">The key of the file.</param>
    /// <returns>A task representing the asynchronous operation, with a stream as the result.</returns>
    Task<Stream> GetFileAsync(string bucket, string key);

    /// <summary>
    /// Uploads a file to the specified bucket.
    /// </summary>
    /// <param name="stream">The stream representing the file content.</param>
    /// <param name="bucket">The name of the bucket.</param>
    /// <param name="key">The key for the file.</param>
    void UploadFile(Stream stream, string bucket, string key);

    /// <summary>
    /// Asynchronously uploads a file to the specified bucket.
    /// </summary>
    /// <param name="stream">The stream representing the file content.</param>
    /// <param name="bucket">The name of the bucket.</param>
    /// <param name="key">The key for the file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UploadFileAsync(Stream stream, string bucket, string key);

    /// <summary>
    /// Deletes a file from the specified bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket.</param>
    /// <param name="key">The key of the file.</param>
    void DeleteFile(string bucket, string key);

    /// <summary>
    /// Asynchronously deletes a file from the specified bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket.</param>
    /// <param name="key">The key of the file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteFileAsync(string bucket, string key);

    /// <summary>
    /// Retrieves a download link for a file from the specified bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket.</param>
    /// <param name="key">The key of the file.</param>
    /// <param name="BucketLinkDownloadExpires">The expiration time of the download link in seconds. Default is 3600 seconds.</param>
    /// <returns>A string representing the download link.</returns>
    string GetDownloadLink(string bucket, string key, int BucketLinkDownloadExpires = 3600);

    /// <summary>
    /// Asynchronously retrieves a download link for a file from the specified bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket.</param>
    /// <param name="key">The key of the file.</param>
    /// <param name="BucketLinkDownloadExpires">The expiration time of the download link in seconds. Default is 3600 seconds.</param>
    /// <returns>A task representing the asynchronous operation, with a string result containing the download link.</returns>
    Task<string> GetDownloadLinkAsync(string bucket, string key, int BucketLinkDownloadExpires = 3600);

    /// <summary>
    /// Asynchronously retrieves the details of a file from the specified bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket.</param>
    /// <param name="key">The key of the file.</param>
    /// <returns>A task representing the asynchronous operation, with a FileDetailModel result containing the file details.</returns>
    Task<FileDetailModel> GetFileDetailAsync(string bucket, string key);
}
