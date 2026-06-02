using CrmApp.Models;

namespace CrmApp.Services;

public interface IContractService
{
    IEnumerable<Contract> GetAllContracts();
    Contract? GetContractById(int id);
    void CreateContract(Contract contract);
    void UpdateContract(Contract contract);
    void DeleteContract(int id);
}
