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
        var existingContract = context.Contracts
        .Include(c => c.Participants)
        .FirstOrDefault(c => c.Id == contract.Id);

        if (existingContract != null)
        {
            context.Entry(existingContract).CurrentValues.SetValues(contract);

            if (contract.Participants != null && contract.Participants.Count != 0)
            {
                var participantIds = contract.Participants.Select(p => p.Id).ToList();
                existingContract.Participants.Clear();
                existingContract.Participants = [.. context.Advisors.Where(a => participantIds.Contains(a.Id))];
            }
            context.SaveChanges();
        }
    }
}
