using CrmApp.Extensions;
using CrmApp.Models;
using CrmApp.Models.Queries;
using CrmApp.Models.ViewModels;
using CrmApp.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmApp.Controllers;

[Authorize]
public class ContractsController(IContractService contractService, IClientService clientService, IAdvisorService advisorService) : Controller
{
    // GET: ContractsController
    public ActionResult Index(ContractQuery query)
    {
        var vm = new ContractIndexViewModel
        {
            Contracts = contractService.GetAllContracts(query),
            Query = query
        };
        return View(vm);
    }

    // GET: ContractsController/Details/5
    public ActionResult Details(int id)
    {
        var contract = contractService.GetContractById(id);
        return contract == null ? NotFound() : View(contract);
    }

    // GET: ContractsController/Create
    public ActionResult Create()
    {
        ViewBag.Clients = clientService.GetAllClients();
        ViewBag.Advisors = advisorService.GetAllAdvisors();

        var vm = new ContractFormViewModel
        {
            RegistrationNumber = "",
            Institution = ""
        };

        return View(vm);
    }

    // POST: ContractsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ContractFormViewModel vm)
    {
        var allAdvisors = advisorService.GetAllAdvisors();

        if (!ModelState.IsValid)
        {
            ViewBag.Clients = clientService.GetAllClients();
            ViewBag.Advisors = allAdvisors;
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

        contractService.CreateContract(newContract);
        return RedirectToAction(nameof(Index));
    }

    // GET: ContractsController/Edit/5
    public ActionResult Edit(int id)
    {
        var contract = contractService.GetContractById(id);
        if (contract == null)
        {
            return NotFound();
        }

        ViewBag.Clients = clientService.GetAllClients();
        ViewBag.Advisors = advisorService.GetAllAdvisors();

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
            ParticipantIds = [.. contract.Participants.Select(p => p.Id)]
        };

        return View(vm);
    }

    // POST: ContractsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, ContractFormViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        var allAdvisors = advisorService.GetAllAdvisors();

        if (!ModelState.IsValid)
        {
            ViewBag.Clients = clientService.GetAllClients();
            ViewBag.Advisors = allAdvisors;
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

        contractService.UpdateContract(updatedContract);
        return RedirectToAction(nameof(Index));
    }

    // POST: ContractsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id)
    {
        contractService.DeleteContract(id);
        return RedirectToAction(nameof(Index));
    }

    // GET: ContractsController/ExportCsv
    public ActionResult ExportCsv(ContractQuery query)
    {
        var contracts = contractService.GetAllContracts(query);

        var sb = new System.Text.StringBuilder();
        sb.Append("Evidenční číslo;Instituce;Klient;Správce;Datum podpisu;Platnost od;Platnost do\r\n");

        foreach (var c in contracts)
        {
            var clientName = c.Client != null ? $"{c.Client.FirstName} {c.Client.LastName}".EscapeCsv() : "";
            var managerName = c.Manager != null ? $"{c.Manager.FirstName} {c.Manager.LastName}".EscapeCsv() : "";
            var endDate = c.EndDate?.ToShortDateString() ?? "";

            sb.Append($"{c.RegistrationNumber.EscapeCsv()};{c.Institution.EscapeCsv()};{clientName};{managerName};{c.SignedDate.ToShortDateString()};{c.EffectiveDate.ToShortDateString()};{endDate}\r\n");
        }

        var bom = System.Text.Encoding.UTF8.GetPreamble();
        var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        var fileBytes = bom.Concat(bytes).ToArray();

        return File(fileBytes, "text/csv", "smlouvy.csv");
    }
}
