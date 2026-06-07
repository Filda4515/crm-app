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

public class AdvisorsControllerTests
{
    private static AdvisorsController CreateController(Mock<IAdvisorService> mockService)
    {
        return new AdvisorsController(mockService.Object);
    }

    private static Advisor GetValidAdvisor()
    {
        return new Advisor
        {
            Id = 1,
            FirstName = "Petr",
            LastName = "Svoboda",
            BirthNumber = "900101/1234"
        };
    }

    private static AdvisorFormViewModel GetValidViewModel()
    {
        return new AdvisorFormViewModel
        {
            Id = 1,
            FirstName = "Petr",
            LastName = "Svoboda",
            BirthNumber = "900101/1234"
        };
    }

    [Fact]
    public async Task Index_ShouldPassQueryToService_WhenCalled()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        mockService.Setup(s => s.GetAllAdvisors(It.IsAny<PersonQuery>())).ReturnsAsync([GetValidAdvisor()]);
        var controller = CreateController(mockService);
        var query = new PersonQuery { Search = "Testovací dotaz" };

        // Act
        var result = await controller.Index(query);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdvisorIndexViewModel>(viewResult.Model);
        Assert.NotNull(model.Advisors);
        mockService.Verify(s => s.GetAllAdvisors(query), Times.Once);
    }

    [Fact]
    public async Task Details_ShouldReturnViewWithModel_WhenAdvisorExists()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var existingAdvisor = GetValidAdvisor();
        mockService.Setup(s => s.GetAdvisorById(1)).ReturnsAsync(existingAdvisor);
        var controller = CreateController(mockService);

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(existingAdvisor, viewResult.Model);
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        mockService.Setup(s => s.GetAdvisorById(It.IsAny<int>())).ReturnsAsync((Advisor?)null);
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
        var mockService = new Mock<IAdvisorService>();
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
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        var newViewModel = GetValidViewModel();

        // Act
        var result = await controller.Create(newViewModel);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdvisorsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.CreateAdvisor(It.IsAny<Advisor>()), Times.Once);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        controller.ModelState.AddModelError("FirstName", "Jméno je povinné.");
        var invalidViewModel = GetValidViewModel();
        invalidViewModel.FirstName = "";

        // Act
        var result = await controller.Create(invalidViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdvisorFormViewModel>(viewResult.Model);
        Assert.Equal(invalidViewModel.LastName, model.LastName);
        mockService.Verify(s => s.CreateAdvisor(It.IsAny<Advisor>()), Times.Never);
    }

    [Fact]
    public async Task Create_Post_ShouldReturnViewWithModelError_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        mockService.Setup(s => s.CreateAdvisor(It.IsAny<Advisor>())).ThrowsAsync(new DbUpdateException());
        var controller = CreateController(mockService);
        var newViewModel = GetValidViewModel();

        // Act
        var result = await controller.Create(newViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdvisorFormViewModel>(viewResult.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey("BirthNumber"));
    }

    [Fact]
    public async Task Edit_ShouldReturnViewWithViewModel_WhenAdvisorExists()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var existingAdvisor = GetValidAdvisor();
        mockService.Setup(s => s.GetAdvisorById(1)).ReturnsAsync(existingAdvisor);
        var controller = CreateController(mockService);

        // Act
        var result = await controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdvisorFormViewModel>(viewResult.Model);
        Assert.Equal(existingAdvisor.Id, model.Id);
        Assert.Equal(existingAdvisor.FirstName, model.FirstName);
        Assert.Equal(existingAdvisor.LastName, model.LastName);
        Assert.Equal(existingAdvisor.BirthNumber, model.BirthNumber);
    }

    [Fact]
    public async Task Edit_ShouldReturnNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        mockService.Setup(s => s.GetAdvisorById(It.IsAny<int>())).ReturnsAsync((Advisor?)null);
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
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        var updatedViewModel = GetValidViewModel();

        // Act
        var result = await controller.Edit(1, updatedViewModel);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdvisorsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.UpdateAdvisor(It.IsAny<Advisor>()), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnNotFound_WhenIdMismatchesModelId()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        var tamperedViewModel = GetValidViewModel();
        tamperedViewModel.Id = 2;

        // Act
        var result = await controller.Edit(1, tamperedViewModel);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockService.Verify(s => s.UpdateAdvisor(It.IsAny<Advisor>()), Times.Never);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        controller.ModelState.AddModelError("FirstName", "Jméno je povinné.");
        var invalidViewModel = GetValidViewModel();
        invalidViewModel.FirstName = "";

        // Act
        var result = await controller.Edit(1, invalidViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdvisorFormViewModel>(viewResult.Model);
        Assert.Equal(invalidViewModel.Id, model.Id);
        mockService.Verify(s => s.UpdateAdvisor(It.IsAny<Advisor>()), Times.Never);
    }

    [Fact]
    public async Task Edit_Post_ShouldReturnViewWithModelError_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        mockService.Setup(s => s.UpdateAdvisor(It.IsAny<Advisor>())).ThrowsAsync(new DbUpdateException());
        var controller = CreateController(mockService);
        var updatedViewModel = GetValidViewModel();

        // Act
        var result = await controller.Edit(1, updatedViewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdvisorFormViewModel>(viewResult.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey("BirthNumber"));
    }

    [Fact]
    public async Task Delete_Post_ShouldRedirectToIndex_WhenAdvisorDeleted()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        int targetAdvisorId = 1;

        // Act
        var result = await controller.Delete(targetAdvisorId, true);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdvisorsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.DeleteAdvisor(targetAdvisorId, true), Times.Once);
    }

    [Fact]
    public async Task Delete_Post_ShouldSetTempData_WhenDbUpdateExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        mockService.Setup(s => s.DeleteAdvisor(It.IsAny<int>(), false)).ThrowsAsync(new DbUpdateException());
        var controller = CreateController(mockService);
        controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        // Act
        var result = await controller.Delete(1);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdvisorsController.Index), redirectToActionResult.ActionName);
        Assert.NotNull(controller.TempData["ErrorMessage"]);
    }

    [Fact]
    public async Task ExportCsv_ShouldReturnCsvFileWithCorrectDataAndHandleNulls_WhenCalled()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var sampleAdvisors = new List<Advisor>
        {
            new() { Id = 1, FirstName = "Petr", LastName = "Obojí", BirthNumber = "850202/5678", Email = "petr@test.cz", Phone = "+420 987 654 321" },
            new() { Id = 2, FirstName = "Pavel", LastName = "Prázdný", BirthNumber = "900101/0000", Email = null, Phone = null }
        };

        mockService.Setup(s => s.GetAllAdvisors(It.IsAny<PersonQuery>())).ReturnsAsync(sampleAdvisors);

        var controller = CreateController(mockService);
        var query = new PersonQuery();

        // Act
        var result = await controller.ExportCsv(query);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.Equal("poradci.csv", fileResult.FileDownloadName);

        var fileString = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);

        Assert.Contains("Jméno;Příjmení;Rodné číslo;Věk;E-mail;Telefon", fileString);
        Assert.Contains($"Petr;Obojí;850202/5678;{sampleAdvisors[0].Age};petr@test.cz;'+420 987 654 321", fileString);
        Assert.Contains($"Pavel;Prázdný;900101/0000;{sampleAdvisors[1].Age};;", fileString);
    }
}
