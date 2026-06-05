using CrmApp.Data;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Services;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Tests.Services;

public class ClientServicesTests
{
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
        new() { Id = 1, FirstName = "Jan", LastName = "Běžný", BirthNumber = "960101/1234", Age = 30 },
        new() { Id = 2, FirstName = "Alena", LastName = "Prázdná", BirthNumber = "955555/5555", Age = 28 },
        new() { Id = 3, FirstName = "Štěpán", LastName = "Bývalý", BirthNumber = "800505/1111", Age = 46 }
    ];

    [Fact]
    public void GetAllClients_ShouldReturnAll_WhenQueryIsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        context.SaveChanges();
        var service = CreateService(context);

        // Act
        var result = service.GetAllClients(null);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetAllClients_ShouldReturnAll_WhenSearchIsNullOrWhitespace(string? search)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new PersonQuery { Search = search };

        // Act
        var result = service.GetAllClients(query);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void GetAllClients_ShouldReturnMatching_WhenSearchMatchesLastNameCase()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new PersonQuery { Search = "Prázdná" };

        // Act
        var result = service.GetAllClients(query);

        // Assert
        Assert.Single(result);
        Assert.Equal("Prázdná", result.First().LastName);
    }

    [Theory]
    [InlineData("firstName", "Alena")]
    [InlineData("firstNameDesc", "Štěpán")]
    public void GetAllClients_ShouldOrderByFirstName_WhenSortByIsFirstName(string sortOrder, string expectedFirstFirstName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllClients(query).ToList();

        // Assert
        Assert.Equal(expectedFirstFirstName, result[0].FirstName);
    }

    [Theory]
    [InlineData("lastNameDesc", "Prázdná")]
    [InlineData("invalid_sort", "Běžný")]
    public void GetAllClients_ShouldOrderByLastName_WhenSortByIsLastName(string sortOrder, string expectedFirstLastName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllClients(query).ToList();

        // Assert
        Assert.Equal(expectedFirstLastName, result[0].LastName);
    }

    [Theory]
    [InlineData("birthNumber", "800505/1111")]
    [InlineData("birthNumberDesc", "960101/1234")]
    public void GetAllClients_ShouldOrderByBirthNumber_WhenSortByIsBirthNumber(string sortOrder, string expectedFirstBirthNumber)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Clients.AddRange(GetSampleClients());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllClients(query).ToList();

        // Assert
        Assert.Equal(expectedFirstBirthNumber, result[0].BirthNumber);
    }
}
