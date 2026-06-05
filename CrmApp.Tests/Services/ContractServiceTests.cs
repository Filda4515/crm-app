using CrmApp.Data;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Services;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Tests.Services;

public class ContractServiceTests
{
    private static CrmDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new CrmDbContext(options);
    }

    private static ContractService CreateService(CrmDbContext context)
    {
        return new ContractService(context);
    }

    private static List<Contract> GetSampleContracts()
    {
        var clientBez = new Client { Id = 1, FirstName = "Jan", LastName = "Běžný", BirthNumber = "960101/1234" };
        var clientByv = new Client { Id = 3, FirstName = "Štěpán", LastName = "Bývalý", BirthNumber = "800505/1111" };

        var managerObo = new Advisor { Id = 1, FirstName = "Petr", LastName = "Obojí", BirthNumber = "850202/5678" };
        var managerByv = new Advisor { Id = 4, FirstName = "Žaneta", LastName = "Bývalá-Správcová", BirthNumber = "980808/8888" };

        return
        [
            new() {
                Id = 1, RegistrationNumber = "2026/001", Institution = "ČSOB",
                SignedDate = new DateTime(2026, 6, 7), EffectiveDate = new DateTime(2026, 7, 7), EndDate = null,
                Client = clientBez, Manager = managerObo
            },
            new() {
                Id = 2, RegistrationNumber = "2024/099", Institution = "Komerční banka",
                SignedDate = new DateTime(2024, 1, 15), EffectiveDate = new DateTime(2024, 2, 1), EndDate = new DateTime(2025, 12, 31),
                Client = clientByv, Manager = managerByv
            },
            new() {
                Id = 3, RegistrationNumber = "2026/002", Institution = "Kooperativa",
                SignedDate = new DateTime(2026, 1, 10), EffectiveDate = new DateTime(2026, 1, 10), EndDate = new DateTime(2035, 1, 1),
                Client = clientBez, Manager = managerObo
            }
        ];
    }

    [Fact]
    public void GetAllContracts_ShouldReturnAll_WhenQueryIsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        context.SaveChanges();
        var service = CreateService(context);

        // Act
        var result = service.GetAllContracts(null);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void GetAllContracts_ShouldExcludeExpired_WhenHideInactiveIsTrue()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new ContractQuery { HideInactive = true };

        // Act
        var result = service.GetAllContracts(query).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, c => c.RegistrationNumber == "2024/099");
    }

    [Fact]
    public void GetAllContracts_ShouldReturnMatching_WhenSearchMatchesInstitution()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new ContractQuery { Search = "Komer" };

        // Act
        var result = service.GetAllContracts(query);

        // Assert
        Assert.Single(result);
        Assert.Equal("Komerční banka", result.First().Institution);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void GetAllContracts_ShouldReturnAll_WhenSearchIsNullOrWhitespace(string? search)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new ContractQuery { Search = search };

        // Act
        var result = service.GetAllContracts(query);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Theory]
    [InlineData("signedDateAsc", 2024)]
    [InlineData("invalid_fallback", 2026)]
    public void GetAllContracts_ShouldOrderBySignedDate_WhenSortByIsSignedDate(string sortOrder, int expectedFirstYear)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllContracts(query).ToList();

        // Assert
        Assert.Equal(expectedFirstYear, result[0].SignedDate.Year);
    }

    [Theory]
    [InlineData("registrationNumber", "2024/099")]
    [InlineData("registrationNumberDesc", "2026/002")]
    public void GetAllContracts_ShouldOrderByRegistrationNumber_WhenSortByIsRegistrationNumber(string sortOrder, string expectedFirstRegNum)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllContracts(query).ToList();

        // Assert
        Assert.Equal(expectedFirstRegNum, result[0].RegistrationNumber);
    }

    [Theory]
    [InlineData("institution", "ČSOB")]
    [InlineData("institutionDesc", "Kooperativa")]
    public void GetAllContracts_ShouldOrderByInstitution_WhenSortByIsInstitution(string sortOrder, string expectedFirstInstitution)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllContracts(query).ToList();

        // Assert
        Assert.Equal(expectedFirstInstitution, result[0].Institution);
    }

    [Theory]
    [InlineData("effectiveDate", 2024)]
    [InlineData("effectiveDateDesc", 2026)]
    public void GetAllContracts_ShouldOrderByEffectiveDate_WhenSortByIsEffectiveDate(string sortOrder, int expectedFirstYear)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllContracts(query).ToList();

        // Assert
        Assert.Equal(expectedFirstYear, result[0].EffectiveDate.Year);
    }

    [Theory]
    [InlineData("client", "Běžný")]
    [InlineData("clientDesc", "Bývalý")]
    public void GetAllContracts_ShouldOrderByClient_WhenSortByIsClient(string sortOrder, string expectedFirstClientLastName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllContracts(query).ToList();

        // Assert
        Assert.Equal(expectedFirstClientLastName, result[0].Client!.LastName);
    }

    [Theory]
    [InlineData("manager", "Bývalá-Správcová")]
    [InlineData("managerDesc", "Obojí")]
    public void GetAllContracts_ShouldOrderByManager_WhenSortByIsManager(string sortOrder, string expectedFirstManagerLastName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllContracts(query).ToList();

        // Assert
        Assert.Equal(expectedFirstManagerLastName, result[0].Manager!.LastName);
    }
}
