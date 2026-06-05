using CrmApp.Models;
using CrmApp.Models.Queries;

namespace CrmApp.Services;

public interface IContractService
{
    IEnumerable<Contract> GetAllContracts(ContractQuery? query = null);
    Contract? GetContractById(int id);
    void CreateContract(Contract contract);
    void UpdateContract(Contract contract);
    void DeleteContract(int id);
}
