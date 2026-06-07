using CrmApp.Controllers;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Models.ViewModels;
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
            BirthNumber = "960101/1234"
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
    public async Task Index_ShouldPassQueryToService_WhenCalled()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        mockService.Setup(s => s.GetAllClients(It.IsAny<PersonQuery>())).ReturnsAsync([GetValidClient()]);
        var controller = CreateController(mockService);
        var query = new PersonQuery { Search = "Testovací dotaz" };

        // Act
        var result = await controller.Index(query);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClientIndexViewModel>(viewResult.Model);
        Assert.NotNull(model.Clients);
        mockService.Verify(s => s.GetAllClients(query), Times.Once);
    }

    [Fact]
    public async Task Details_ShouldReturnViewWithModel_WhenClientExists()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var existingClient = GetValidClient();
        mockService.Setup(s => s.GetClientById(1)).ReturnsAsync(existingClient);
        var controller = CreateController(mockService);

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(existingClient, viewResult.Model);
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        mockService.Setup(s => s.GetClientById(It.IsAny<int>())).ReturnsAsync((Client?)null);
        var controller = CreateController(mockService);

        // Act
        var result = await controller.Details(999);

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
    public async Task Create_Post_ShouldRedirectToIndex_WhenModelStateIsValid()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        var newViewModel = GetValidViewModel();

        // Act
        var result = await controller.Create(newViewModel);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.CreateClient(It.IsAny<Client>()), Times.Once);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        controller.ModelState.AddModelError("FirstName", "Jméno je povinné.");

        var invalidViewModel = GetValidViewModel();
        invalidViewModel.FirstName = "";

        // Act
        var result = await controller.Create(invalidViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClientFormViewModel>(viewResult.Model);
        Assert.Equal(invalidViewModel.LastName, model.LastName);
        mockService.Verify(s => s.CreateClient(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnViewWithModelError_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        mockService.Setup(s => s.CreateClient(It.IsAny<Client>())).ThrowsAsync(new DbUpdateException());
        var controller = CreateController(mockService);
        var newViewModel = GetValidViewModel();

        // Act
        var result = await controller.Create(newViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClientFormViewModel>(viewResult.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey("BirthNumber"));
    }

    [Fact]
    public async Task Edit_ShouldReturnViewWithModel_WhenClientExists()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var existingClient = GetValidClient();
        mockService.Setup(s => s.GetClientById(1)).ReturnsAsync(existingClient);
        var controller = CreateController(mockService);

        // Act
        var result = await controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClientFormViewModel>(viewResult.Model);
        Assert.Equal(existingClient.Id, model.Id);
        Assert.Equal(existingClient.FirstName, model.FirstName);
        Assert.Equal(existingClient.LastName, model.LastName);
        Assert.Equal(existingClient.BirthNumber, model.BirthNumber);
    }

    [Fact]
    public async Task Edit_ShouldReturnNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        mockService.Setup(s => s.GetClientById(It.IsAny<int>())).ReturnsAsync((Client?)null);
        var controller = CreateController(mockService);

        // Act
        var result = await controller.Edit(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ShouldRedirectToIndex_WhenModelAndIdAreValid()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        var updatedViewModel = GetValidViewModel();

        // Act
        var result = await controller.Edit(1, updatedViewModel);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.UpdateClient(It.IsAny<Client>()), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnNotFound_WhenIdMismatchesModelId()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        var tamperedViewModel = GetValidViewModel();
        tamperedViewModel.Id = 2;

        // Act
        var result = await controller.Edit(1, tamperedViewModel);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockService.Verify(s => s.UpdateClient(It.IsAny<Client>()), Times.Never);
    }


    [Fact]
    public async Task Edit_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        controller.ModelState.AddModelError("FirstName", "Jméno je povinné.");
        var invalidViewModel = GetValidViewModel();
        invalidViewModel.FirstName = "";

        // Act
        var result = await controller.Edit(1, invalidViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClientFormViewModel>(viewResult.Model);
        Assert.Equal(invalidViewModel.Id, model.Id);
        mockService.Verify(s => s.UpdateClient(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnViewWithModelError_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        mockService.Setup(s => s.UpdateClient(It.IsAny<Client>())).ThrowsAsync(new DbUpdateException());
        var controller = CreateController(mockService);
        var updatedViewModel = GetValidViewModel();

        // Act
        var result = await controller.Edit(1, updatedViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClientFormViewModel>(viewResult.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey("BirthNumber"));
    }

    [Fact]
    public async Task Delete_Post_ShouldRedirectToIndex_WhenClientDeleted()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var controller = CreateController(mockService);
        int targetClientId = 1;

        // Act
        var result = await controller.Delete(targetClientId, true);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.DeleteClient(targetClientId, true), Times.Once);
    }

    [Fact]
    public async Task Delete_Post_ShouldSetTempData_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        mockService.Setup(s => s.DeleteClient(It.IsAny<int>(), false)).ThrowsAsync(new DbUpdateException());
        var controller = CreateController(mockService);
        controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        // Act
        var result = await controller.Delete(1);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        Assert.NotNull(controller.TempData["ErrorMessage"]);
    }

    [Fact]
    public async Task ExportCsv_ShouldReturnCsvFileWithCorrectDataAndHandleNulls_WhenCalled()
    {
        // Arrange
        var mockService = new Mock<IClientService>();
        var sampleClients = new List<Client>
        {
            new() { Id = 1, FirstName = "Jan", LastName = "Běžný", BirthNumber = "960101/1234", Email = "jan@test.cz", Phone = "+420 123 456" },
            new() { Id = 2, FirstName = "Alena", LastName = "Prázdná", BirthNumber = "955515/5555", Email = null, Phone = null }
        };

        mockService.Setup(s => s.GetAllClients(It.IsAny<PersonQuery>())).ReturnsAsync(sampleClients);

        var controller = CreateController(mockService);
        var query = new PersonQuery();

        // Act
        var result = await controller.ExportCsv(query);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.Equal("klienti.csv", fileResult.FileDownloadName);

        var fileString = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);

        Assert.Contains("Jméno;Příjmení;Rodné číslo;Věk;E-mail;Telefon", fileString);
        Assert.Contains($"Jan;Běžný;960101/1234;{sampleClients[0].Age};jan@test.cz;'+420 123 456", fileString);
        Assert.Contains($"Alena;Prázdná;955515/5555;{sampleClients[1].Age};;", fileString);
    }
}
