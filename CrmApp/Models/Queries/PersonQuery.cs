namespace CrmApp.Models.Queries;

public class PersonQuery
{
    public string? Search { get; set; }
    public string SortBy { get; set; } = "lastName";
}
