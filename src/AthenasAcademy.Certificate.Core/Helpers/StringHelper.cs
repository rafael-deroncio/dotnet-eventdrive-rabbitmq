using System.Globalization;

namespace AthenasAcademy.Certificate.Core.Helpers;


/// <summary>
/// The StringHelper class provides static helper methods for formatting specific strings used in the Athenas Academy application.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Formats a name string to "Title Case", where each word starts with an uppercase letter and the remaining letters are lowercase.
    /// </summary>
    /// <param name="str">The string containing the name to be formatted.</param>
    /// <returns>The name formatted in "Title Case".</returns>
    /// <example>
    /// Input: "john doe"
    /// Output: "John Doe"
    /// </example>
    public static string FormatName(string str)
        => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                string.Join(" ", str.Split([' '], StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim())).ToLower()
            );

    /// <summary>
    /// Formats a registration string. If the length of the registration string is greater than the mask length, it trims and converts the string to uppercase. 
    /// Otherwise, it pads the string on the left with zeros to match the mask length.
    /// </summary>
    /// <param name="registration">The registration string to be formatted.</param>
    /// <param name="mask">The mask to determine the desired length of the registration string. Default is "0000000000".</param>
    /// <returns>The formatted registration string.</returns>
    /// <example>
    /// Input: registration: "abc123", mask: "0000000000"
    /// Output: "00000ABC123"
    /// </example>
    public static string FormatRegistration(string registration, string mask = "0000000000")
        => registration.Trim().Length > mask.Length ?
            registration.ToUpper().Trim() :
            registration.ToUpper().Trim().PadLeft(mask.Length, '0');

    /// <summary>
    /// Formats a student document string by combining the document type and document number. 
    /// The document number is stripped of spaces, trimmed, and converted to uppercase.
    /// </summary>
    /// <param name="type">The type of the document.</param>
    /// <param name="document">The document number to be formatted.</param>
    /// <returns>The formatted student document string in the format "type - document".</returns>
    /// <example>
    /// Input: type:"ID", document:" 123 456 789 "
    /// Output: "ID - 123456789"
    /// </example>
    public static string FormatStudentDocument(string type, string document)
        => $"{type} - {document.Replace(" ", string.Empty).Trim().ToUpper()}";
}
