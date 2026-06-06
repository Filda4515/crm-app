using CrmApp.Data;
using CrmApp.Models;
using CrmApp.Models.Queries;

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

    public IEnumerable<Contract> GetAllContracts(ContractQuery? query = null)
    {
        var q = context.Contracts
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
            "signedDateDesc" => q.OrderByDescending(c => c.SignedDate),
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
            _ => q.OrderBy(c => c.SignedDate)
        };

        return [.. q];
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
