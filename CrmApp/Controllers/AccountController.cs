using System.Security.Claims;

using CrmApp.Models.ViewModels;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CrmApp.Controllers;

public class AccountController(IConfiguration configuration) : Controller
{
    // GET: AccountController/Login
    [HttpGet]
    public IActionResult Login()
    {
        return User.Identity != null && User.Identity.IsAuthenticated ? RedirectToAction("Index", "Clients") : View(new LoginViewModel());
    }

    // POST: AccountController/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var validUser = configuration["AdminCredentials:Username"];
        var validPass = configuration["AdminCredentials:Password"];

        if (model.Username == validUser && model.Password == validPass)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, model.Username),
                new(ClaimTypes.Role, "Administrator")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Clients");
        }

        ModelState.AddModelError(string.Empty, "Neplatné jméno nebo heslo.");
        return View(model);
    }

    // POST: AccountController/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}
