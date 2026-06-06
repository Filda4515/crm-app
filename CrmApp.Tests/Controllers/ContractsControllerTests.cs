using CrmApp.Controllers;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Models.ViewModels;
using CrmApp.Services;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace CrmApp.Tests.Controllers;

public class ContractsControllerTests
{
    private static ContractsController CreateController(Mock<IContractService> mockContract, Mock<IClientService> mockClient, Mock<IAdvisorService> mockAdvisor)
    {
        return new ContractsController(mockContract.Object, mockClient.Object, mockAdvisor.Object);
    }

    private static Contract GetValidContract()
    {
        return new Contract
        {
            Id = 1,
            RegistrationNumber = "2026/001",
            Institution = "ČSOB",
            ClientId = 1,
            ManagerId = 1,
            SignedDate = DateTime.Today,
            EffectiveDate = DateTime.Today,
            Participants = []
        };
    }

    private static ContractFormViewModel GetValidViewModel()
    {
        return new ContractFormViewModel
        {
            Id = 1,
            RegistrationNumber = "2026/001",
            Institution = "ČSOB",
            ClientId = 1,
            ManagerId = 1,
            SignedDate = DateTime.Today,
            EffectiveDate = DateTime.Today,
            ParticipantIds = [1]
        };
    }

    [Fact]
    public void Index_ShouldPassQueryToService_WhenCalled()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var mockClient = new Mock<IClientService>();
        var mockAdvisor = new Mock<IAdvisorService>();

        mockContract.Setup(s => s.GetAllContracts()).Returns([GetValidContract()]);
        var controller = CreateController(mockContract, mockClient, mockAdvisor);
        var query = new ContractQuery { Search = "Testovací dotaz" };

        // Act
        var result = controller.Index(query);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ContractIndexViewModel>(viewResult.Model);
        Assert.NotNull(model.Contracts);
        mockContract.Verify(s => s.GetAllContracts(query), Times.Once);
    }

    [Fact]
    public void Details_ShouldReturnViewWithModel_WhenContractExists()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var contract = GetValidContract();
        mockContract.Setup(s => s.GetContractById(1)).Returns(contract);

        var controller = CreateController(mockContract, new Mock<IClientService>(), new Mock<IAdvisorService>());

        // Act
        var result = controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(contract, viewResult.Model);
    }

    [Fact]
    public void Details_ShouldReturnNotFound_WhenContractDoesNotExist()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        mockContract.Setup(s => s.GetContractById(It.IsAny<int>())).Returns((Contract?)null);

        var controller = CreateController(mockContract, new Mock<IClientService>(), new Mock<IAdvisorService>());

        // Act
        var result = controller.Details(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Create_ShouldReturnView_WhenGetIsCalled()
    {
        // Arrange
        var mockClient = new Mock<IClientService>();
        var mockAdvisor = new Mock<IAdvisorService>();
        var controller = CreateController(new Mock<IContractService>(), mockClient, mockAdvisor);

        // Act
        var result = controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
        mockClient.Verify(s => s.GetAllClients(), Times.Once);
        mockAdvisor.Verify(s => s.GetAllAdvisors(), Times.Once);
    }


    [Fact]
    public void Create_Post_ShouldCallServiceWithPopulatedParticipants()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var mockAdvisor = new Mock<IAdvisorService>();

        var fakeAdvisor = new Advisor { Id = 1, FirstName = "Petr", LastName = "Novák", BirthNumber = "900101/1234" };
        mockAdvisor.Setup(s => s.GetAllAdvisors()).Returns([fakeAdvisor]);

        var controller = CreateController(mockContract, new Mock<IClientService>(), mockAdvisor);
        var newViewModel = GetValidViewModel();

        // Act
        var result = controller.Create(newViewModel);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ContractsController.Index), redirectToActionResult.ActionName);
        mockContract.Verify(s => s.CreateContract(It.Is<Contract>(c => c.Participants.Count == 1 && c.Participants.First().Id == 1)), Times.Once);
    }

    [Fact]
    public void Create_Post_ShouldReturnViewWithViewModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var mockClient = new Mock<IClientService>();
        var mockAdvisor = new Mock<IAdvisorService>();
        var controller = CreateController(mockContract, mockClient, mockAdvisor);

        controller.ModelState.AddModelError("RegistrationNumber", "Číslo je povinné.");

        var invalidViewModel = GetValidViewModel();
        invalidViewModel.RegistrationNumber = "";

        // Act
        var result = controller.Create(invalidViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ContractFormViewModel>(viewResult.Model);
        Assert.Equal(invalidViewModel.Institution, model.Institution);
        mockContract.Verify(s => s.CreateContract(It.IsAny<Contract>()), Times.Never);
        mockClient.Verify(s => s.GetAllClients(), Times.Once);
        mockAdvisor.Verify(s => s.GetAllAdvisors(), Times.Once);
    }

    [Fact]
    public void Edit_ShouldReturnViewWithViewModel_WhenContractExists()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var mockClient = new Mock<IClientService>();
        var mockAdvisor = new Mock<IAdvisorService>();
        var existingContract = GetValidContract();

        mockContract.Setup(s => s.GetContractById(1)).Returns(existingContract);
        var controller = CreateController(mockContract, mockClient, mockAdvisor);

        // Act
        var result = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ContractFormViewModel>(viewResult.Model);
        Assert.Equal(existingContract.Id, model.Id);
        mockClient.Verify(s => s.GetAllClients(), Times.Once);
        mockAdvisor.Verify(s => s.GetAllAdvisors(), Times.Once);
    }

    [Fact]
    public void Edit_ShouldReturnNotFound_WhenContractDoesNotExist()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        mockContract.Setup(s => s.GetContractById(It.IsAny<int>())).Returns((Contract?)null);
        var controller = CreateController(mockContract, new Mock<IClientService>(), new Mock<IAdvisorService>());

        // Act
        var result = controller.Edit(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Edit_Post_ShouldRedirectToIndex_WhenModelAndIdAreValid()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var controller = CreateController(mockContract, new Mock<IClientService>(), new Mock<IAdvisorService>());
        var updatedViewModel = GetValidViewModel();

        // Act
        var result = controller.Edit(1, updatedViewModel);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ContractsController.Index), redirectToActionResult.ActionName);
        mockContract.Verify(s => s.UpdateContract(It.IsAny<Contract>()), Times.Once);
    }

    [Fact]
    public void Edit_Post_ShouldReturnNotFound_WhenIdMismatchesModelId()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var controller = CreateController(mockContract, new Mock<IClientService>(), new Mock<IAdvisorService>());
        var tamperedViewModel = GetValidViewModel();
        tamperedViewModel.Id = 2;

        // Act
        var result = controller.Edit(1, tamperedViewModel);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockContract.Verify(s => s.UpdateContract(It.IsAny<Contract>()), Times.Never);
    }

    [Fact]
    public void Edit_Post_ShouldReturnViewWithViewModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var mockClient = new Mock<IClientService>();
        var mockAdvisor = new Mock<IAdvisorService>();
        var controller = CreateController(mockContract, mockClient, mockAdvisor);

        controller.ModelState.AddModelError("Institution", "Chyba.");

        var invalidViewModel = GetValidViewModel();
        invalidViewModel.Institution = "";

        // Act
        var result = controller.Edit(1, invalidViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ContractFormViewModel>(viewResult.Model);
        Assert.Equal(invalidViewModel.Id, model.Id);
        mockContract.Verify(s => s.UpdateContract(It.IsAny<Contract>()), Times.Never);
        mockClient.Verify(s => s.GetAllClients(), Times.Once);
        mockAdvisor.Verify(s => s.GetAllAdvisors(), Times.Once);
    }

    [Fact]
    public void Delete_Post_ShouldRedirectToIndex_WhenContractDeleted()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var controller = CreateController(mockContract, new Mock<IClientService>(), new Mock<IAdvisorService>());
        int targetId = 1;

        // Act
        var result = controller.Delete(targetId);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ContractsController.Index), redirectToActionResult.ActionName);
        mockContract.Verify(s => s.DeleteContract(targetId), Times.Once);
    }

    [Fact]
    public void ExportCsv_ShouldReturnCsvFileWithCorrectDataAndHandleNulls_WhenCalled()
    {
        // Arrange
        var mockContractService = new Mock<IContractService>();
        var mockClientService = new Mock<IClientService>();
        var mockAdvisorService = new Mock<IAdvisorService>();

        var client = new Client { Id = 1, FirstName = "Jan", LastName = "Běžný", BirthNumber = "960101/1234" };
        var manager = new Advisor { Id = 1, FirstName = "Petr", LastName = "Obojí", BirthNumber = "850202/5678" };

        var sampleContracts = new List<Contract>
        {
            new() {
                Id = 1,
                RegistrationNumber = "2024/099",
                Institution = "Komerční banka",
                Client = client,
                Manager = manager,
                SignedDate = new DateTime(2024, 1, 15),
                EffectiveDate = new DateTime(2024, 2, 1),
                EndDate = new DateTime(2025, 12, 31)
            },
            new() {
                Id = 2,
                RegistrationNumber = "2026/001",
                Institution = "ČSOB",
                Client = client, Manager = manager,
                SignedDate = new DateTime(2026, 6, 7),
                EffectiveDate = new DateTime(2026, 7, 7),
                EndDate = null
            }
        };

        mockContractService.Setup(s => s.GetAllContracts(It.IsAny<ContractQuery>())).Returns(sampleContracts);

        var controller = new ContractsController(mockContractService.Object, mockClientService.Object, mockAdvisorService.Object);
        var query = new ContractQuery();

        // Act
        var result = controller.ExportCsv(query);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.Equal("smlouvy.csv", fileResult.FileDownloadName);

        var fileString = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);

        Assert.Contains("Evidenční číslo;Instituce;Klient;Správce;Datum podpisu;Platnost od;Platnost do", fileString);
        Assert.Contains($"2024/099;Komerční banka;Jan Běžný;Petr Obojí;{new DateTime(2024, 1, 15):d};{new DateTime(2024, 2, 1):d};{new DateTime(2025, 12, 31):d}", fileString);
        Assert.Contains($"2026/001;ČSOB;Jan Běžný;Petr Obojí;{new DateTime(2026, 6, 7):d};{new DateTime(2026, 7, 7):d};", fileString);
    }
}
