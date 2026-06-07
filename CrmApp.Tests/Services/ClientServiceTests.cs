using CrmApp.Data;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Services;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Tests.Services;

public class ClientServiceTests
{
    private static CancellationToken CT => TestContext.Current.CancellationToken;

    private static CrmDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new CrmDbContext(options);
    }

    private static ClientService CreateService(CrmDbContext context)
    {
        return new ClientService(context);
    }

    private static List<Client> GetSampleClients() =>
    [
        new() { Id = 1, FirstName = "Jan", LastName = "Běžný", BirthNumber = "960101/1234" },
        new() { Id = 2, FirstName = "Alena", LastName = "Prázdná", BirthNumber = "955515/5555" },
        new() { Id = 3, FirstName = "Štěpán", LastName = "Bývalý", BirthNumber = "800505/1111" }
    ];

    [Fact]
    public async Task GetAllClients_ShouldReturnAll_WhenQueryIsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        var result = await service.GetAllClients(null);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetAllClients_ShouldReturnAll_WhenSearchIsNullOrWhitespace(string? search)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new PersonQuery { Search = search };

        // Act
        var result = await service.GetAllClients(query);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllClients_ShouldReturnMatching_WhenSearchMatchesLastNameCase()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new PersonQuery { Search = "Prázdná" };

        // Act
        var result = await service.GetAllClients(query);

        // Assert
        Assert.Single(result);
        Assert.Equal("Prázdná", result.First().LastName);
    }

    [Theory]
    [InlineData("firstName", "Alena")]
    [InlineData("firstNameDesc", "Štěpán")]
    public async Task GetAllClients_ShouldOrderByFirstName_WhenSortByIsFirstName(string sortOrder, string expectedFirstFirstName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllClients(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstFirstName, result[0].FirstName);
    }

    [Theory]
    [InlineData("lastNameDesc", "Prázdná")]
    [InlineData("invalid_sort", "Běžný")]
    public async Task GetAllClients_ShouldOrderByLastName_WhenSortByIsLastName(string sortOrder, string expectedFirstLastName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllClients(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstLastName, result[0].LastName);
    }

    [Theory]
    [InlineData("birthNumber", "800505/1111")]
    [InlineData("birthNumberDesc", "960101/1234")]
    public async Task GetAllClients_ShouldOrderByBirthNumber_WhenSortByIsBirthNumber(string sortOrder, string expectedFirstBirthNumber)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllClients(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstBirthNumber, result[0].BirthNumber);
    }

    [Fact]
    public async Task GetClientById_ShouldReturnClient_WhenClientExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        var result = await service.GetClientById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetClientById_ShouldReturnNull_WhenClientDoesNotExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        var result = await service.GetClientById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateClient_ShouldAddClientToDatabase_WhenCalled()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = CreateService(context);
        var newClient = new Client { Id = 1, FirstName = "Nový", LastName = "Klient", BirthNumber = "000000/0000" };

        // Act
        await service.CreateClient(newClient);

        // Assert
        Assert.Equal(1, await context.Clients.CountAsync(CT));
        var dbClient = await context.Clients.FirstAsync(CT);
        Assert.Equal("Nový", dbClient.FirstName);
    }

    [Fact]
    public async Task UpdateClient_ShouldModifyDatabase_WhenClientExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var client = new Client { Id = 1, FirstName = "Starý", LastName = "Klient", BirthNumber = "000000/0000" };
        context.Clients.Add(client);
        await context.SaveChangesAsync(CT);

        var service = CreateService(context);
        var updatedClient = new Client { Id = 1, FirstName = "Změněný", LastName = "Klient", BirthNumber = "000000/0000" };

        // Act
        await service.UpdateClient(updatedClient);

        // Assert
        var dbClient = await context.Clients.FirstAsync(CT);
        Assert.Equal("Změněný", dbClient.FirstName);
    }

    [Fact]
    public async Task UpdateClient_ShouldThrowKeyNotFoundException_WhenClientDoesNotExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = CreateService(context);
        var nonExistentClient = new Client { Id = 999, FirstName = "Duch", LastName = "Klient", BirthNumber = "000000/0000" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateClient(nonExistentClient));
    }

    [Fact]
    public async Task DeleteClient_ShouldRemoveClient_WhenClientExistsAndHasNoContracts()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.Add(new Client { Id = 1, FirstName = "Jan", LastName = "Běžný", BirthNumber = "960101/1234" });
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        await service.DeleteClient(1, false);

        // Assert
        Assert.Empty(await context.Clients.ToListAsync(CT));
    }

    [Fact]
    public async Task DeleteClient_ShouldNotThrow_WhenClientDoesNotExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = CreateService(context);

        // Act
        await service.DeleteClient(999, false);

        // Assert
        Assert.Empty(await context.Clients.ToListAsync(CT));
    }

    [Fact]
    public async Task DeleteClient_ShouldRemoveContracts_WhenDeleteContractsIsTrueAndHasContracts()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var client = new Client { Id = 1, FirstName = "Jan", LastName = "Běžný", BirthNumber = "960101/1234" };
        var contract = new Contract
        {
            Id = 1,
            RegistrationNumber = "001",
            Institution = "Banka",
            ClientId = 1,
            Client = client,
            ManagerId = 1,
            SignedDate = DateTime.Now,
            EffectiveDate = DateTime.Now
        };

        context.Clients.Add(client);
        context.Contracts.Add(contract);
        await context.SaveChangesAsync(CT);

        var service = CreateService(context);

        // Act
        await service.DeleteClient(1, true);

        // Assert
        Assert.Empty(await context.Clients.ToListAsync(CT));
        Assert.Empty(await context.Contracts.ToListAsync(CT));
    }
}
