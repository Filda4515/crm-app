using CrmApp.Controllers;
using CrmApp.Models;
using CrmApp.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace CrmApp.Tests.Controllers;

public class ClientsControllerTests
{
    private static ClientsController CreateController(Mock<IClientService> mockService)
    {
        return new ClientsController(mockService.Object);
    }

    private static Client GetValidClient()
    {
        return new Client
        {
            Id = 1,
            FirstName = "Jan",
            LastName = "Novák",
            BirthNumber = "960101/1234",
            Age = 30
        };
    }

    private static ClientFormViewModel GetValidViewModel()
    {
        return new ClientFormViewModel
        {
            Id = 1,
            FirstName = "Jan",
            LastName = "Novák",
            BirthNumber = "960101/1234"
        };
    }

    [Fact]
    public void Index_ShouldReturnViewWithModel_WhenCalled()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        mockService.Setup(s => s.GetAllClients()).Returns([GetValidClient()]);
        var controller = CreateController(mockService);

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<IEnumerable<Client>>(viewResult.ViewData.Model, exactMatch: false);
        Assert.Single(model);
    }

    [Fact]
    public void Details_ShouldReturnViewWithModel_WhenClientExists()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var existingClient = GetValidClient();
        mockService.Setup(s => s.GetClientById(1)).Returns(existingClient);
        var controller = CreateController(mockService);

        // Act
        var result = controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(existingClient, viewResult.Model);
    }

    [Fact]
    public void Details_ShouldReturnNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        mockService.Setup(s => s.GetClientById(It.IsAny<int>())).Returns((Client?)null);
        var controller = CreateController(mockService);

        // Act
        var result = controller.Details(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Create_ShouldReturnView_WhenGetIsCalled()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);

        // Act
        var result = controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Create_Post_ShouldRedirectToIndex_WhenModelStateIsValid()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        var newViewModel = GetValidViewModel();

        // Act
        var result = controller.Create(newViewModel);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.CreateClient(It.IsAny<Client>()), Times.Once);
    }

    [Fact]
    public void Create_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        controller.ModelState.AddModelError("FirstName", "Jméno je povinné.");

        var invalidViewModel = GetValidViewModel();
        invalidViewModel.FirstName = "";

        // Act
        var result = controller.Create(invalidViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClientFormViewModel>(viewResult.Model);
        Assert.Equal(invalidViewModel.LastName, model.LastName);
        mockService.Verify(s => s.CreateClient(It.IsAny<Client>()), Times.Never);
    }


    [Fact]
    public void Edit_ShouldReturnViewWithModel_WhenClientExists()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var existingClient = GetValidClient();
        mockService.Setup(s => s.GetClientById(1)).Returns(existingClient);
        var controller = CreateController(mockService);

        // Act
        var result = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClientFormViewModel>(viewResult.Model);
        Assert.Equal(existingClient.Id, model.Id);
        Assert.Equal(existingClient.FirstName, model.FirstName);
        Assert.Equal(existingClient.LastName, model.LastName);
        Assert.Equal(existingClient.BirthNumber, model.BirthNumber);
    }

    [Fact]
    public void Edit_ShouldReturnNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        mockService.Setup(s => s.GetClientById(It.IsAny<int>())).Returns((Client?)null);
        var controller = CreateController(mockService);

        // Act
        var result = controller.Edit(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Edit_Post_ShouldRedirectToIndex_WhenModelAndIdAreValid()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        var updatedViewModel = GetValidViewModel();

        // Act
        var result = controller.Edit(1, updatedViewModel);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.UpdateClient(It.IsAny<Client>()), Times.Once);
    }

    [Fact]
    public void Edit_Post_ShouldReturnNotFound_WhenIdMismatchesModelId()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        var tamperedViewModel = GetValidViewModel();
        tamperedViewModel.Id = 2;

        // Act
        var result = controller.Edit(1, tamperedViewModel);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockService.Verify(s => s.UpdateClient(It.IsAny<Client>()), Times.Never);
    }


    [Fact]
    public void Edit_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        controller.ModelState.AddModelError("FirstName", "Jméno je povinné.");
        var invalidViewModel = GetValidViewModel();
        invalidViewModel.FirstName = "";

        // Act
        var result = controller.Edit(1, invalidViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClientFormViewModel>(viewResult.Model);
        Assert.Equal(invalidViewModel.Id, model.Id);
        mockService.Verify(s => s.UpdateClient(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Delete_Post_ShouldRedirectToIndex_WhenClientDeleted()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        int targetClientId = 1;

        // Act
        var result = controller.Delete(targetClientId, true);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.DeleteClient(targetClientId, true), Times.Once);
    }

    [Fact]
    public void Delete_Post_ShouldSetTempData_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        mockService.Setup(s => s.DeleteClient(It.IsAny<int>(), false)).Throws(new DbUpdateException());
        var controller = CreateController(mockService);
        controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        // Act
        var result = controller.Delete(1);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        Assert.NotNull(controller.TempData["ErrorMessage"]);
    }
}
