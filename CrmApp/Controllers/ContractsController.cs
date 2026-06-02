using CrmApp.Models;
using CrmApp.Services;

using Microsoft.AspNetCore.Mvc;

namespace CrmApp.Controllers;

public class ContractsController(IContractService contractService, IClientService clientService, IAdvisorService advisorService) : Controller
{
    // GET: ContractsController
    public ActionResult Index()
    {
        var contracts = contractService.GetAllContracts();
        return View(contracts);
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
        return View();
    }

    // POST: ContractsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Contract contract)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Clients = clientService.GetAllClients();
            ViewBag.Advisors = advisorService.GetAllAdvisors();
            return View(contract);
        }

        contractService.CreateContract(contract);
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
        return View(contract);
    }

    // POST: ContractsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, Contract contract)
    {
        if (id != contract.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Clients = clientService.GetAllClients();
            ViewBag.Advisors = advisorService.GetAllAdvisors();
            return View(contract);
        }

        contractService.UpdateContract(contract);
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
}
