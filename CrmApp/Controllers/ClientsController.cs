using CrmApp.Models;
using CrmApp.Services;

using Microsoft.AspNetCore.Mvc;

namespace CrmApp.Controllers;

public class ClientsController(IClientService clientService) : Controller
{
    // GET: ClientsController
    public ActionResult Index()
    {
        var clients = clientService.GetAllClients();
        return View(clients);
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
        return View();
    }

    // POST: ClientsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Client client)
    {
        if (!ModelState.IsValid)
        {
            return View(client);
        }

        clientService.CreateClient(client);
        return RedirectToAction(nameof(Index));
    }

    // GET: ClientsController/Edit/5
    public ActionResult Edit(int id)
    {
        var client = clientService.GetClientById(id);
        return client == null ? NotFound() : View(client);
    }

    // POST: ClientsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, Client client)
    {
        if (id != client.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(client);
        }

        clientService.UpdateClient(client);
        return RedirectToAction(nameof(Index));
    }

    // POST: ClientsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id)
    {
        clientService.DeleteClient(id);
        return RedirectToAction(nameof(Index));
    }
}
