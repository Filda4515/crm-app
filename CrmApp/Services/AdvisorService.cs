using CrmApp.Data;
using CrmApp.Models;
using CrmApp.Models.Queries;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Services;

public class AdvisorService(CrmDbContext context) : IAdvisorService
{
    public async Task CreateAdvisor(Advisor advisor)
    {
        context.Advisors.Add(advisor);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAdvisor(int id, bool deleteContracts = false)
    {
        var advisor = await context.Advisors
            .Include(a => a.ManagedContracts)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (advisor != null)
        {
            if (deleteContracts && advisor.ManagedContracts != null && advisor.ManagedContracts.Count != 0)
            {
                context.Contracts.RemoveRange(advisor.ManagedContracts);
            }

            context.Advisors.Remove(advisor);
            await context.SaveChangesAsync();
        }
    }

    public async Task<Advisor?> GetAdvisorById(int id)
    {
        return await context.Advisors
            .Include(a => a.ManagedContracts)
            .Include(a => a.ParticipatedContracts)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Advisor>> GetAllAdvisors(PersonQuery? query = null)
    {
        var q = context.Advisors
            .AsNoTracking()
            .Include(a => a.ManagedContracts)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query?.Search))
        {
            var search = query.Search.Trim();
            q = q.Where(a =>
                a.FirstName.Contains(search) ||
                a.LastName.Contains(search) ||
                a.BirthNumber.Contains(search));
        }

        q = query?.SortBy switch
        {
            "lastNameDesc" => q.OrderByDescending(c => c.LastName).ThenBy(c => c.FirstName),
            "firstName" => q.OrderBy(c => c.FirstName).ThenBy(c => c.LastName),
            "firstNameDesc" => q.OrderByDescending(c => c.FirstName).ThenBy(c => c.LastName),
            "birthNumber" => q.OrderBy(c => c.BirthNumber),
            "birthNumberDesc" => q.OrderByDescending(c => c.BirthNumber),
            _ => q.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
        };

        return await q.ToListAsync();
    }

    public async Task UpdateAdvisor(Advisor advisor)
    {
        var existingAdvisor = await context.Advisors.FindAsync(advisor.Id) ?? throw new KeyNotFoundException($"Záznam s ID {advisor.Id} nebyl nalezen.");
        context.Entry(existingAdvisor).CurrentValues.SetValues(advisor);
        await context.SaveChangesAsync();
    }
}
