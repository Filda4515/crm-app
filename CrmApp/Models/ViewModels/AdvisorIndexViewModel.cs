using CrmApp.Models.Queries;

namespace CrmApp.Models.ViewModels;

public class AdvisorIndexViewModel
{
    public required IEnumerable<Advisor> Advisors { get; set; }
    public required PersonQuery Query { get; set; }
}
