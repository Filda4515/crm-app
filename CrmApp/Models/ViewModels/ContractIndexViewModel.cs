using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Web.Models.ViewModels;

public class ContractIndexViewModel
{
    public required IEnumerable<Contract> Contracts { get; set; }
    public required ContractQuery Query { get; set; }
}
