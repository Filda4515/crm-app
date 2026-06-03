using CrmApp.Data;
using CrmApp.Models;

namespace CrmApp.Services;

public class AdvisorService(CrmDbContext context) : IAdvisorService
{
    public void CreateAdvisor(Advisor advisor)
    {
        context.Advisors.Add(advisor);
        context.SaveChanges();
    }

    public void DeleteAdvisor(int id)
    {
        var advisor = context.Advisors.Find(id);
        if (advisor != null)
        {
            context.Advisors.Remove(advisor);
            context.SaveChanges();
        }
    }

    public Advisor? GetAdvisorById(int id)
    {
        return context.Advisors.FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Advisor> GetAllAdvisors()
    {
        return [.. context.Advisors];
    }

    public void UpdateAdvisor(Advisor advisor)
    {
        context.Advisors.Update(advisor);
        context.SaveChanges();
    }
}
