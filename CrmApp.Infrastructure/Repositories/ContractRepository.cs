using CrmApp.Application.Interfaces;
using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;
using CrmApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Infrastructure.Repositories;

public class ContractRepository(CrmDbContext context) : IContractRepository
{
    public async Task<IEnumerable<Contract>> GetAllAsync(ContractQuery? query = null)
    {
        var q = context.Contracts
            .AsNoTracking()
            .Include(c => c.Client)
            .Include(c => c.Manager)
            .Include(c => c.Participants)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query?.Search))
        {
            var search = query.Search.Trim();
            q = q.Where(c =>
                c.RegistrationNumber.Contains(search) ||
                c.Institution.Contains(search) ||
                (c.Client != null && (
                    c.Client.FirstName.Contains(search) ||
                    c.Client.LastName.Contains(search))) ||
                (c.Manager != null && (
                    c.Manager.FirstName.Contains(search) ||
                    c.Manager.LastName.Contains(search))));
        }

        if (query?.HideInactive == true)
            q = q.Where(c => c.EndDate == null || c.EndDate > DateTime.Today);

        q = query?.SortBy switch
        {
            "signedDate" => q.OrderBy(c => c.SignedDate),
            "registrationNumber" => q.OrderBy(c => c.RegistrationNumber),
            "registrationNumberDesc" => q.OrderByDescending(c => c.RegistrationNumber),
            "institution" => q.OrderBy(c => c.Institution).ThenBy(c => c.RegistrationNumber),
            "institutionDesc" => q.OrderByDescending(c => c.Institution).ThenBy(c => c.RegistrationNumber),
            "effectiveDate" => q.OrderBy(c => c.EffectiveDate),
            "effectiveDateDesc" => q.OrderByDescending(c => c.EffectiveDate),
            "endDate" => q.OrderBy(c => c.EndDate),
            "endDateDesc" => q.OrderByDescending(c => c.EndDate),
            "client" => q.OrderBy(c => c.Client!.LastName).ThenBy(c => c.Client!.FirstName),
            "clientDesc" => q.OrderByDescending(c => c.Client!.LastName).ThenBy(c => c.Client!.FirstName),
            "manager" => q.OrderBy(c => c.Manager!.LastName).ThenBy(c => c.Manager!.FirstName),
            "managerDesc" => q.OrderByDescending(c => c.Manager!.LastName).ThenBy(c => c.Manager!.FirstName),
            _ => q.OrderByDescending(c => c.SignedDate)
        };

        return await q.ToListAsync();
    }

    public async Task<Contract?> GetByIdAsync(int id) =>
        await context.Contracts
            .Include(c => c.Client)
            .Include(c => c.Manager)
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(Contract contract)
    {
        if (contract.Participants != null)
        {
            foreach (var p in contract.Participants)
            {
                context.Advisors.Attach(p);
            }
        }
        await context.Contracts.AddAsync(contract);
    }

    public async Task<List<Advisor>> GetAdvisorsByIdsAsync(IEnumerable<int> ids)
    {
        return await context.Advisors.Where(a => ids.Contains(a.Id)).ToListAsync();
    }

    public void Update(Contract contract, Contract existing)
    {
        context.Entry(existing).CurrentValues.SetValues(contract);
    }

    public void Delete(Contract contract)
    {
        context.Contracts.Remove(contract);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
