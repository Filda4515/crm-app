using CrmApp.Application.Interfaces;
using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;
using CrmApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Infrastructure.Repositories;

public class ClientRepository(CrmDbContext context) : IClientRepository
{
    public async Task<IEnumerable<Client>> GetAllAsync(PersonQuery? query = null)
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

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await context.Clients
            .Include(c => c.Contracts)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Client client)
    {
        await context.Clients.AddAsync(client);
    }

    public void Update(Client client, Client existingClient)
    {
        context.Entry(existingClient).CurrentValues.SetValues(client);
    }

    public void Delete(Client client)
    {
        context.Clients.Remove(client);
    }

    public void DeleteRange(IEnumerable<Contract> contracts)
    {
        context.Contracts.RemoveRange(contracts);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
