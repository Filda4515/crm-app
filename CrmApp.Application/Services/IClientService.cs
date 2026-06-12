using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Application.Services;

public interface IClientService
{
    Task<IEnumerable<Client>> GetAllClients(PersonQuery? query = null);
    Task<Client?> GetClientById(int id);
    Task CreateClient(Client client);
    Task UpdateClient(Client client);
    Task DeleteClient(int id, bool deleteContracts);
}
