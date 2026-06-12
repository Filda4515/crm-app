namespace CrmApp.Domain.Models;

public class Client : Person
{
    public ICollection<Contract> Contracts { get; set; } = [];
}
