namespace AthenasAcademy.Certificate.Core.Helpers;

/// <summary>
/// The DecimalHelper class provides static helper methods for formatting decimal values used in the Athenas Academy application.
/// </summary>
public static class DecimalHelper
{
    /// <summary>
    /// Formats a decimal course utilization value by rounding it to the nearest integer and optionally appending a percent symbol.
    /// </summary>
    /// <param name="utilization">The decimal value representing course utilization.</param>
    /// <param name="addPercentSymbol">A boolean indicating whether to add a percent symbol to the formatted string. Default is true.</param>
    /// <returns>A string representing the formatted course utilization value.</returns>
    /// <example>
    /// Input: 85.7m
    /// Output: "86%"
    /// </example>
    public static string FormatCourseUtilization(decimal utilization, bool addPercentSymbol = true)
        => $"{(int)Math.Round(utilization)}{(addPercentSymbol ? "%" : string.Empty)}";
}
