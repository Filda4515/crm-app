namespace CrmApp.Models;

public class Client : Person
{
    public ICollection<Contract> Contracts { get; set; } = [];
}
