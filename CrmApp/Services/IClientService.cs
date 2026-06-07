using CrmApp.Models;
using CrmApp.Models.Queries;

namespace CrmApp.Services;

public interface IClientService
{
    Task<IEnumerable<Client>> GetAllClients(PersonQuery? query = null);
    Task<Client?> GetClientById(int id);
    Task CreateClient(Client client);
    Task UpdateClient(Client client);
    Task DeleteClient(int id, bool deleteContracts);
}
