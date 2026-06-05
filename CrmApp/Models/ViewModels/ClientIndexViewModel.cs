using CrmApp.Models.Queries;

namespace CrmApp.Models.ViewModels;

public class ClientIndexViewModel
{
    public required IEnumerable<Client> Clients { get; set; }
    public required PersonQuery Query { get; set; }
}
