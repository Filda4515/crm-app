using CrmApp.Extensions;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Models.ViewModels;
using CrmApp.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrmApp.Controllers;

[Authorize]
public class AdvisorsController(IAdvisorService advisorService) : Controller
{
    // GET: AdvisorsController
    public async Task<IActionResult> Index(PersonQuery query)
    {
        var vm = new AdvisorIndexViewModel
        {
            Advisors = await advisorService.GetAllAdvisors(query),
            Query = query
        };
        return View(vm);
    }

    // GET: AdvisorsController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var advisor = await advisorService.GetAdvisorById(id);
        return advisor == null ? NotFound() : View(advisor);
    }

    // GET: AdvisorsController/Create
    public IActionResult Create()
    {
        var vm = new AdvisorFormViewModel
        {
            FirstName = "",
            LastName = "",
            BirthNumber = ""
        };
        return View(vm);
    }

    // POST: AdvisorsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdvisorFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var newAdvisor = new Advisor
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            Phone = vm.Phone,
            BirthNumber = vm.BirthNumber
        };

        try
        {
            await advisorService.CreateAdvisor(newAdvisor);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(vm.BirthNumber), "Osoba s tímto rodným číslem již existuje.");
            return View(vm);
        }
    }

    // GET: AdvisorsController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var advisor = await advisorService.GetAdvisorById(id);
        if (advisor == null)
        {
            return NotFound();
        }

        var vm = new AdvisorFormViewModel
        {
            Id = advisor.Id,
            FirstName = advisor.FirstName,
            LastName = advisor.LastName,
            Email = advisor.Email,
            Phone = advisor.Phone,
            BirthNumber = advisor.BirthNumber
        };

        return View(vm);
    }

    // POST: AdvisorsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdvisorFormViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var updatedAdvisor = new Advisor
        {
            Id = vm.Id,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            Phone = vm.Phone,
            BirthNumber = vm.BirthNumber
        };

        try
        {
            await advisorService.UpdateAdvisor(updatedAdvisor);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(vm.BirthNumber), "Osoba s tímto rodným číslem již existuje.");
            return View(vm);
        }
    }

    // POST: AdvisorsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, bool deleteContracts = false)
    {
        try
        {
            await advisorService.DeleteAdvisor(id, deleteContracts);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "Tohoto poradce nelze smazat, protože stále spravuje smlouvy.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: AdvisorsController/ExportCsv
    public async Task<IActionResult> ExportCsv(PersonQuery query)
    {
        var advisors = await advisorService.GetAllAdvisors(query);

        var sb = new System.Text.StringBuilder();
        sb.Append("Jméno;Příjmení;Rodné číslo;Věk;E-mail;Telefon\r\n");

        foreach (var a in advisors)
        {
            sb.Append($"{a.FirstName.EscapeCsv()};{a.LastName.EscapeCsv()};{a.BirthNumber.EscapeCsv()};{a.Age};{a.Email.EscapeCsv()};{a.Phone.EscapeCsv()}\r\n");
        }

        return File(sb.GetCsvBytes(), "text/csv", "poradci.csv");
    }
}
