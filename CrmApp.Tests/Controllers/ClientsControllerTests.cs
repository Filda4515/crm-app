using CrmApp.Controllers;
using CrmApp.Models;
using CrmApp.Services;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace CrmApp.Tests.Controllers;

public class ClientsControllerTests
{
    private static ClientsController CreateController(Mock<IClientService> mockService)
    {
        return new ClientsController(mockService.Object);
    }

    [Fact]
    public void Index_ShouldReturnViewWithModel_WhenCalled()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var fakeClients = new List<Client>
        {
            new() { Id = 1, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 }
        };
        mockService.Setup(s => s.GetAllClients()).Returns(fakeClients);
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
        var existingClient = new Client { Id = 1, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 };
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
        var newClient = new Client { FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 };

        // Act
        var result = controller.Create(newClient);

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

        var invalidClient = new Client { FirstName = "", LastName = "Novák", BirthNumber = "" };

        // Act
        var result = controller.Create(invalidClient);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(invalidClient, viewResult.Model);
        mockService.Verify(s => s.CreateClient(It.IsAny<Client>()), Times.Never);
    }


    [Fact]
    public void Edit_ShouldReturnViewWithModel_WhenClientExists()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var existingClient = new Client { Id = 1, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 };
        mockService.Setup(s => s.GetClientById(1)).Returns(existingClient);
        var controller = CreateController(mockService);

        // Act
        var result = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(existingClient, viewResult.Model);
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
        var updatedClient = new Client { Id = 1, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 };

        // Act
        var result = controller.Edit(1, updatedClient);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.UpdateClient(updatedClient), Times.Once);
    }

    [Fact]
    public void Edit_Post_ShouldReturnNotFound_WhenIdMismatchesModelId()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        var tamperedClient = new Client { Id = 2, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 };

        // Act
        var result = controller.Edit(1, tamperedClient);

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
        var invalidClient = new Client { Id = 1, FirstName = "", LastName = "Novák", BirthNumber = "" };

        // Act
        var result = controller.Edit(1, invalidClient);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(invalidClient, viewResult.Model);
        mockService.Verify(s => s.UpdateClient(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Edit_Post_ShouldReturnNotFound_WhenIdMismatchesAndModelStateIsInvalid()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        controller.ModelState.AddModelError("FirstName", "Jméno je povinné.");
        var tamperedInvalidClient = new Client { Id = 2, FirstName = "", LastName = "Novák", BirthNumber = "" };

        // Act
        var result = controller.Edit(1, tamperedInvalidClient);

        // Assert
        Assert.IsType<NotFoundResult>(result);
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
        var result = controller.Delete(targetClientId);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.DeleteClient(targetClientId), Times.Once);
    }
}
