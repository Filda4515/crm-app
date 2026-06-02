using CrmApp.Controllers;
using CrmApp.Models;
using CrmApp.Services;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace CrmApp.Tests.Controllers;

public class AdvisorsControllerTests
{
    private static AdvisorsController CreateController(Mock<IAdvisorService> mockService)
    {
        return new AdvisorsController(mockService.Object);
    }

    [Fact]
    public void Index_ShouldReturnViewWithModel_WhenCalled()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var fakeAdvisors = new List<Advisor>
        {
            new() { Id = 1, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 }
        };
        mockService.Setup(s => s.GetAllAdvisors()).Returns(fakeAdvisors);
        var controller = CreateController(mockService);

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<IEnumerable<Advisor>>(viewResult.ViewData.Model, exactMatch: false);
        Assert.Single(model);
    }

    [Fact]
    public void Details_ShouldReturnViewWithModel_WhenAdvisorExists()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var existingAdvisor = new Advisor { Id = 1, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 };
        mockService.Setup(s => s.GetAdvisorById(1)).Returns(existingAdvisor);
        var controller = CreateController(mockService);

        // Act
        var result = controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(existingAdvisor, viewResult.Model);
    }

    [Fact]
    public void Details_ShouldReturnNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        mockService.Setup(s => s.GetAdvisorById(It.IsAny<int>())).Returns((Advisor?)null);
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
        var mockService = new Mock<IAdvisorService>();
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
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        var newAdvisor = new Advisor { FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 };

        // Act
        var result = controller.Create(newAdvisor);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdvisorsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.CreateAdvisor(It.IsAny<Advisor>()), Times.Once);
    }

    [Fact]
    public void Create_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        controller.ModelState.AddModelError("FirstName", "Jméno je povinné.");
        var invalidAdvisor = new Advisor { FirstName = "", LastName = "Novák", BirthNumber = "" };

        // Act
        var result = controller.Create(invalidAdvisor);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(invalidAdvisor, viewResult.Model);
        mockService.Verify(s => s.CreateAdvisor(It.IsAny<Advisor>()), Times.Never);
    }

    [Fact]
    public void Edit_ShouldReturnViewWithModel_WhenAdvisorExists()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var existingAdvisor = new Advisor { Id = 1, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 };
        mockService.Setup(s => s.GetAdvisorById(1)).Returns(existingAdvisor);
        var controller = CreateController(mockService);

        // Act
        var result = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(existingAdvisor, viewResult.Model);
    }

    [Fact]
    public void Edit_ShouldReturnNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        mockService.Setup(s => s.GetAdvisorById(It.IsAny<int>())).Returns((Advisor?)null);
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
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        var updatedAdvisor = new Advisor { Id = 1, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 };

        // Act
        var result = controller.Edit(1, updatedAdvisor);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdvisorsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.UpdateAdvisor(updatedAdvisor), Times.Once);
    }

    [Fact]
    public void Edit_Post_ShouldReturnNotFound_WhenIdMismatchesModelId()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        var tamperedAdvisor = new Advisor { Id = 2, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 };

        // Act
        var result = controller.Edit(1, tamperedAdvisor);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockService.Verify(s => s.UpdateAdvisor(It.IsAny<Advisor>()), Times.Never);
    }

    [Fact]
    public void Edit_Post_ShouldReturnNotFound_WhenIdMismatchesAndModelStateIsInvalid()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        controller.ModelState.AddModelError("FirstName", "Jméno je povinné.");
        var tamperedInvalidAdvisor = new Advisor { Id = 2, FirstName = "", LastName = "Novák", BirthNumber = "" };

        // Act
        var result = controller.Edit(1, tamperedInvalidAdvisor);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockService.Verify(s => s.UpdateAdvisor(It.IsAny<Advisor>()), Times.Never);
    }

    [Fact]
    public void Edit_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        controller.ModelState.AddModelError("FirstName", "Jméno je povinné.");
        var invalidAdvisor = new Advisor { Id = 1, FirstName = "", LastName = "Novák", BirthNumber = "" };

        // Act
        var result = controller.Edit(1, invalidAdvisor);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(invalidAdvisor, viewResult.Model);
        mockService.Verify(s => s.UpdateAdvisor(It.IsAny<Advisor>()), Times.Never);
    }

    [Fact]
    public void Delete_Post_ShouldRedirectToIndex_WhenAdvisorDeleted()
    {
        // Arrange
        var mockService = new Mock<IAdvisorService>();
        var controller = CreateController(mockService);
        int targetAdvisorId = 1;

        // Act
        var result = controller.Delete(targetAdvisorId);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdvisorsController.Index), redirectToActionResult.ActionName);
        mockService.Verify(s => s.DeleteAdvisor(targetAdvisorId), Times.Once);
    }
}
