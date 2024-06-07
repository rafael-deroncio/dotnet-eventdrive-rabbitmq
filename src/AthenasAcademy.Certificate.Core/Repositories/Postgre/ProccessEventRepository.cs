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
        _logger.LogInformation("Start check event in proccess.");
        try
        {
            string query = @"SELECT EVP.STATUS 
                              FROM EVENT_PROCCESS
                             WHERE EVP.ID = @Proccess";

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
            _logger.LogInformation("Finished check event in proccess.");
        }
    }

    public async Task<bool> MaximumAttemptsReached(int proccess, int maxAttempts)
    {
        _logger.LogInformation("Start check max attemps for event");
        try
        {
            string query = @"SELECT EVP.ATTEMPS 
                              FROM EVENT_PROCCESS EVP 
                             WHERE EVP.ID = @Proccess";

            using IDbConnection connection = GetConnection();
            return await connection.QueryFirstAsync<int>(query, new { proccess }) >= maxAttempts;
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on check max attemps for event. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogInformation("Finished check max attemps for event");
        }
    }

    public async Task<int> SaveEventProccess(string json, EventProcessStatus status = EventProcessStatus.Padding)
    {
        _logger.LogInformation("Start save event proccess.");
        try
        {
            string query = @"INSERT INTO EVENT_PROCCESS (STATUS, JSON_EVENT, ACTIVE)
                             VALUES (@Status, @Json, true)
                             RETURNING ID;";

            using IDbConnection connection = GetConnection();
            return await connection.QueryFirstAsync<int>(query, new { status, json });
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on save event proccess. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogInformation("Finished save event proccess.");
        }
    }

    public async Task UpdateEventProccess(int proccess, EventProcessStatus status, string error = "")
    {
        _logger.LogInformation("Start update event proccess.");
        try
        {
            bool containsError = !string.IsNullOrEmpty(error);
            string addTry = containsError ? "(SELECT ATTEMPS + 1 FROM EVENT_PROCCESS WHERE @Proccess)" : "";

            string query = @$"UPDATE EVENT_PROCCESS
                                 SET STATUS = @Status,
                                     {(containsError ? "ERROR = @Error," : "")}
                                     {(!string.IsNullOrEmpty(addTry) ? $"ATTEMPS = {addTry}," : "")}
                                     UPDATED = NOW() 
                               WHERE ID = @Proccess";

            using IDbConnection connection = GetConnection();
            await connection.QueryFirstAsync<int>(query, new { status, proccess });
        }
        catch (Exception exception)
        {
            _logger.LogError("Error on update event proccess. {0}", exception.Message);
            throw;
        }
        finally
        {
            _logger.LogInformation("Finished update event proccess.");
        }
    }
}
