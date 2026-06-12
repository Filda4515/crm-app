using CrmApp.Application.Interfaces;
using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Application.Services;

public class ClientService(IClientRepository repository) : IClientService
{
    public async Task CreateClient(Client client)
    {
        await repository.AddAsync(client);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteClient(int id, bool deleteContracts = false)
    {
        var client = await repository.GetByIdAsync(id);

        if (client != null)
        {
            if (deleteContracts && client.Contracts != null && client.Contracts.Count != 0)
            {
                repository.DeleteRange(client.Contracts);
            }

            repository.Delete(client);
            await repository.SaveChangesAsync();
        }
    }

    public async Task<Client?> GetClientById(int id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Client>> GetAllClients(PersonQuery? query = null)
    {
        return await repository.GetAllAsync(query);
    }

    public async Task UpdateClient(Client client)
    {
        var existingClient = await repository.GetByIdAsync(client.Id) ?? throw new KeyNotFoundException($"Záznam s ID {client.Id} nebyl nalezen.");
        repository.Update(client, existingClient);
        await repository.SaveChangesAsync();
    }
}
