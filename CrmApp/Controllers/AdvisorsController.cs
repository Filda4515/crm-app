using CrmApp.Extensions;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Models.ViewModels;
using CrmApp.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrmApp.Controllers;

public class AdvisorsController(IAdvisorService advisorService) : Controller
{
    // GET: AdvisorsController
    public ActionResult Index(PersonQuery query)
    {
        var vm = new AdvisorIndexViewModel
        {
            Advisors = advisorService.GetAllAdvisors(query),
            Query = query
        };
        return View(vm);
    }

    // GET: AdvisorsController/Details/5
    public ActionResult Details(int id)
    {
        var advisor = advisorService.GetAdvisorById(id);
        return advisor == null ? NotFound() : View(advisor);
    }

    // GET: AdvisorsController/Create
    public ActionResult Create()
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
    public ActionResult Create(AdvisorFormViewModel vm)
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
            BirthNumber = vm.BirthNumber,
            Age = vm.BirthNumber.GetAge() ?? 0
        };

        advisorService.CreateAdvisor(newAdvisor);
        return RedirectToAction(nameof(Index));
    }

    // GET: AdvisorsController/Edit/5
    public ActionResult Edit(int id)
    {
        var advisor = advisorService.GetAdvisorById(id);
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
    public ActionResult Edit(int id, AdvisorFormViewModel vm)
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
            BirthNumber = vm.BirthNumber,
            Age = vm.BirthNumber.GetAge() ?? 0
        };

        advisorService.UpdateAdvisor(updatedAdvisor);
        return RedirectToAction(nameof(Index));
    }

    // POST: AdvisorsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, bool deleteContracts = false)
    {
        try
        {
            advisorService.DeleteAdvisor(id, deleteContracts);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "Tohoto poradce nelze smazat, protože stále spravuje smlouvy.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: AdvisorsController/ExportCsv
    public ActionResult ExportCsv(PersonQuery query)
    {
        var advisors = advisorService.GetAllAdvisors(query);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Jméno;Příjmení;Rodné číslo;Věk;E-mail;Telefon");

        foreach (var a in advisors)
        {
            sb.AppendLine($"{a.FirstName};{a.LastName};{a.BirthNumber};{a.Age};{a.Email ?? ""};{a.Phone ?? ""}");
        }

        var bom = System.Text.Encoding.UTF8.GetPreamble();
        var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        var fileBytes = bom.Concat(bytes).ToArray();

        return File(fileBytes, "text/csv", "poradci.csv");
    }
}
