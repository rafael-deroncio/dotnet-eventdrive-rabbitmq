using AthenasAcademy.Certificate.Core.Configurations.Enums;

namespace AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;

public interface IProccessEventRepository
{
    Task SaveEventProccess(int proccess, string json, EventProcessStatus status, string error = "");
    Task UpdateEventProccess(int proccess, EventProcessStatus status, string error = "");
    Task<bool> MaximumAttemptsReached(int proccess, int maxAttempts);
    Task<bool> EventInProccess(int proccess);
}
