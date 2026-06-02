using CrmApp.Models;
using CrmApp.Services;

using Microsoft.AspNetCore.Mvc;

namespace CrmApp.Controllers;

public class AdvisorsController(IAdvisorService advisorService) : Controller
{
    // GET: AdvisorsController
    public ActionResult Index()
    {
        var advisors = advisorService.GetAllAdvisors();
        return View(advisors);
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
        return View();
    }

    // POST: AdvisorsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Advisor advisor)
    {
        if (!ModelState.IsValid)
        {
            return View(advisor);
        }

        advisorService.CreateAdvisor(advisor);
        return RedirectToAction(nameof(Index));
    }

    // GET: AdvisorsController/Edit/5
    public ActionResult Edit(int id)
    {
        var advisor = advisorService.GetAdvisorById(id);
        return advisor == null ? NotFound() : View(advisor);
    }

    // POST: AdvisorsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, Advisor advisor)
    {
        if (id != advisor.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(advisor);
        }

        advisorService.UpdateAdvisor(advisor);
        return RedirectToAction(nameof(Index));
    }

    // POST: AdvisorsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id)
    {
        advisorService.DeleteAdvisor(id);
        return RedirectToAction(nameof(Index));
    }
}
