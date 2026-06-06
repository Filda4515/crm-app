using System.Diagnostics;

using CrmApp.Models.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace CrmApp.Controllers;

public class ErrorsController : Controller
{
    [Route("Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        var vm = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        return View("Error", vm);
    }
}
