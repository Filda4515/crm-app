using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Application.Services;

public interface IAdvisorService
{
    Task<IEnumerable<Advisor>> GetAllAdvisors(PersonQuery? query = null);
    Task<Advisor?> GetAdvisorById(int id);
    Task CreateAdvisor(Advisor advisor);
    Task UpdateAdvisor(Advisor advisor);
    Task DeleteAdvisor(int id, bool deleteContracts);
}
