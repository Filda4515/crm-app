using CrmApp.Application.Interfaces;
using CrmApp.Application.Models.Queries;
using CrmApp.Application.Services;
using CrmApp.Domain.Models;

using Moq;

namespace CrmApp.Tests.Services;

public class AdvisorServiceTests
{
    private static (AdvisorService Service, Mock<IAdvisorRepository> MockRepo) CreateService()
    {
        var mockRepo = new Mock<IAdvisorRepository>();
        var service = new AdvisorService(mockRepo.Object);
        return (service, mockRepo);
    }

    private static List<Advisor> GetSampleAdvisors() =>
    [
        new() { Id = 1, FirstName = "Petr", LastName = "Obojí", BirthNumber = "850202/5678" },
        new() { Id = 2, FirstName = "Karel", LastName = "Účastník", BirthNumber = "920303/9999" },
        new() { Id = 3, FirstName = "Pavel", LastName = "Prázdný", BirthNumber = "900101/0000" },
        new() { Id = 4, FirstName = "Žaneta", LastName = "Bývalá-Správcová", BirthNumber = "980808/8888" }
    ];

    [Fact]
    public async Task GetAllAdvisors_ShouldPassQueryToRepository_WhenCalled()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var query = new PersonQuery { Search = "Petr" };
        var expectedAdvisors = GetSampleAdvisors();
        mockRepo.Setup(r => r.GetAllAsync(query)).ReturnsAsync(expectedAdvisors);

        // Act
        var result = await service.GetAllAdvisors(query);

        // Assert
        Assert.Equal(4, result.Count());
        mockRepo.Verify(r => r.GetAllAsync(query), Times.Once);
    }

    [Fact]
    public async Task GetAdvisorById_ShouldReturnAdvisor_WhenExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var expectedAdvisor = GetSampleAdvisors().First();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedAdvisor);

        // Act
        var result = await service.GetAdvisorById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetAdvisorById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Advisor?)null);

        // Act
        var result = await service.GetAdvisorById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAdvisor_ShouldAddAndSave_WhenCalled()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var newAdvisor = new Advisor { Id = 5, FirstName = "Nový", LastName = "Poradce", BirthNumber = "000000/0000" };

        // Act
        await service.CreateAdvisor(newAdvisor);

        // Assert
        mockRepo.Verify(r => r.AddAsync(newAdvisor), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAdvisor_ShouldUpdateAndSave_WhenExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var existingAdvisor = GetSampleAdvisors().First(a => a.Id == 1);
        var updatedAdvisor = new Advisor { Id = 1, FirstName = "Změněný", LastName = "Obojí", BirthNumber = "850202/5678" };

        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingAdvisor);

        // Act
        await service.UpdateAdvisor(updatedAdvisor);

        // Assert
        mockRepo.Verify(r => r.Update(updatedAdvisor, existingAdvisor), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAdvisor_ShouldThrowKeyNotFoundException_WhenNotExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var nonExistentAdvisor = new Advisor { Id = 999, FirstName = "Duch", LastName = "Poradce", BirthNumber = "000000/0000" };
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Advisor?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAdvisor(nonExistentAdvisor));
        mockRepo.Verify(r => r.Update(It.IsAny<Advisor>(), It.IsAny<Advisor>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAdvisor_ShouldDeleteAndSave_WhenExistsAndNoContractsDeleted()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var advisor = GetSampleAdvisors().First(a => a.Id == 1);
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(advisor);

        // Act
        await service.DeleteAdvisor(1, false);

        // Assert
        mockRepo.Verify(r => r.DeleteRange(It.IsAny<IEnumerable<Contract>>()), Times.Never);
        mockRepo.Verify(r => r.Delete(advisor), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAdvisor_ShouldNotDelete_WhenNotExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Advisor?)null);

        // Act
        await service.DeleteAdvisor(999, true);

        // Assert
        mockRepo.Verify(r => r.Delete(It.IsAny<Advisor>()), Times.Never);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAdvisor_ShouldDeleteContractsAndAdvisor_WhenDeleteContractsIsTrueAndHasContracts()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var advisor = GetSampleAdvisors().First(a => a.Id == 1);
        var contracts = new List<Contract>
        {
            new() { Id = 1, RegistrationNumber = "001", Institution = "Banka", ClientId = 1, ManagerId = 1 }
        };
        advisor.ManagedContracts = contracts;

        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(advisor);

        // Act
        await service.DeleteAdvisor(1, true);

        // Assert
        mockRepo.Verify(r => r.DeleteRange(contracts), Times.Once);
        mockRepo.Verify(r => r.Delete(advisor), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
