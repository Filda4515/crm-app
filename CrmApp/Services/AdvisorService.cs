using CrmApp.Data;
using CrmApp.Models;

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

    public IEnumerable<Advisor> GetAllAdvisors()
    {
        return [.. context.Advisors.Include(a => a.ManagedContracts)];
    }

    public void UpdateAdvisor(Advisor advisor)
    {
        context.Advisors.Update(advisor);
        context.SaveChanges();
    }
}
