using AthenasAcademy.Certificate.Core.Configurations.Enums;

namespace AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;

public interface IProccessEventRepository
{
    Task<int> SaveEventProccess(string json, EventProcessStatus status = EventProcessStatus.Padding);
    Task UpdateEventProccess(int proccess, EventProcessStatus status, string error = "");
    Task<bool> MaximumAttemptsReached(int proccess, int maxAttempts);
    Task<bool> EventInProccess(int proccess);
}
