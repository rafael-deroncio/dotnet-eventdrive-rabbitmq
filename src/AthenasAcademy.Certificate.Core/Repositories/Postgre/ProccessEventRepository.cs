using System.Data;
using AthenasAcademy.Certificate.Core.Configurations.Enums;
using AthenasAcademy.Certificate.Core.Configurations.Settings;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Core;

public class ProccessEventRepository : BaseRepository, IProccessEventRepository
{
    private readonly ILogger<ProccessEventRepository> _logger;

    public ProccessEventRepository(
        IOptions<PostgreSettings> settings,
        ILogger<ProccessEventRepository> logger) : base(settings.Value)
    {
        _logger = logger;
    }

    public async Task<bool> EventInProccess(int proccess)
    {
        _logger.LogDebug("Start check event in proccess.");
        try
        {
            string query = @"SELECT EVP.CODE_STATUS 
                               FROM PROCCESS_EVENT EVP
                              WHERE EVP.CODE_PROCCESS_EVENT = @Proccess";

            using IDbConnection connection = GetConnection();
            return await connection.QueryFirstAsync<int>(query, new { proccess }) == (int)EventProcessStatus.OnProccess;
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on check event in proccess. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished check event in proccess.");
        }
    }

    public async Task<bool> MaximumAttemptsReached(int proccess, int maxAttempts)
    {
        _logger.LogDebug("Start check max attemps for event");
        try
        {
            string query = @"SELECT EVP.ATTEMPS 
                               FROM PROCCESS_EVENT EVP 
                              WHERE EVP.CODE_PROCCESS_EVENT = @Proccess";

            using IDbConnection connection = GetConnection();
            return await connection.QueryFirstAsync<int>(query, new { proccess }) == maxAttempts;
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

    public async Task<int> SaveEventProccess(string json, EventProcessStatus status = EventProcessStatus.Padding)
    {
        _logger.LogDebug("Start save event proccess.");
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
            _logger.LogError("Error on save event proccess. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished save event proccess.");
        }
    }

    public async Task<bool> UpdateEventProccess(int proccess, EventProcessStatus status, string error = "", bool finish = false)
    {
        _logger.LogDebug("Start update event proccess.");
        try
        {
            string query = @$"UPDATE PROCCESS_EVENT
                                 SET CODE_STATUS = @Status,
                                     ERROR = CASE WHEN @Error = '' THEN ERROR ELSE @Error END,
                                     ATTEMPS = CASE WHEN @Status = 4 OR @Error != '' THEN COALESCE(ATTEMPS, 0) + 1 ELSE ATTEMPS END,
                                     FINISHED = CASE WHEN @Finish THEN true ELSE false END,
                                     UPDATED_AT = NOW() 
                               WHERE CODE_PROCCESS_EVENT = @Proccess";

            using IDbConnection connection = GetConnection();
            return await connection.ExecuteAsync(query, new { Status = (int)status, proccess, error, finish }) > 0;
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on update event proccess. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished update event proccess.");
        }
    }

    public async Task<string> GetEventProccess(int proccess)
    {
        _logger.LogDebug("Start gets event proccess.");
        try
        {
            string query = @$"SELECT JSONB(JSON)
                              FROM PROCCESS_EVENT 
                              WHERE CODE_PROCCESS_EVENT = @Proccess";

            using IDbConnection connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<string>(query, new { proccess });
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on gets event proccess. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished gets event proccess.");
        }
    }
}
