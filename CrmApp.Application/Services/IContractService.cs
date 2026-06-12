using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Application.Services;

public interface IContractService
{
    Task<IEnumerable<Contract>> GetAllContracts(ContractQuery? query = null);
    Task<Contract?> GetContractById(int id);
    Task CreateContract(Contract contract);
    Task UpdateContract(Contract contract);
    Task DeleteContract(int id);
}
