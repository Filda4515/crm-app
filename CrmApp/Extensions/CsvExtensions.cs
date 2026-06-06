namespace CrmApp.Extensions;

public static class CsvExtensions
{
    public static byte[] GetCsvBytes(this System.Text.StringBuilder sb)
    {
        var bom = System.Text.Encoding.UTF8.GetPreamble();
        var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        return [.. bom, .. bytes];
    }

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
