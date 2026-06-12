using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Application.Interfaces;

public interface IContractRepository
{
    Task<IEnumerable<Contract>> GetAllAsync(ContractQuery? query = null);
    Task<Contract?> GetByIdAsync(int id);
    Task AddAsync(Contract contract);
    Task<List<Advisor>> GetAdvisorsByIdsAsync(IEnumerable<int> ids);
    void Update(Contract contract, Contract existingContract);
    void Delete(Contract contract);
    Task SaveChangesAsync();
}
