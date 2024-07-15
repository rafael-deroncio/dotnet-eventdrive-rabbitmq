using System.Globalization;

namespace AthenasAcademy.Certificate.Core.Helpers;

    /// <summary>
    /// The DateHelper class provides static helper methods for formatting date values used in the Athenas Academy application.
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// Formats a DateTime value according to a specified format string.
        /// </summary>
        /// <param name="date">The DateTime value to be formatted.</param>
        /// <param name="mask">The format string to use for formatting the date. Default is "dd MMMM yyyy".</param>
        /// <returns>A string representing the formatted date.</returns>
        /// <example>
        /// Input: new DateTime(2024, 7, 15)
        /// Output: "15 July 2024"
        /// </example>
        public static string FormatDate(DateTime date, string mask = "dd MMMM yyyy") 
            => date.ToString(mask, CultureInfo.GetCultureInfo("en-US"));

        /// <summary>
        /// Formats a DateTime value with a location prefix and a specified format string.
        /// </summary>
        /// <param name="date">The DateTime value to be formatted.</param>
        /// <param name="prefix">The location prefix to prepend to the formatted date. Default is "São Paulo - Brazil".</param>
        /// <param name="mask">The format string to use for formatting the date. Default is "dd MMMM yyyy".</param>
        /// <returns>A string representing the formatted date with the location prefix.</returns>
        /// <example>
        /// Input: new DateTime(2024, 7, 15)
        /// Output: "São Paulo - Brazil, 15 July 2024"
        /// </example>
        public static string FormatLocationDate(DateTime date, string prefix = "São Paulo - Brazil", string mask = "dd MMMM yyyy") 
            => $"{prefix}, {FormatDate(date, mask)}";
    }
