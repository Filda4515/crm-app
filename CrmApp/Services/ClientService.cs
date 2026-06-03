using CrmApp.Data;
using CrmApp.Models;

namespace CrmApp.Services;

public class ClientService(CrmDbContext context) : IClientService
{
    public void CreateClient(Client client)
    {
        context.Clients.Add(client);
        context.SaveChanges();
    }

    public void DeleteClient(int id)
    {
        var client = context.Clients.Find(id);
        if (client != null)
        {
            context.Clients.Remove(client);
            context.SaveChanges();
        }
    }

    public Client? GetClientById(int id)
    {
        return context.Clients.FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Client> GetAllClients()
    {
        return [.. context.Clients];
    }

    public void UpdateClient(Client client)
    {
        context.Clients.Update(client);
        context.SaveChanges();
    }
}
