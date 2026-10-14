using Microsoft.AspNetCore.Mvc;

namespace DishManagement.Controllers;

public class HomeController : AppController
{
    public IActionResult Index() => RedirectToAction("Index", "Dishes");

    public IActionResult HttpStatus(int code)
    {
        if (code == 404) return NotFoundPage("err.pageNotFound");
        Response.StatusCode = code;
        return View("Error");
    }

    public IActionResult Error()
    {
        Response.StatusCode = 500;
        return View();
    }
}
