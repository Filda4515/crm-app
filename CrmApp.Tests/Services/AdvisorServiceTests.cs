using CrmApp.Data;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Services;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Tests.Services;

public class AdvisorServiceTests
{
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
        new() { Id = 1, FirstName = "Petr", LastName = "Obojí", BirthNumber = "850202/5678", Age = 41 },
        new() { Id = 2, FirstName = "Karel", LastName = "Účastník", BirthNumber = "920303/9999", Age = 34 },
        new() { Id = 3, FirstName = "Pavel", LastName = "Prázdný", BirthNumber = "900101/0000", Age = 36 },
        new() { Id = 4, FirstName = "Žaneta", LastName = "Bývalá-Správcová", BirthNumber = "980808/8888", Age = 28 }
    ];

    [Fact]
    public void GetAllAdvisors_ShouldReturnAll_WhenQueryIsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        context.SaveChanges();
        var service = CreateService(context);

        // Act
        var result = service.GetAllAdvisors(null);

        // Assert
        Assert.Equal(4, result.Count());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetAllAdvisors_ShouldReturnAll_WhenSearchIsNullOrWhitespace(string? search)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new PersonQuery { Search = search };

        // Act
        var result = service.GetAllAdvisors(query);

        // Assert
        Assert.Equal(4, result.Count());
    }

    [Fact]
    public void GetAllAdvisors_ShouldReturnMatching_WhenSearchMatchesLastName()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new PersonQuery { Search = "Obojí" };

        // Act
        var result = service.GetAllAdvisors(query);

        // Assert
        Assert.Single(result);
        Assert.Equal("Obojí", result.First().LastName);
    }

    [Theory]
    [InlineData("firstName", "Karel")]
    [InlineData("firstNameDesc", "Žaneta")]
    public void GetAllAdvisors_ShouldOrderByFirstName_WhenSortByIsFirstName(string sortOrder, string expectedFirstFirstName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllAdvisors(query).ToList();

        // Assert
        Assert.Equal(expectedFirstFirstName, result[0].FirstName);
    }

    [Theory]
    [InlineData("lastNameDesc", "Účastník")]
    [InlineData("invalid", "Bývalá-Správcová")]
    public void GetAllAdvisors_ShouldOrderByName_WhenSortByIsLastName(string sortOrder, string expectedFirstLastName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllAdvisors(query).ToList();

        // Assert
        Assert.Equal(expectedFirstLastName, result[0].LastName);
    }

    [Theory]
    [InlineData("birthNumber", "850202/5678")]
    [InlineData("birthNumberDesc", "980808/8888")]
    public void GetAllAdvisors_ShouldOrderByBirthNumber_WhenSortByIsBirthNumber(string sortOrder, string expectedFirstBirthNumber)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Advisors.AddRange(GetSampleAdvisors());
        context.SaveChanges();
        var service = CreateService(context);
        var query = new PersonQuery { SortBy = sortOrder };

        // Act
        var result = service.GetAllAdvisors(query).ToList();

        // Assert
        Assert.Equal(expectedFirstBirthNumber, result[0].BirthNumber);
    }
}
