using CrmApp.Application.Interfaces;
using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Application.Services;

public class AdvisorService(IAdvisorRepository repository) : IAdvisorService
{
    public async Task CreateAdvisor(Advisor advisor)
    {
        await repository.AddAsync(advisor);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteAdvisor(int id, bool deleteContracts = false)
    {
        var advisor = await repository.GetByIdAsync(id);

        if (advisor != null)
        {
            if (deleteContracts && advisor.ManagedContracts != null && advisor.ManagedContracts.Count != 0)
            {
                repository.DeleteRange(advisor.ManagedContracts);
            }

            repository.Delete(advisor);
            await repository.SaveChangesAsync();
        }
    }

    public async Task<Advisor?> GetAdvisorById(int id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Advisor>> GetAllAdvisors(PersonQuery? query = null)
    {
        return await repository.GetAllAsync(query);
    }

    public async Task UpdateAdvisor(Advisor advisor)
    {
        var existingAdvisor = await repository.GetByIdAsync(advisor.Id) ?? throw new KeyNotFoundException($"Záznam s ID {advisor.Id} nebyl nalezen.");
        repository.Update(advisor, existingAdvisor);
        await repository.SaveChangesAsync();
    }
}
