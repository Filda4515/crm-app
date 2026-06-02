using CrmApp.Controllers;
using CrmApp.Models;
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
            EffectiveDate = DateTime.Today
        };
    }

    [Fact]
    public void Index_ShouldReturnViewWithModel_WhenCalled()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var mockClient = new Mock<IClientService>();
        var mockAdvisor = new Mock<IAdvisorService>();

        mockContract.Setup(s => s.GetAllContracts()).Returns([GetValidContract()]);
        var controller = CreateController(mockContract, mockClient, mockAdvisor);

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<IEnumerable<Contract>>(viewResult.ViewData.Model, exactMatch: false);
        Assert.Single(model);
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
    public void Create_Post_ShouldRedirectToIndex_WhenModelStateIsValid()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var controller = CreateController(mockContract, new Mock<IClientService>(), new Mock<IAdvisorService>());
        var newContract = GetValidContract();

        // Act
        var result = controller.Create(newContract);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ContractsController.Index), redirectToActionResult.ActionName);
        mockContract.Verify(s => s.CreateContract(It.IsAny<Contract>()), Times.Once);
    }

    [Fact]
    public void Create_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var mockClient = new Mock<IClientService>();
        var mockAdvisor = new Mock<IAdvisorService>();
        var controller = CreateController(mockContract, mockClient, mockAdvisor);

        controller.ModelState.AddModelError("RegistrationNumber", "Číslo je povinné.");
        var invalidContract = new Contract { Institution = "ČSOB", ClientId = 1, ManagerId = 0, RegistrationNumber = "" };

        // Act
        var result = controller.Create(invalidContract);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(invalidContract, viewResult.Model);
        mockContract.Verify(s => s.CreateContract(It.IsAny<Contract>()), Times.Never);
        mockClient.Verify(s => s.GetAllClients(), Times.Once);
        mockAdvisor.Verify(s => s.GetAllAdvisors(), Times.Once);
    }

    [Fact]
    public void Edit_ShouldReturnViewWithModel_WhenContractExists()
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
        Assert.Equal(existingContract, viewResult.Model);
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
        var updatedContract = GetValidContract();

        // Act
        var result = controller.Edit(1, updatedContract);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ContractsController.Index), redirectToActionResult.ActionName);
        mockContract.Verify(s => s.UpdateContract(updatedContract), Times.Once);
    }

    [Fact]
    public void Edit_Post_ShouldReturnNotFound_WhenIdMismatchesModelId()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var controller = CreateController(mockContract, new Mock<IClientService>(), new Mock<IAdvisorService>());
        var tamperedContract = GetValidContract();
        tamperedContract.Id = 2;

        // Act
        var result = controller.Edit(1, tamperedContract);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockContract.Verify(s => s.UpdateContract(It.IsAny<Contract>()), Times.Never);
    }

    [Fact]
    public void Edit_Post_ShouldReturnNotFound_WhenIdMismatchesAndModelStateIsInvalid()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var controller = CreateController(mockContract, new Mock<IClientService>(), new Mock<IAdvisorService>());
        controller.ModelState.AddModelError("Institution", "Chyba.");

        var tamperedInvalidContract = GetValidContract();
        tamperedInvalidContract.Id = 2;
        tamperedInvalidContract.Institution = "";

        // Act
        var result = controller.Edit(1, tamperedInvalidContract);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockContract.Verify(s => s.UpdateContract(It.IsAny<Contract>()), Times.Never);
    }

    [Fact]
    public void Edit_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockContract = new Mock<IContractService>();
        var mockClient = new Mock<IClientService>();
        var mockAdvisor = new Mock<IAdvisorService>();
        var controller = CreateController(mockContract, mockClient, mockAdvisor);

        controller.ModelState.AddModelError("Institution", "Chyba.");
        var invalidContract = GetValidContract();
        invalidContract.Institution = "";

        // Act
        var result = controller.Edit(1, invalidContract);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(invalidContract, viewResult.Model);
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
}
