using System.Data;
using AthenasAcademy.Certificate.Core.Configurations.Enums;
using AthenasAcademy.Certificate.Core.Configurations.Settings;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Core;

public class ProcessEventRepository : BaseRepository, IProcessEventRepository
{
    private readonly ILogger<ProcessEventRepository> _logger;

    public ProcessEventRepository(
        IOptions<PostgreSettings> settings,
        ILogger<ProcessEventRepository> logger) : base(settings.Value)
    {
        _logger = logger;
    }

    public async Task<bool> EventInProcess(int process)
    {
        _logger.LogDebug("Start check event in process.");
        try
        {
            string query = @"SELECT EVP.CODE_STATUS 
                               FROM PROCCESS_EVENT EVP
                              WHERE EVP.CODE_PROCCESS_EVENT = @Process";

            using IDbConnection connection = GetConnection();
            return await connection.QueryFirstAsync<int>(query, new { process }) == (int)EventProcessStatus.OnProcess;
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on check event in process. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished check event in process.");
        }
    }

    public async Task<bool> MaximumAttemptsReached(int process, int maxAttempts)
    {
        _logger.LogDebug("Start check max attemps for event");
        try
        {
            string query = @"SELECT EVP.ATTEMPS 
                               FROM PROCCESS_EVENT EVP 
                              WHERE EVP.CODE_PROCCESS_EVENT = @Process";

            using IDbConnection connection = GetConnection();
            return await connection.QueryFirstAsync<int>(query, new { process }) == maxAttempts;
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on check max attemps for event. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished check max attemps for event");
        }
    }

    public async Task<int> SaveEventProcess(string json, EventProcessStatus status = EventProcessStatus.Padding)
    {
        _logger.LogDebug("Start save event process.");
        try
        {
            string query = @"INSERT INTO PROCCESS_EVENT (CODE_STATUS, JSON)
                             VALUES (@Status, @Json)
                             RETURNING CODE_PROCCESS_EVENT;";

            using IDbConnection connection = GetConnection();
            return await connection.QueryFirstAsync<int>(query, new { Status = (int)status, Json = json });
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on save event process. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished save event process.");
        }
    }

    public async Task<bool> UpdateEventProcess(int process, EventProcessStatus status, string error = "", bool finish = false)
    {
        _logger.LogDebug("Start update event process.");
        try
        {
            string query = @$"UPDATE PROCCESS_EVENT
                                 SET CODE_STATUS = @Status,
                                     ERROR = CASE WHEN @Error = '' THEN ERROR ELSE @Error END,
                                     ATTEMPS = CASE WHEN @Status = 4 OR @Error != '' THEN COALESCE(ATTEMPS, 0) + 1 ELSE ATTEMPS END,
                                     FINISHED = CASE WHEN @Finish THEN true ELSE false END,
                                     UPDATED_AT = NOW() 
                               WHERE CODE_PROCCESS_EVENT = @Process";

            using IDbConnection connection = GetConnection();
            return await connection.ExecuteAsync(query, new { Status = (int)status, process, error, finish }) > 0;
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on update event process. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished update event process.");
        }
    }

    public async Task<string> GetEventProcess(int process)
    {
        _logger.LogDebug("Start gets event process.");
        try
        {
            string query = @$"SELECT JSONB(JSON)
                              FROM PROCCESS_EVENT 
                              WHERE CODE_PROCCESS_EVENT = @Process";

            using IDbConnection connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<string>(query, new { process });
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on gets event process. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished gets event process.");
        }
    }
}
