using CrmApp.Data;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Services;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Tests.Services;

public class ContractServiceTests
{
    private static CancellationToken CT => TestContext.Current.CancellationToken;

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
    public async Task GetAllContracts_ShouldReturnAll_WhenQueryIsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        var result = await service.GetAllContracts(null);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllContracts_ShouldExcludeExpired_WhenHideInactiveIsTrue()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new ContractQuery { HideInactive = true };

        // Act
        var result = (await service.GetAllContracts(query)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, c => c.RegistrationNumber == "2024/099");
    }

    [Fact]
    public async Task GetAllContracts_ShouldReturnMatching_WhenSearchMatchesInstitution()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new ContractQuery { Search = "Komer" };

        // Act
        var result = await service.GetAllContracts(query);

        // Assert
        Assert.Single(result);
        Assert.Equal("Komerční banka", result.First().Institution);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task GetAllContracts_ShouldReturnAll_WhenSearchIsNullOrWhitespace(string? search)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new ContractQuery { Search = search };

        // Act
        var result = await service.GetAllContracts(query);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Theory]
    [InlineData("signedDate", 2024)]
    [InlineData("signedDateDesc", 2026)]
    public async Task GetAllContracts_ShouldOrderBySignedDate_WhenSortByIsSignedDate(string sortOrder, int expectedFirstYear)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllContracts(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstYear, result[0].SignedDate.Year);
    }

    [Theory]
    [InlineData("registrationNumber", "2024/099")]
    [InlineData("registrationNumberDesc", "2026/002")]
    public async Task GetAllContracts_ShouldOrderByRegistrationNumber_WhenSortByIsRegistrationNumber(string sortOrder, string expectedFirstRegNum)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllContracts(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstRegNum, result[0].RegistrationNumber);
    }

    [Theory]
    [InlineData("institution", "ČSOB")]
    [InlineData("institutionDesc", "Kooperativa")]
    public async Task GetAllContracts_ShouldOrderByInstitution_WhenSortByIsInstitution(string sortOrder, string expectedFirstInstitution)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllContracts(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstInstitution, result[0].Institution);
    }

    [Theory]
    [InlineData("effectiveDate", 2024)]
    [InlineData("effectiveDateDesc", 2026)]
    public async Task GetAllContracts_ShouldOrderByEffectiveDate_WhenSortByIsEffectiveDate(string sortOrder, int expectedFirstYear)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllContracts(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstYear, result[0].EffectiveDate.Year);
    }

    [Theory]
    [InlineData("endDate", "2026/001")]
    [InlineData("endDateDesc", "2026/002")]
    public async Task GetAllContracts_ShouldOrderByEndDate_WhenSortByIsEndDate(string sortOrder, string expectedFirstRegNum)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllContracts(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstRegNum, result[0].RegistrationNumber);
    }

    [Theory]
    [InlineData("client", "Běžný")]
    [InlineData("clientDesc", "Bývalý")]
    public async Task GetAllContracts_ShouldOrderByClient_WhenSortByIsClient(string sortOrder, string expectedFirstClientLastName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllContracts(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstClientLastName, result[0].Client!.LastName);
    }

    [Theory]
    [InlineData("manager", "Bývalá-Správcová")]
    [InlineData("managerDesc", "Obojí")]
    public async Task GetAllContracts_ShouldOrderByManager_WhenSortByIsManager(string sortOrder, string expectedFirstManagerLastName)
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);
        var query = new ContractQuery { SortBy = sortOrder };

        // Act
        var result = (await service.GetAllContracts(query)).ToList();

        // Assert
        Assert.Equal(expectedFirstManagerLastName, result[0].Manager!.LastName);
    }

    [Fact]
    public async Task GetContractById_ShouldReturnContract_WhenExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.AddRange(GetSampleContracts());
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        var result = await service.GetContractById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetContractById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = CreateService(context);

        // Act
        var result = await service.GetContractById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateContract_ShouldAddContract_WhenParticipantsAreNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = CreateService(context);
        var contract = new Contract { Id = 1, RegistrationNumber = "NEW/001", Institution = "Test Banka", Participants = null };

        // Act
        await service.CreateContract(contract);

        // Assert
        Assert.Equal(1, await context.Contracts.CountAsync(CT));
    }

    [Fact]
    public async Task CreateContract_ShouldAddContractAndAttachParticipants_WhenParticipantsExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var advisor = new Advisor { Id = 1, FirstName = "A", LastName = "B", BirthNumber = "111" };
        context.Advisors.Add(advisor);
        await context.SaveChangesAsync(CT);

        var service = CreateService(context);
        var contract = new Contract { Id = 1, RegistrationNumber = "NEW/002", Institution = "Test Banka", Participants = [advisor] };

        // Act
        await service.CreateContract(contract);

        // Assert
        var dbContract = await context.Contracts.Include(c => c.Participants).FirstAsync(CT);
        Assert.Single(dbContract.Participants);
    }

    [Fact]
    public async Task UpdateContract_ShouldModifyContract_WhenParticipantsAreNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var contract = new Contract { Id = 1, RegistrationNumber = "OLD/001", Institution = "Stará Banka" };
        context.Contracts.Add(contract);
        await context.SaveChangesAsync(CT);

        var service = CreateService(context);
        var updatedContract = new Contract { Id = 1, RegistrationNumber = "UPDATED/001", Institution = "Nová Banka", Participants = null };

        // Act
        await service.UpdateContract(updatedContract);

        // Assert
        var dbContract = await context.Contracts.FirstAsync(CT);
        Assert.Equal("UPDATED/001", dbContract.RegistrationNumber);
        Assert.Equal("Nová Banka", dbContract.Institution);
    }

    [Fact]
    public async Task UpdateContract_ShouldModifyContractAndParticipants_WhenParticipantsExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var advisor1 = new Advisor { Id = 1, FirstName = "A", LastName = "B", BirthNumber = "111" };
        var advisor2 = new Advisor { Id = 2, FirstName = "C", LastName = "D", BirthNumber = "222" };
        context.Advisors.AddRange(advisor1, advisor2);
        context.Contracts.Add(new Contract { Id = 1, RegistrationNumber = "OLD", Institution = "Banka", Participants = [advisor1] });
        await context.SaveChangesAsync(CT);

        var service = CreateService(context);
        var updatedContract = new Contract { Id = 1, RegistrationNumber = "UPDATED", Institution = "Banka", Participants = [advisor2] };

        // Act
        await service.UpdateContract(updatedContract);

        // Assert
        var dbContract = await context.Contracts.Include(c => c.Participants).FirstAsync(CT);
        Assert.Single(dbContract.Participants);
        Assert.Equal(2, dbContract.Participants.First().Id);
    }

    [Fact]
    public async Task UpdateContract_ShouldThrowKeyNotFoundException_WhenNotExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = CreateService(context);
        var contract = new Contract { Id = 999, RegistrationNumber = "GHOST", Institution = "Nic" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateContract(contract));
    }

    [Fact]
    public async Task DeleteContract_ShouldRemoveContract_WhenExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Contracts.Add(new Contract { Id = 1, RegistrationNumber = "TO_DELETE", Institution = "Delete Banka" });
        await context.SaveChangesAsync(CT);
        var service = CreateService(context);

        // Act
        await service.DeleteContract(1);

        // Assert
        Assert.Empty(await context.Contracts.ToListAsync(CT));
    }

    [Fact]
    public async Task DeleteContract_ShouldNotThrow_WhenNotExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = CreateService(context);

        // Act
        await service.DeleteContract(999);

        // Assert
        Assert.Empty(await context.Contracts.ToListAsync(CT));
    }
}
