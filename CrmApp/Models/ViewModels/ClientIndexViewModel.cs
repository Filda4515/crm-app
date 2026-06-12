using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Web.Models.ViewModels;

public class ClientIndexViewModel
{
    public required IEnumerable<Client> Clients { get; set; }
    public required PersonQuery Query { get; set; }
}
