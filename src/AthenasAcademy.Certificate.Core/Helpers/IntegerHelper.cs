namespace AthenasAcademy.Certificate.Core.Helpers;

    /// <summary>
    /// The IntegerHelper class provides static helper methods for formatting integer values used in the Athenas Academy application.
    /// </summary>
    public static class IntegerHelper
    {
        /// <summary>
        /// Formats an integer workload value by appending a suffix.
        /// </summary>
        /// <param name="workload">The integer value representing the course workload.</param>
        /// <param name="suffix">The suffix to append to the workload value. Default is "hours".</param>
        /// <returns>A string representing the formatted workload value.</returns>
        /// <example>
        /// Input: 40
        /// Output: "40 hours"
        /// </example>
        public static string FormatCourseWorkload(int workload, string suffix = "hours") 
            => $"{workload} {suffix}";
    }
