using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Application.Interfaces;

public interface IAdvisorRepository
{
    Task<IEnumerable<Advisor>> GetAllAsync(PersonQuery? query = null);
    Task<Advisor?> GetByIdAsync(int id);
    Task AddAsync(Advisor advisor);
    void Update(Advisor advisor, Advisor existingAdvisor);
    void Delete(Advisor advisor);
    void DeleteRange(IEnumerable<Contract> contracts);
    Task SaveChangesAsync();
}
