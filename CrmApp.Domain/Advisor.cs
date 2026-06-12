namespace CrmApp.Domain.Models;

public class Advisor : Person
{
    public ICollection<Contract> ManagedContracts { get; set; } = [];

    public ICollection<Contract> ParticipatedContracts { get; set; } = [];
}
