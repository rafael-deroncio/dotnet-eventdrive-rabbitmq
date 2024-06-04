using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using AthenasAcademy.Certificate.Core.Configurations.Settings;
using AthenasAcademy.Certificate.Core.Exceptions;
using AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Core.Repositories.Bucket;

public class BucketRepository(
    IAmazonS3 client,
    ILogger<BucketRepository> logger,
    IOptions<AWSSettings> options
    ) : IBucketRepository
{
    private readonly ILogger<BucketRepository> _logger = logger;
    private readonly IOptions<AWSSettings> _options = options;
    private readonly IAmazonS3 _client = client;

    public void DeleteFile(string key)
    {
        _logger.LogInformation("Starting deleting for file {0}.", key);
        try
        {
            DeleteObjectRequest request = new()
            {
                BucketName = _options.Value.BucketName,
                Key = key
            };

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
            _logger.LogInformation("Finishing deleting for file {0}.", key);
        }
    }

    public async Task DeleteFileAsync(string key)
    {
        _logger.LogInformation("Starting async deleting for file {0}.", key);
        try
        {
            DeleteObjectRequest request = new()
            {
                BucketName = _options.Value.BucketName,
                Key = key
            };

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
            _logger.LogInformation("Finishing async deleting for file {0}.", key);
        }
    }

    public Stream GetFile(string key)
    {
        _logger.LogInformation("Starting get for file {0}.", key);
        try
        {
            GetObjectRequest request = new()
            {
                BucketName = _options.Value.BucketName,
                Key = key
            };

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
            _logger.LogInformation("Finishing get for file {0}.", key);
        }
    }

    public async Task<Stream> GetFileAsync(string key)
    {
        _logger.LogInformation("Starting async get for file {0}.", key);
        try
        {
            GetObjectRequest request = new()
            {
                BucketName = _options.Value.BucketName,
                Key = key
            };

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
            _logger.LogInformation("Finishing async get for file {0}.", key);
        }
    }

    public string GetDownloadLink(string key)
    {
        _logger.LogInformation("Starting get link for download to file {0}.", key);
        try
        {
            GetPreSignedUrlRequest request = new()
            {
                BucketName = _options.Value.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddHours(_options.Value.Expires)
            };

            string response = _client.GetPreSignedURLAsync(request).Result;

            if (string.IsNullOrEmpty(response))
                throw new BaseException("File Manager Error", $"Could not get file {key}.");

            return response;
        }
        catch (BaseException) { throw; }
        catch (Exception exception)
        {
            _logger.LogError($"Erro on get link download file {0}. {1}", key, exception.Message);
            throw new BaseException(
                title: "File Manager Error",
                message: $"Erro on get link download file {key}. Plase try later.",
                inner: exception,
                code: System.Net.HttpStatusCode.InternalServerError
                );
        }
        finally
        {
            _logger.LogInformation("Finishing get link for download to file {0}.", key);
        }
    }

    public async Task InitializeBucketAsync()
    {
        _logger.LogInformation("Starting async bucket creation: {0}.", _options.Value.BucketName);
        try
        {
            if (!await AmazonS3Util.DoesS3BucketExistV2Async(_client, _options.Value.BucketName))
            {
                PutBucketRequest request = new()
                {
                    BucketName = _options.Value.BucketName,
                    UseClientRegion = true
                };

                PutBucketResponse response = await _client.PutBucketAsync(request);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    throw new BaseException("Initialize Bucket", "Error on initialize bucket with file manager.");

                _logger.LogInformation("Bucket {0} created with success.", _options.Value.BucketName);

                return;
            }

            _logger.LogInformation("Bucket {0} alredy exists.", _options.Value.BucketName);

        }
        catch (BaseException) { throw; }
        catch (Exception exception)
        {
            _logger.LogError($"Erro on delete file {0}. {1}", exception.Message);
            throw new BaseException(
                title: "File Manager Error",
                message: $"Erro on initialize bucket.",
                inner: exception,
                code: System.Net.HttpStatusCode.InternalServerError
                );
        }
        finally
        {
            _logger.LogInformation("Finishing async bucket creation: {0}.", _options.Value.BucketName);
        }
    }

    public void UploadFile(string key, Stream stream)
    {
        _logger.LogInformation("Starting upload for file {0}.", key);
        try
        {
            ITransferUtility transfer = new TransferUtility(_client);

            using (stream)
            transfer.Upload(stream, _options.Value.BucketName, key);

            _logger.LogInformation("Successfully uploaded file {0}.", key);
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
            _logger.LogInformation("Finishing upload for file {0}.", key);
        }
    }

    public async Task UploadFileAsync(string key, Stream stream)
    {
        _logger.LogInformation("Starting async upload for file {0}.", key);
        try
        {
            ITransferUtility transfer = new TransferUtility(_client);

            using (stream)
            await transfer.UploadAsync(stream, _options.Value.BucketName, key);

            _logger.LogInformation("Successfully uploaded file {0}.", key);
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
            _logger.LogInformation("Finishing asyncupload for file {0}.", key);
        }
    }
}
