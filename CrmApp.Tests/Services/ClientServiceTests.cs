using CrmApp.Application.Interfaces;
using CrmApp.Application.Models.Queries;
using CrmApp.Application.Services;
using CrmApp.Domain.Models;

using Moq;

namespace CrmApp.Tests.Services;

public class ClientServiceTests
{
    private static (ClientService Service, Mock<IClientRepository> MockRepo) CreateService()
    {
        var mockRepo = new Mock<IClientRepository>();
        var service = new ClientService(mockRepo.Object);
        return (service, mockRepo);
    }

    private static List<Client> GetSampleClients() =>
    [
        new() { Id = 1, FirstName = "Jan", LastName = "Běžný", BirthNumber = "960101/1234" },
        new() { Id = 2, FirstName = "Alena", LastName = "Prázdná", BirthNumber = "955515/5555" },
        new() { Id = 3, FirstName = "Štěpán", LastName = "Bývalý", BirthNumber = "800505/1111" }
    ];

    [Fact]
    public async Task GetAllClients_ShouldPassQueryToRepository_WhenCalled()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var query = new PersonQuery { Search = "Jan" };
        var expectedClients = GetSampleClients();
        mockRepo.Setup(r => r.GetAllAsync(query)).ReturnsAsync(expectedClients);

        // Act
        var result = await service.GetAllClients(query);

        // Assert
        Assert.Equal(3, result.Count());
        mockRepo.Verify(r => r.GetAllAsync(query), Times.Once);
    }

    [Fact]
    public async Task GetClientById_ShouldReturnClient_WhenExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var expectedClient = GetSampleClients().First();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedClient);

        // Act
        var result = await service.GetClientById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetClientById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Client?)null);

        // Act
        var result = await service.GetClientById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateClient_ShouldAddAndSave_WhenCalled()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var newClient = new Client { Id = 4, FirstName = "Nový", LastName = "Klient", BirthNumber = "000000/0000" };

        // Act
        await service.CreateClient(newClient);

        // Assert
        mockRepo.Verify(r => r.AddAsync(newClient), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateClient_ShouldUpdateAndSave_WhenExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var existingClient = GetSampleClients().First(c => c.Id == 1);
        var updatedClient = new Client { Id = 1, FirstName = "Změněný", LastName = "Běžný", BirthNumber = "960101/1234" };

        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingClient);

        // Act
        await service.UpdateClient(updatedClient);

        // Assert
        mockRepo.Verify(r => r.Update(updatedClient, existingClient), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateClient_ShouldThrowKeyNotFoundException_WhenNotExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var nonExistentClient = new Client { Id = 999, FirstName = "Duch", LastName = "Klient", BirthNumber = "000000/0000" };
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Client?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateClient(nonExistentClient));
        mockRepo.Verify(r => r.Update(It.IsAny<Client>(), It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public async Task DeleteClient_ShouldDeleteAndSave_WhenExistsAndNoContractsDeleted()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var client = GetSampleClients().First(c => c.Id == 1);
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(client);

        // Act
        await service.DeleteClient(1, false);

        // Assert
        mockRepo.Verify(r => r.DeleteRange(It.IsAny<IEnumerable<Contract>>()), Times.Never);
        mockRepo.Verify(r => r.Delete(client), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteClient_ShouldNotDelete_WhenNotExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Client?)null);

        // Act
        await service.DeleteClient(999, true);

        // Assert
        mockRepo.Verify(r => r.Delete(It.IsAny<Client>()), Times.Never);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteClient_ShouldDeleteContractsAndClient_WhenDeleteContractsIsTrueAndHasContracts()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var client = GetSampleClients().First(c => c.Id == 1);
        var contracts = new List<Contract>
        {
            new() { Id = 1, RegistrationNumber = "001", Institution = "Banka", ClientId = 1, ManagerId = 1 }
        };
        client.Contracts = contracts;

        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(client);

        // Act
        await service.DeleteClient(1, true);

        // Assert
        mockRepo.Verify(r => r.DeleteRange(contracts), Times.Once);
        mockRepo.Verify(r => r.Delete(client), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
