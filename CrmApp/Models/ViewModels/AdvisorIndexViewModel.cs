using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Web.Models.ViewModels;

public class AdvisorIndexViewModel
{
    public required IEnumerable<Advisor> Advisors { get; set; }
    public required PersonQuery Query { get; set; }
}
