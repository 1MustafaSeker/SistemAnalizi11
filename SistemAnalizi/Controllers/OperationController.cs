using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class OperationController : Controller
{
    OperationManager operationManager = new OperationManager(new EfOperationDal());

    // Index action
    public IActionResult Index()
    {
        var operations = operationManager.GetList();
        return View(operations);
    }

    // Create action
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Operation operation)
    {
        if (ModelState.IsValid)
        {
            operationManager.TAdd(operation);
            return RedirectToAction(nameof(Index));
        }
        return View(operation);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var operation = operationManager.GetById(id);
        if (operation == null)
        {
            return NotFound();
        }
        return View(operation);
    }

    [HttpPost]
    public IActionResult Edit(Operation operation)
    {
        if (ModelState.IsValid)
        {
            operationManager.TUpdate(operation);
            return RedirectToAction(nameof(Index));
        }
        return View(operation);
    }

    public IActionResult Delete(int id)
    {
        var value = operationManager.GetById(id);
        operationManager.TDelete(value);
        return RedirectToAction("Index");
    }
}