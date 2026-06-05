using CrmApp.Models.Queries;

namespace CrmApp.Models.ViewModels;

public class ContractIndexViewModel
{
    public required IEnumerable<Contract> Contracts { get; set; }
    public required ContractQuery Query { get; set; }
}
