using CrmApp.Extensions;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Models.ViewModels;
using CrmApp.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrmApp.Controllers;

public class ClientsController(IClientService clientService) : Controller
{
    // GET: ClientsController
    public ActionResult Index(PersonQuery query)
    {
        var vm = new ClientIndexViewModel
        {
            Clients = clientService.GetAllClients(query),
            Query = query
        };
        return View(vm);
    }

    // GET: ClientsController/Details/5
    public ActionResult Details(int id)
    {
        var client = clientService.GetClientById(id);
        return client == null ? NotFound() : View(client);
    }

    // GET: ClientsController/Create
    public ActionResult Create()
    {
        var vm = new ClientFormViewModel
        {
            FirstName = "",
            LastName = "",
            BirthNumber = ""
        };
        return View(vm);
    }

    // POST: ClientsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ClientFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var newClient = new Client
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            Phone = vm.Phone,
            BirthNumber = vm.BirthNumber,
            Age = vm.BirthNumber.GetAge() ?? 0
        };

        clientService.CreateClient(newClient);
        return RedirectToAction(nameof(Index));
    }

    // GET: ClientsController/Edit/5
    public ActionResult Edit(int id)
    {
        var client = clientService.GetClientById(id);
        if (client == null)
        {
            return NotFound();
        }

        var vm = new ClientFormViewModel
        {
            Id = client.Id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            Phone = client.Phone,
            BirthNumber = client.BirthNumber
        };

        return View(vm);
    }

    // POST: ClientsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, ClientFormViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var updatedClient = new Client
        {
            Id = vm.Id,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            Phone = vm.Phone,
            BirthNumber = vm.BirthNumber,
            Age = vm.BirthNumber.GetAge() ?? 0
        };

        clientService.UpdateClient(updatedClient);
        return RedirectToAction(nameof(Index));
    }

    // POST: ClientsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, bool deleteContracts = false)
    {
        try
        {
            clientService.DeleteClient(id, deleteContracts);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "Tohoto klienta nelze smazat, protože má aktivní smlouvy.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: ClientsController/ExportCsv
    public ActionResult ExportCsv(PersonQuery query)
    {
        var clients = clientService.GetAllClients(query);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Jméno;Příjmení;Rodné číslo;Věk;E-mail;Telefon");

        foreach (var c in clients)
        {
            sb.AppendLine($"{c.FirstName};{c.LastName};{c.BirthNumber};{c.Age};{c.Email ?? ""};{c.Phone ?? ""}");
        }

        var bom = System.Text.Encoding.UTF8.GetPreamble();
        var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        var fileBytes = bom.Concat(bytes).ToArray();

        return File(fileBytes, "text/csv", "klienti.csv");
    }
}
