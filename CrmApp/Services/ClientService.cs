using CrmApp.Data;
using CrmApp.Models;
using CrmApp.Models.Queries;

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

    public IEnumerable<Client> GetAllClients(PersonQuery? query = null)
    {
        var q = context.Clients
            .AsNoTracking()
            .Include(c => c.Contracts)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query?.Search))
        {
            var search = query.Search.Trim();
            q = q.Where(c =>
                c.FirstName.Contains(search) ||
                c.LastName.Contains(search) ||
                c.BirthNumber.Contains(search));
        }

        q = query?.SortBy switch
        {
            "lastNameDesc" => q.OrderByDescending(c => c.LastName).ThenBy(c => c.FirstName),
            "firstName" => q.OrderBy(c => c.FirstName).ThenBy(c => c.LastName),
            "firstNameDesc" => q.OrderByDescending(c => c.FirstName).ThenBy(c => c.LastName),
            "birthNumber" => q.OrderBy(c => c.BirthNumber),
            "birthNumberDesc" => q.OrderByDescending(c => c.BirthNumber),
            _ => q.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
        };

        return [.. q];
    }

    public void UpdateClient(Client client)
    {
        var existingClient = context.Clients.Find(client.Id);
        if (existingClient != null)
        {
            context.Entry(existingClient).CurrentValues.SetValues(client);
            context.SaveChanges();
        }
    }
}
