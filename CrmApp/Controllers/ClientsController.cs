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
public class ClientsController(IClientService clientService) : Controller
{
    // GET: ClientsController
    public async Task<IActionResult> Index(PersonQuery query)
    {
        var vm = new ClientIndexViewModel
        {
            Clients = await clientService.GetAllClients(query),
            Query = query
        };
        return View(vm);
    }

    // GET: ClientsController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var client = await clientService.GetClientById(id);
        return client == null ? NotFound() : View(client);
    }

    // GET: ClientsController/Create
    public IActionResult Create()
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
    public async Task<IActionResult> Create(ClientFormViewModel vm)
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
            BirthNumber = vm.BirthNumber
        };

        try
        {
            await clientService.CreateClient(newClient);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(vm.BirthNumber), "Osoba s tímto rodným číslem již existuje.");
            return View(vm);
        }
    }

    // GET: ClientsController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var client = await clientService.GetClientById(id);
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
    public async Task<IActionResult> Edit(int id, ClientFormViewModel vm)
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
            BirthNumber = vm.BirthNumber
        };

        try
        {
            await clientService.UpdateClient(updatedClient);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(vm.BirthNumber), "Osoba s tímto rodným číslem již existuje.");
            return View(vm);
        }
    }

    // POST: ClientsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, bool deleteContracts = false)
    {
        try
        {
            await clientService.DeleteClient(id, deleteContracts);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "Tohoto klienta nelze smazat, protože má aktivní smlouvy.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: ClientsController/ExportCsv
    public async Task<IActionResult> ExportCsv(PersonQuery query)
    {
        var clients = await clientService.GetAllClients(query);

        var sb = new System.Text.StringBuilder();
        sb.Append("Jméno;Příjmení;Rodné číslo;Věk;E-mail;Telefon\r\n");

        foreach (var c in clients)
        {
            sb.Append($"{c.FirstName.EscapeCsv()};{c.LastName.EscapeCsv()};{c.BirthNumber.EscapeCsv()};{c.Age};{c.Email.EscapeCsv()};{c.Phone.EscapeCsv()}\r\n");
        }

        return File(sb.GetCsvBytes(), "text/csv", "klienti.csv");
    }
}
