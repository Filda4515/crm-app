namespace CrmApp.Models.Queries;

public class ContractQuery
{
    public string? Search { get; set; }
    public string SortBy { get; set; } = "signedDateDesc";
    public bool HideInactive { get; set; } = false;
}
