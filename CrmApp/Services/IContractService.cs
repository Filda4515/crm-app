using CrmApp.Models;
using CrmApp.Models.Queries;

namespace CrmApp.Services;

public interface IContractService
{
    Task<IEnumerable<Contract>> GetAllContracts(ContractQuery? query = null);
    Task<Contract?> GetContractById(int id);
    Task CreateContract(Contract contract);
    Task UpdateContract(Contract contract);
    Task DeleteContract(int id);
}
