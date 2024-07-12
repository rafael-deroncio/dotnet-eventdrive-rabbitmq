using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AthenasAcademy.Certificate.Core.Exceptions;
using AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;

namespace AthenasAcademy.Certificate.Core.Repositories.Bucket;

public class BucketRepository(
    IAmazonS3 client,
    ILogger<BucketRepository> logger
    ) : IBucketRepository
{
    private readonly ILogger<BucketRepository> _logger = logger;
    private readonly IAmazonS3 _client = client;

    public void DeleteFile(string bucket, string key)
    {
        _logger.LogDebug("Starting deleting for file {0}.", key);
        try
        {
            DeleteObjectRequest request = new() { BucketName = bucket, Key = key };
            DeleteObjectResponse response = _client.DeleteObjectAsync(request).Result;

            if (response.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
                throw new BaseException("File Manager Error", $"Could not delete file {key}.");
        }
        catch (BaseException) { throw; }
        catch (Exception exception)
        {
            _logger.LogError($"Erro on delete file {0}. {1}", key, exception.Message);
            throw new BaseException(
                title: "File Manager Error",
                message: $"Erro on delete file {key}. Plase try later.",
                inner: exception,
                code: System.Net.HttpStatusCode.InternalServerError
                );
        }
        finally
        {
            _logger.LogDebug("Finishing deleting for file {0}.", key);
        }
    }

    public async Task DeleteFileAsync(string bucket, string key)
    {
        _logger.LogDebug("Starting async deleting for file {0}.", key);
        try
        {
            DeleteObjectRequest request = new() { BucketName = bucket, Key = key };
            DeleteObjectResponse response = await _client.DeleteObjectAsync(request);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
                throw new BaseException("File Manager Error", $"Could not delete file {key}.");
        }
        catch (BaseException) { throw; }
        catch (Exception exception)
        {
            _logger.LogError($"Erro on delete file {0}. {1}", key, exception.Message);
            throw new BaseException(
                title: "File Manager Error",
                message: $"Erro on delete file {key}. Plase try later.",
                inner: exception,
                code: System.Net.HttpStatusCode.InternalServerError
                );
        }
        finally
        {
            _logger.LogDebug("Finishing async deleting for file {0}.", key);
        }
    }

    public Stream GetFile(string bucket, string key)
    {
        _logger.LogDebug("Starting get for file {0}.", key);
        try
        {
            GetObjectRequest request = new() { BucketName = bucket, Key = key };
            GetObjectResponse response = _client.GetObjectAsync(request).Result;

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new BaseException("File Manager Error", $"Could not get file {key}.");

            return response.ResponseStream;
        }
        catch (BaseException) { throw; }
        catch (Exception exception)
        {
            _logger.LogError($"Erro on get file {0}. {1}", key, exception.Message);
            throw new BaseException(
                title: "File Manager Error",
                message: $"Erro on get file {key}. Plase try later.",
                inner: exception,
                code: System.Net.HttpStatusCode.InternalServerError
                );
        }
        finally
        {
            _logger.LogDebug("Finishing get for file {0}.", key);
        }
    }

    public async Task<Stream> GetFileAsync(string bucket, string key)
    {
        _logger.LogDebug("Starting async get for file {0}.", key);
        try
        {
            GetObjectRequest request = new() { BucketName = bucket, Key = key };
            GetObjectResponse response = await _client.GetObjectAsync(request);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new BaseException("File Manager Error", $"Could not get file {key}.");

            return response.ResponseStream;
        }
        catch (BaseException) { throw; }
        catch (Exception exception)
        {
            _logger.LogError($"Erro on get file {0}. {1}", key, exception.Message);
            throw new BaseException(
                title: "File Manager Error",
                message: $"Erro on get file {key}. Plase try later.",
                inner: exception,
                code: System.Net.HttpStatusCode.InternalServerError
                );
        }
        finally
        {
            _logger.LogDebug("Finishing async get for file {0}.", key);
        }
    }

    public void UploadFile(Stream stream, string bucket, string key)
    {
        _logger.LogDebug("Starting upload for file {0}.", key);
        try
        {
            new TransferUtility(_client).Upload(stream, bucket, key);

            _logger.LogDebug("Successfully uploaded file {0}.", key);
        }
        catch (AmazonS3Exception exception)
        {
            _logger.LogError($"Erro on upload file {0}. {1}", key, exception.Message);
            throw new BaseException(
                title: "File Manager Error",
                message: $"Erro on upload file {key}. Plase try later.",
                inner: exception,
                code: System.Net.HttpStatusCode.InternalServerError
                );
        }
        finally
        {
            _logger.LogDebug("Finishing upload for file {0}.", key);
        }
    }

    public async Task UploadFileAsync(Stream stream, string bucket, string key)
    {
        _logger.LogDebug("Starting async upload for file {0}.", key);
        try
        {
            await new TransferUtility(_client).UploadAsync(stream, bucket, key);

            _logger.LogDebug("Successfully uploaded file {0}.", key);
        }
        catch (AmazonS3Exception exception)
        {
            _logger.LogError($"Erro on upload file {0}. {1}", key, exception.Message);
            throw new BaseException(
                title: "File Manager Error",
                message: $"Erro on upload file {key}. Plase try later.",
                inner: exception,
                code: System.Net.HttpStatusCode.InternalServerError
                );
        }
        finally
        {
            _logger.LogDebug("Finishing asyncupload for file {0}.", key);
        }
    }

    public string GetDownloadLink(string bucket, string key, int Expires = 3600)
    {
        return _client.GetPreSignedURL(new()
        {
            BucketName = bucket,
            Key = key,
            Expires = DateTime.UtcNow.AddSeconds(Expires),
            Verb = HttpVerb.GET,
            Protocol = Protocol.HTTP
        });
    }

    public async Task<string> GetDownloadLinkAsync(string bucket, string key, int Expires = 3600)
    {
        return await _client.GetPreSignedURLAsync(new()
        {
            BucketName = bucket,
            Key = key,
            Expires = DateTime.UtcNow.AddSeconds(Expires),
            Verb = HttpVerb.GET,
            Protocol = Protocol.HTTP
        });
    }
}
