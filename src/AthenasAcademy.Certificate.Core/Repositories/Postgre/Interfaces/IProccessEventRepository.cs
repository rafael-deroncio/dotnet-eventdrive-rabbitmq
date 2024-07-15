using AthenasAcademy.Certificate.Core.Configurations.Enums;

namespace AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;

/// <summary>
/// Interface for Process Event Repository
/// </summary>
public interface IProcessEventRepository
{
    /// <summary>
    /// Saves the event process.
    /// </summary>
    /// <param name="json">The JSON string representing the event data.</param>
    /// <param name="status">The status of the event process. Default is EventProcessStatus.Padding.</param>
    /// <returns>A task representing the asynchronous operation, with an integer result indicating the process ID.</returns>
    Task<int> SaveEventProcess(string json, EventProcessStatus status = EventProcessStatus.Padding);

    /// <summary>
    /// Gets the event process by process ID.
    /// </summary>
    /// <param name="process">The process ID of the event.</param>
    /// <returns>A task representing the asynchronous operation, with a string result containing the event process data in JSON format.</returns>
    Task<string> GetEventProcess(int process);

    /// <summary>
    /// Updates the event process.
    /// </summary>
    /// <param name="process">The process ID of the event.</param>
    /// <param name="status">The new status of the event process.</param>
    /// <param name="error">The error message, if any. Default is an empty string.</param>
    /// <param name="finish">Indicates whether the process is finished. Default is false.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
    Task<bool> UpdateEventProcess(int process, EventProcessStatus status, string error = "", bool finish = false);

    /// <summary>
    /// Checks if the maximum number of attempts for the event process has been reached.
    /// </summary>
    /// <param name="process">The process ID of the event.</param>
    /// <param name="maxAttempts">The maximum number of attempts allowed.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean result indicating whether the maximum attempts have been reached.</returns>
    Task<bool> MaximumAttemptsReached(int process, int maxAttempts);

    /// <summary>
    /// Checks if the event process is currently in progress.
    /// </summary>
    /// <param name="process">The process ID of the event.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean result indicating whether the event process is in progress.</returns>
    Task<bool> EventInProcess(int process);
}
