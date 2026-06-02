using CrmApp.Models;

namespace CrmApp.Services;

public interface IAdvisorService
{
    IEnumerable<Advisor> GetAllAdvisors();
    Advisor? GetAdvisorById(int id);
    void CreateAdvisor(Advisor advisor);
    void UpdateAdvisor(Advisor advisor);
    void DeleteAdvisor(int id);
}
