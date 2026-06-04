using CrmApp.Data;
using CrmApp.Models;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Services;

public class ClientService(CrmDbContext context) : IClientService
{
    public void CreateClient(Client client)
    {
        context.Clients.Add(client);
        context.SaveChanges();
    }

    public void DeleteClient(int id, bool deleteContracts = false)
    {
        var client = context.Clients
            .Include(c => c.Contracts)
            .FirstOrDefault(c => c.Id == id);

        if (client != null)
        {
            if (deleteContracts && client.Contracts != null && client.Contracts.Any())
            {
                context.Contracts.RemoveRange(client.Contracts);
            }

            context.Clients.Remove(client);
            context.SaveChanges();
        }
    }

    public Client? GetClientById(int id)
    {
        return context.Clients
            .Include(c => c.Contracts)
            .FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Client> GetAllClients()
    {
        return [.. context.Clients.Include(c => c.Contracts)];
    }

    public void UpdateClient(Client client)
    {
        context.Clients.Update(client);
        context.SaveChanges();
    }
}
