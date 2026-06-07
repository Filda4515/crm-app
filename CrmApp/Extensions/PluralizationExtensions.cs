namespace CrmApp.Extensions;

public static class PluralizationExtensions
{
    public static string PluralizeContracts(this int count)
    {
        return count switch
        {
            1 => "smlouvu",
            2 or 3 or 4 => "smlouvy",
            _ => "smluv"
        };
    }
}
