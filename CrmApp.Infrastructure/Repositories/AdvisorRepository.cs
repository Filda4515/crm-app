using CrmApp.Application.Interfaces;
using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;
using CrmApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Infrastructure.Repositories;

public class AdvisorRepository(CrmDbContext context) : IAdvisorRepository
{
    public async Task<IEnumerable<Advisor>> GetAllAsync(PersonQuery? query = null)
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

    public async Task<Advisor?> GetByIdAsync(int id)
    {
        return await context.Advisors
            .Include(a => a.ManagedContracts)
            .Include(a => a.ParticipatedContracts)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(Advisor advisor)
    {
        await context.Advisors.AddAsync(advisor);
    }

    public void Update(Advisor advisor, Advisor existingAdvisor)
    {
        context.Entry(existingAdvisor).CurrentValues.SetValues(advisor);
    }

    public void Delete(Advisor advisor)
    {
        context.Advisors.Remove(advisor);
    }

    public void DeleteRange(IEnumerable<Contract> contracts)
    {
        context.Contracts.RemoveRange(contracts);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
