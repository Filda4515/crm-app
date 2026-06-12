using CrmApp.Application.Models.Queries;
using CrmApp.Domain.Models;

namespace CrmApp.Application.Interfaces;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAllAsync(PersonQuery? query = null);
    Task<Client?> GetByIdAsync(int id);
    Task AddAsync(Client client);
    void Update(Client client, Client existingClient);
    void Delete(Client client);
    void DeleteRange(IEnumerable<Contract> contracts);
    Task SaveChangesAsync();
}
