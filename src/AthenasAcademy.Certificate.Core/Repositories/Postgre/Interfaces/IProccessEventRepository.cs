﻿using AthenasAcademy.Certificate.Core.Configurations.Enums;

namespace AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;

public interface IProccessEventRepository
{
    Task<int> SaveEventProccess(string json, EventProcessStatus status = EventProcessStatus.Padding);
    Task<string> GetEventProccess(int proccess);
    Task<bool> UpdateEventProccess(int proccess, EventProcessStatus status, string error = "", bool finish = false);
    Task<bool> MaximumAttemptsReached(int proccess, int maxAttempts);
    Task<bool> EventInProccess(int proccess);
}
