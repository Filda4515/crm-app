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
public class ContractsController(IContractService contractService, IClientService clientService, IAdvisorService advisorService) : Controller
{
    // GET: ContractsController
    public async Task<IActionResult> Index(ContractQuery query)
    {
        var vm = new ContractIndexViewModel
        {
            Contracts = await contractService.GetAllContracts(query),
            Query = query
        };
        return View(vm);
    }

    // GET: ContractsController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var contract = await contractService.GetContractById(id);
        return contract == null ? NotFound() : View(contract);
    }

    // GET: ContractsController/Create
    public async Task<IActionResult> Create()
    {
        var vm = new ContractFormViewModel
        {
            RegistrationNumber = "",
            Institution = "",
            AvailableClients = await clientService.GetAllClients(),
            AvailableAdvisors = await advisorService.GetAllAdvisors()
        };

        return View(vm);
    }

    // POST: ContractsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContractFormViewModel vm)
    {
        var allAdvisors = await advisorService.GetAllAdvisors();

        if (!ModelState.IsValid)
        {
            vm.AvailableClients = await clientService.GetAllClients();
            vm.AvailableAdvisors = allAdvisors;
            return View(vm);
        }

        var newContract = new Contract
        {
            RegistrationNumber = vm.RegistrationNumber,
            Institution = vm.Institution,
            ClientId = vm.ClientId,
            ManagerId = vm.ManagerId,
            SignedDate = vm.SignedDate,
            EffectiveDate = vm.EffectiveDate,
            EndDate = vm.EndDate,
            Participants = []
        };

        foreach (var p in allAdvisors.Where(a => vm.ParticipantIds.Contains(a.Id)))
        {
            newContract.Participants.Add(p);
        }

        try
        {
            await contractService.CreateContract(newContract);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(vm.RegistrationNumber), "Smlouva s tímto evidenčním číslem již existuje.");
            vm.AvailableClients = await clientService.GetAllClients();
            vm.AvailableAdvisors = allAdvisors;
            return View(vm);
        }
    }

    // GET: ContractsController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var contract = await contractService.GetContractById(id);
        if (contract == null)
        {
            return NotFound();
        }

        var vm = new ContractFormViewModel
        {
            Id = contract.Id,
            RegistrationNumber = contract.RegistrationNumber,
            Institution = contract.Institution,
            ClientId = contract.ClientId,
            ManagerId = contract.ManagerId,
            SignedDate = contract.SignedDate,
            EffectiveDate = contract.EffectiveDate,
            EndDate = contract.EndDate,
            ParticipantIds = [.. contract.Participants.Select(p => p.Id)],
            AvailableClients = await clientService.GetAllClients(),
            AvailableAdvisors = await advisorService.GetAllAdvisors()
        };

        return View(vm);
    }

    // POST: ContractsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContractFormViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        var allAdvisors = await advisorService.GetAllAdvisors();

        if (!ModelState.IsValid)
        {
            vm.AvailableClients = await clientService.GetAllClients();
            vm.AvailableAdvisors = allAdvisors;
            return View(vm);
        }

        var updatedContract = new Contract
        {
            Id = vm.Id,
            RegistrationNumber = vm.RegistrationNumber,
            Institution = vm.Institution,
            ClientId = vm.ClientId,
            ManagerId = vm.ManagerId,
            SignedDate = vm.SignedDate,
            EffectiveDate = vm.EffectiveDate,
            EndDate = vm.EndDate,
            Participants = []
        };

        foreach (var p in allAdvisors.Where(a => vm.ParticipantIds.Contains(a.Id)))
        {
            updatedContract.Participants.Add(p);
        }

        try
        {
            await contractService.UpdateContract(updatedContract);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(vm.RegistrationNumber), "Smlouva s tímto evidenčním číslem již existuje.");
            vm.AvailableClients = await clientService.GetAllClients();
            vm.AvailableAdvisors = allAdvisors;
            return View(vm);
        }
    }

    // POST: ContractsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await contractService.DeleteContract(id);
        return RedirectToAction(nameof(Index));
    }

    // GET: ContractsController/ExportCsv
    public async Task<IActionResult> ExportCsv(ContractQuery query)
    {
        var contracts = await contractService.GetAllContracts(query);

        var sb = new System.Text.StringBuilder();
        sb.Append("Evidenční číslo;Instituce;Klient;Správce;Datum podpisu;Platnost od;Platnost do\r\n");

        foreach (var c in contracts)
        {
            var clientName = c.Client != null ? $"{c.Client.FirstName} {c.Client.LastName}".EscapeCsv() : "";
            var managerName = c.Manager != null ? $"{c.Manager.FirstName} {c.Manager.LastName}".EscapeCsv() : "";
            var endDate = c.EndDate?.ToShortDateString() ?? "";

            sb.Append($"{c.RegistrationNumber.EscapeCsv()};{c.Institution.EscapeCsv()};{clientName};{managerName};{c.SignedDate.ToShortDateString()};{c.EffectiveDate.ToShortDateString()};{endDate}\r\n");
        }

        return File(sb.GetCsvBytes(), "text/csv", "smlouvy.csv");
    }
}
