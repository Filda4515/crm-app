using System.Security.Claims;

using CrmApp.Web.Controllers;
using CrmApp.Web.Models;
using CrmApp.Web.Models.ViewModels;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;

using Moq;

namespace CrmApp.Tests.Controllers;

public class AccountControllerTests
{
    private static AccountController CreateController(bool isAuthenticated = false)
    {
        var adminSettings = new AdminSettings
        {
            Username = "username",
            Password = "password"
        };

        var optionsMock = new Mock<IOptions<AdminSettings>>();
        optionsMock.Setup(o => o.Value).Returns(adminSettings);

        var authServiceMock = new Mock<IAuthenticationService>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(s => s.GetService(typeof(IAuthenticationService))).Returns(authServiceMock.Object);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.RequestServices).Returns(serviceProviderMock.Object);

        if (isAuthenticated)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "username") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            httpContextMock.Setup(c => c.User).Returns(new ClaimsPrincipal(identity));
        }
        else
        {
            httpContextMock.Setup(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var urlHelperMock = new Mock<IUrlHelper>();
        var tempDataMock = new Mock<ITempDataDictionary>();

        return new AccountController(optionsMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object },
            Url = urlHelperMock.Object,
            TempData = tempDataMock.Object
        };
    }

    [Fact]
    public void LoginGet_ShouldReturnView_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var controller = CreateController(isAuthenticated: false);

        // Act
        var result = controller.Login();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void LoginGet_ShouldRedirectToClients_WhenUserIsAlreadyAuthenticated()
    {
        // Arrange
        var controller = CreateController(isAuthenticated: true);

        // Act
        var result = controller.Login();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Clients", redirectResult.ControllerName);
    }

    [Fact]
    public async Task LoginPost_ShouldReturnView_WhenModelStateIsInvalid()
    {
        // Arrange
        var controller = CreateController();
        controller.ModelState.AddModelError("Username", "Required");
        var model = new LoginViewModel();

        // Act
        var result = await controller.Login(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Theory]
    [InlineData("username", "spatneheslo")]
    [InlineData("spatnejmeno", "password")]
    [InlineData("spatnejmeno", "spatneheslo")]
    public async Task LoginPost_ShouldReturnViewWithError_WhenCredentialsAreInvalid(string username, string password)
    {
        // Arrange
        var controller = CreateController();
        var model = new LoginViewModel { Username = username, Password = password };

        // Act
        var result = await controller.Login(model);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(string.Empty));
    }

    [Fact]
    public async Task LoginPost_ShouldSignInAndRedirect_WhenCredentialsAreValid()
    {
        // Arrange
        var controller = CreateController();
        var model = new LoginViewModel { Username = "username", Password = "password" };

        // Act
        var result = await controller.Login(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Clients", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Logout_ShouldSignOutAndRedirect_WhenCalled()
    {
        // Arrange
        var controller = CreateController(isAuthenticated: true);

        // Act
        var result = await controller.Logout();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
        Assert.Equal("Account", redirectResult.ControllerName);
    }
}
