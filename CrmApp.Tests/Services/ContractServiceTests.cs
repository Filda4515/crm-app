using CrmApp.Application.Interfaces;
using CrmApp.Application.Models.Queries;
using CrmApp.Application.Services;
using CrmApp.Domain.Models;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace CrmApp.Tests.Services;

public class ContractServiceTests
{
    private static (ContractService Service, Mock<IContractRepository> MockRepo) CreateService()
    {
        var mockRepo = new Mock<IContractRepository>();
        var service = new ContractService(mockRepo.Object);
        return (service, mockRepo);
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
    public async Task GetAllContracts_ShouldPassQueryToRepository_WhenCalled()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var query = new ContractQuery { Search = "ČSOB" };
        var expectedContracts = GetSampleContracts();
        mockRepo.Setup(r => r.GetAllAsync(query)).ReturnsAsync(expectedContracts);

        // Act
        var result = await service.GetAllContracts(query);

        // Assert
        Assert.Equal(3, result.Count());
        mockRepo.Verify(r => r.GetAllAsync(query), Times.Once);
    }

    [Fact]
    public async Task GetContractById_ShouldReturnContract_WhenExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var expectedContract = GetSampleContracts().First();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedContract);

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
        var (service, mockRepo) = CreateService();
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Contract?)null);

        // Act
        var result = await service.GetContractById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateContract_ShouldAddAndSave_WhenParticipantsAreNull()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var contract = new Contract { Id = 4, RegistrationNumber = "NEW/001", Institution = "Test Banka", Participants = null };

        // Act
        await service.CreateContract(contract);

        // Assert
        mockRepo.Verify(r => r.AddAsync(contract), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateContract_ShouldUpdateAndSave_WhenExistsAndParticipantsNull()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var existingContract = GetSampleContracts().First(c => c.Id == 1);
        var updatedContract = new Contract { Id = 1, RegistrationNumber = "UPDATED/001", Institution = "Nová Banka", Participants = null };

        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingContract);

        // Act
        await service.UpdateContract(updatedContract);

        // Assert
        mockRepo.Verify(r => r.Update(updatedContract, existingContract), Times.Once);
        mockRepo.Verify(r => r.GetAdvisorsByIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateContract_ShouldClearAndReassignParticipants_WhenParticipantsNotNull()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var existingContract = GetSampleContracts().First(c => c.Id == 1);
        existingContract.Participants = [new Advisor { Id = 1, FirstName = "Petr", LastName = "Obojí", BirthNumber = "850202/5678" }];

        var newParticipant = new Advisor { Id = 2, FirstName = "Karel", LastName = "Účastník", BirthNumber = "920303/9999" };
        var updatedContract = new Contract { Id = 1, RegistrationNumber = "2026/001", Institution = "ČSOB", Participants = [newParticipant] };
        var fetchedAdvisors = new List<Advisor> { newParticipant };

        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingContract);
        mockRepo.Setup(r => r.GetAdvisorsByIdsAsync(It.Is<IEnumerable<int>>(ids => ids.Contains(2)))).ReturnsAsync(fetchedAdvisors);

        // Act
        await service.UpdateContract(updatedContract);

        // Assert
        mockRepo.Verify(r => r.Update(updatedContract, existingContract), Times.Once);
        Assert.Single(existingContract.Participants);
        Assert.Equal(2, existingContract.Participants.First().Id);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateContract_ShouldThrowKeyNotFoundException_WhenNotExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var contract = new Contract { Id = 999, RegistrationNumber = "GHOST", Institution = "Nic" };
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Contract?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateContract(contract));
        mockRepo.Verify(r => r.Update(It.IsAny<Contract>(), It.IsAny<Contract>()), Times.Never);
    }

    [Fact]
    public async Task DeleteContract_ShouldDeleteAndSave_WhenExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var contract = GetSampleContracts().First(c => c.Id == 1);
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);

        // Act
        await service.DeleteContract(1);

        // Assert
        mockRepo.Verify(r => r.Delete(contract), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteContract_ShouldNotDelete_WhenNotExists()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Contract?)null);

        // Act
        await service.DeleteContract(999);

        // Assert
        mockRepo.Verify(r => r.Delete(It.IsAny<Contract>()), Times.Never);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
