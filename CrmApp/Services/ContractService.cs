using CrmApp.Data;
using CrmApp.Models;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Services;

public class ContractService(CrmDbContext context) : IContractService
{
    public void CreateContract(Contract contract)
    {
        context.Contracts.Add(contract);
        context.SaveChanges();
    }

    public void DeleteContract(int id)
    {
        var contract = context.Contracts.Find(id);
        if (contract != null)
        {
            context.Contracts.Remove(contract);
            context.SaveChanges();
        }
    }

    public Contract? GetContractById(int id)
    {
        return context.Contracts
            .Include(c => c.Client)
            .Include(c => c.Manager)
            .Include(c => c.Participants)
            .FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Contract> GetAllContracts()
    {
        return [.. context.Contracts
            .Include(c => c.Client)
            .Include(c => c.Manager)
            ];
    }


    public void UpdateContract(Contract contract)
    {
        context.Contracts.Update(contract);
        context.SaveChanges();
    }
}
