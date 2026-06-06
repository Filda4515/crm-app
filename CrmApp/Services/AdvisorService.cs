using CrmApp.Data;
using CrmApp.Models;
using CrmApp.Models.Queries;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Services;

public class AdvisorService(CrmDbContext context) : IAdvisorService
{
    public void CreateAdvisor(Advisor advisor)
    {
        context.Advisors.Add(advisor);
        context.SaveChanges();
    }

    public void DeleteAdvisor(int id, bool deleteContracts = false)
    {
        var advisor = context.Advisors
            .Include(a => a.ManagedContracts)
            .FirstOrDefault(a => a.Id == id);

        if (advisor != null)
        {
            if (deleteContracts && advisor.ManagedContracts != null && advisor.ManagedContracts.Count != 0)
            {
                context.Contracts.RemoveRange(advisor.ManagedContracts);
            }

            context.Advisors.Remove(advisor);
            context.SaveChanges();
        }
    }

    public Advisor? GetAdvisorById(int id)
    {
        return context.Advisors
            .Include(a => a.ManagedContracts)
            .Include(a => a.ParticipatedContracts)
            .FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Advisor> GetAllAdvisors(PersonQuery? query = null)
    {
        var q = context.Advisors
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

        return [.. q];
    }

    public void UpdateAdvisor(Advisor advisor)
    {
        var existingAdvisor = context.Advisors.Find(advisor.Id);
        if (existingAdvisor != null)
        {
            context.Entry(existingAdvisor).CurrentValues.SetValues(advisor);
            context.SaveChanges();
        }
    }
}
