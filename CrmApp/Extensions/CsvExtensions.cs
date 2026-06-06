namespace CrmApp.Extensions;

public static class CsvExtensions
{
    public static string EscapeCsv(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        var escaped = value.Replace("\"", "\"\"");

        if (escaped.StartsWith('=') || escaped.StartsWith('+') || escaped.StartsWith('-') || escaped.StartsWith('@'))
        {
            escaped = "'" + escaped;
        }

        return escaped.Contains(';') || escaped.Contains('"') || escaped.Contains('\n') || escaped.Contains('\r')
            ? $"\"{escaped}\""
            : escaped;
    }
}
