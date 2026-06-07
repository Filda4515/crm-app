using CrmApp.Models;
using CrmApp.Models.Queries;

namespace CrmApp.Services;

public interface IAdvisorService
{
    Task<IEnumerable<Advisor>> GetAllAdvisors(PersonQuery? query = null);
    Task<Advisor?> GetAdvisorById(int id);
    Task CreateAdvisor(Advisor advisor);
    Task UpdateAdvisor(Advisor advisor);
    Task DeleteAdvisor(int id, bool deleteContracts);
}
