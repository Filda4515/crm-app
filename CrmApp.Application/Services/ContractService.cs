using CrmApp.Application.Interfaces;
using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Application.Services;

public class ContractService(IContractRepository repository) : IContractService
{
    public async Task CreateContract(Contract contract)
    {
        await repository.AddAsync(contract);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteContract(int id)
    {
        var contract = await repository.GetByIdAsync(id);
        if (contract != null)
        {
            repository.Delete(contract);
            await repository.SaveChangesAsync();
        }
    }

    public async Task<Contract?> GetContractById(int id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Contract>> GetAllContracts(ContractQuery? query = null)
    {
        return await repository.GetAllAsync(query);
    }

    public async Task UpdateContract(Contract contract)
    {
        var existingContract = await repository.GetByIdAsync(contract.Id) ?? throw new KeyNotFoundException($"Záznam s ID {contract.Id} nebyl nalezen.");

        repository.Update(contract, existingContract);

        if (contract.Participants != null)
        {
            var ids = contract.Participants.Select(p => p.Id).ToList();
            existingContract.Participants.Clear();
            existingContract.Participants = await repository.GetAdvisorsByIdsAsync(ids);
        }

        await repository.SaveChangesAsync();
    }
}
