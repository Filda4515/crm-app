using CrmApp.Models;

namespace CrmApp.Services;

public interface IClientService
{
    IEnumerable<Client> GetAllClients();
    Client? GetClientById(int id);
    void CreateClient(Client client);
    void UpdateClient(Client client);
    void DeleteClient(int id);
}
