namespace CrmApp.Domain.Extensions;

public static class BirthNumberExtensions
{
    public static DateTime? GetDateOfBirth(this string birthNumber)
    {
        if (string.IsNullOrWhiteSpace(birthNumber) || !birthNumber.Contains('/'))
            return null;

        int slashIndex = birthNumber.IndexOf('/');
        if (!int.TryParse(birthNumber.AsSpan(0, 2), out int yy) ||
            !int.TryParse(birthNumber.AsSpan(2, 2), out int mm) ||
            !int.TryParse(birthNumber.AsSpan(4, 2), out int dd))
        {
            return null;
        }

        string suffix = birthNumber[(slashIndex + 1)..];
        int fullYear = suffix.Length == 3 ? 1900 + yy : 2000 + yy;
        if (fullYear > DateTime.Today.Year)
        {
            fullYear -= 100;
        }

        if (mm > 70) mm -= 70;
        else if (mm > 50) mm -= 50;
        else if (mm > 20) mm -= 20;

        return DateTime.TryParseExact($"{fullYear:D4}{mm:D2}{dd:D2}", "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime dob)
            ? dob
            : null;
    }

    public static int? GetAge(this string birthNumber)
    {
        var dob = birthNumber.GetDateOfBirth();
        if (!dob.HasValue) return null;

        int age = DateTime.Today.Year - dob.Value.Year;
        if (DateTime.Today < dob.Value.AddYears(age))
            age--;

        return age;
    }
}
