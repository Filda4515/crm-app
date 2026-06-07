using CrmApp.Data;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Services;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Tests.Services;

public class AdvisorServiceTests
{
    private static CancellationToken CT => TestContext.Current.CancellationToken;

    private static CrmDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new CrmDbContext(options);
    }

    private static AdvisorService CreateService(CrmDbContext context)
    {
        return new AdvisorService(context);
    }

    private static List<Advisor> GetSampleAdvisors() =>
    [
        new() { Id = 1, FirstName = "Petr", LastName = "Obojí", BirthNumber = "850202/5678" },
        new() { Id = 2, FirstName = "Karel", LastName = "Účastník", BirthNumber = "920303/9999" },
        new() { Id = 3, FirstName = "Pavel", LastName = "Prázdný", BirthNumber = "900101/0000" },
        new() { Id = 4, FirstName = "Žaneta", LastName = "Bývalá-Správcová", BirthNumber = "980808/8888" }
    ];

    [Fact]
    public async Task GetAllAdvisors_ShouldReturnAll_WhenQueryIsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        var result = await service.GetAllAdvisors(null);

        // Assert
        Assert.Equal(4, result.Count());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetAllAdvisors_ShouldReturnAll_WhenSearchIsNullOrWhitespace(string? search)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new PersonQuery { Search = search };

        // Act
        var result = await service.GetAllAdvisors(query);

        // Assert
        Assert.Equal(4, result.Count());
    }

    [Fact]
    public async Task GetAllAdvisors_ShouldReturnMatching_WhenSearchMatchesLastName()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new PersonQuery { Search = "Obojí" };

        // Act
        var result = await service.GetAllAdvisors(query);

        // Assert
        Assert.Single(result);
        Assert.Equal("Obojí", result.First().LastName);
    }

    [Theory]
    [InlineData("firstName", "Karel")]
    [InlineData("firstNameDesc", "Žaneta")]
    public async Task GetAllAdvisors_ShouldOrderByFirstName_WhenSortByIsFirstName(string sortOrder, string expectedFirstFirstName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllAdvisors(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstFirstName, result[0].FirstName);
    }

    [Theory]
    [InlineData("lastNameDesc", "Účastník")]
    [InlineData("invalid", "Bývalá-Správcová")]
    public async Task GetAllAdvisors_ShouldOrderByName_WhenSortByIsLastName(string sortOrder, string expectedFirstLastName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllAdvisors(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstLastName, result[0].LastName);
    }

    [Theory]
    [InlineData("birthNumber", "850202/5678")]
    [InlineData("birthNumberDesc", "980808/8888")]
    public async Task GetAllAdvisors_ShouldOrderByBirthNumber_WhenSortByIsBirthNumber(string sortOrder, string expectedFirstBirthNumber)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllAdvisors(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstBirthNumber, result[0].BirthNumber);
    }

    [Fact]
    public async Task GetAdvisorById_ShouldReturnAdvisor_WhenAdvisorExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        var result = await service.GetAdvisorById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetAdvisorById_ShouldReturnNull_WhenAdvisorDoesNotExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        var result = await service.GetAdvisorById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAdvisor_ShouldAddAdvisorToDatabase_WhenCalled()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = CreateService(context);
        var newAdvisor = new Advisor { Id = 1, FirstName = "Nový", LastName = "Poradce", BirthNumber = "000000/0000" };

        // Act
        await service.CreateAdvisor(newAdvisor);

        // Assert
        Assert.Equal(1, await context.Advisors.CountAsync(CT));
        var dbAdvisor = await context.Advisors.FirstAsync(CT);
        Assert.Equal("Nový", dbAdvisor.FirstName);
    }

    [Fact]
    public async Task UpdateAdvisor_ShouldModifyDatabase_WhenAdvisorExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var advisor = new Advisor { Id = 1, FirstName = "Starý", LastName = "Poradce", BirthNumber = "000000/0000" };
        context.Advisors.Add(advisor);
        await context.SaveChangesAsync(CT);

        var service = CreateService(context);
        var updatedAdvisor = new Advisor { Id = 1, FirstName = "Změněný", LastName = "Poradce", BirthNumber = "000000/0000" };

        // Act
        await service.UpdateAdvisor(updatedAdvisor);

        // Assert
        var dbAdvisor = await context.Advisors.FirstAsync(CT);
        Assert.Equal("Změněný", dbAdvisor.FirstName);
    }

    [Fact]
    public async Task UpdateAdvisor_ShouldThrowKeyNotFoundException_WhenAdvisorDoesNotExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = CreateService(context);
        var nonExistentAdvisor = new Advisor { Id = 999, FirstName = "Duch", LastName = "Poradce", BirthNumber = "000000/0000" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAdvisor(nonExistentAdvisor));
    }

    [Fact]
    public async Task DeleteAdvisor_ShouldRemoveAdvisor_WhenAdvisorExistsAndHasNoContracts()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.Add(new Advisor { Id = 1, FirstName = "Petr", LastName = "Obojí", BirthNumber = "850202/5678" });
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        await service.DeleteAdvisor(1, false);

        // Assert
        Assert.Empty(await context.Advisors.ToListAsync(CT));
    }

    [Fact]
    public async Task DeleteAdvisor_ShouldNotThrow_WhenAdvisorDoesNotExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = CreateService(context);

        // Act
        await service.DeleteAdvisor(999, false);

        // Assert
        Assert.Empty(await context.Advisors.ToListAsync(CT));
    }

    [Fact]
    public async Task DeleteAdvisor_ShouldRemoveContracts_WhenDeleteContractsIsTrueAndHasContracts()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var advisor = new Advisor { Id = 1, FirstName = "Petr", LastName = "Obojí", BirthNumber = "850202/5678" };
        var contract = new Contract
        {
            Id = 1,
            RegistrationNumber = "001",
            Institution = "Banka",
            ClientId = 1,
            ManagerId = 1,
            Manager = advisor,
            SignedDate = DateTime.Now,
            EffectiveDate = DateTime.Now
        };

        context.Advisors.Add(advisor);
        context.Contracts.Add(contract);
        await context.SaveChangesAsync(CT);

        var service = CreateService(context);

        // Act
        await service.DeleteAdvisor(1, true);

        // Assert
        Assert.Empty(await context.Advisors.ToListAsync(CT));
        Assert.Empty(await context.Contracts.ToListAsync(CT));
    }
}
