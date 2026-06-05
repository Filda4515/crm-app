using CrmApp.Models;
using CrmApp.Models.Queries;

namespace CrmApp.Services;

public interface IClientService
{
    IEnumerable<Client> GetAllClients(PersonQuery? query = null);
    Client? GetClientById(int id);
    void CreateClient(Client client);
    void UpdateClient(Client client);
    void DeleteClient(int id, bool deleteContracts);
}
