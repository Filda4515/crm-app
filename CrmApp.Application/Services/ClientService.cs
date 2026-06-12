using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;
using CrmApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Application.Services;

public class ClientService(CrmDbContext context) : IClientService
{
    public async Task CreateClient(Client client)
    {
        context.Clients.Add(client);
        await context.SaveChangesAsync();
    }

    public async Task DeleteClient(int id, bool deleteContracts = false)
    {
        var client = await context.Clients
            .Include(c => c.Contracts)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client != null)
        {
            if (deleteContracts && client.Contracts != null && client.Contracts.Count != 0)
            {
                context.Contracts.RemoveRange(client.Contracts);
            }

            context.Clients.Remove(client);
            await context.SaveChangesAsync();
        }
    }

    public async Task<Client?> GetClientById(int id)
    {
        return await context.Clients
            .Include(c => c.Contracts)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Client>> GetAllClients(PersonQuery? query = null)
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

        return await q.ToListAsync();
    }

    public async Task UpdateClient(Client client)
    {
        var existingClient = await context.Clients.FindAsync(client.Id) ?? throw new KeyNotFoundException($"Záznam s ID {client.Id} nebyl nalezen.");
        context.Entry(existingClient).CurrentValues.SetValues(client);
        await context.SaveChangesAsync();
    }
}
