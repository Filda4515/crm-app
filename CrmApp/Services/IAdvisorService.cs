using CrmApp.Models;
using CrmApp.Models.Queries;

namespace CrmApp.Services;

public interface IAdvisorService
{
    IEnumerable<Advisor> GetAllAdvisors(PersonQuery? query = null);
    Advisor? GetAdvisorById(int id);
    void CreateAdvisor(Advisor advisor);
    void UpdateAdvisor(Advisor advisor);
    void DeleteAdvisor(int id, bool deleteContracts);
}
