using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;
using CrmApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Application.Services;

public class ContractService(CrmDbContext context) : IContractService
{
    public async Task CreateContract(Contract contract)
    {
        if (contract.Participants != null)
        {
            foreach (var participant in contract.Participants)
            {
                context.Advisors.Attach(participant);
            }
        }
        context.Contracts.Add(contract);
        await context.SaveChangesAsync();
    }

    public async Task DeleteContract(int id)
    {
        var contract = await context.Contracts.FindAsync(id);
        if (contract != null)
        {
            context.Contracts.Remove(contract);
            await context.SaveChangesAsync();
        }
    }

    public async Task<Contract?> GetContractById(int id)
    {
        return await context.Contracts
            .Include(c => c.Client)
            .Include(c => c.Manager)
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Contract>> GetAllContracts(ContractQuery? query = null)
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

    public async Task UpdateContract(Contract contract)
    {
        var existingContract = await context.Contracts
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == contract.Id)
            ?? throw new KeyNotFoundException($"Záznam s ID {contract.Id} nebyl nalezen.");

        context.Entry(existingContract).CurrentValues.SetValues(contract);

        if (contract.Participants != null)
        {
            var participantIds = contract.Participants.Select(p => p.Id).ToList();
            existingContract.Participants.Clear();
            existingContract.Participants = await context.Advisors
                .Where(a => participantIds.Contains(a.Id))
                .ToListAsync();
        }

        await context.SaveChangesAsync();
    }
}
