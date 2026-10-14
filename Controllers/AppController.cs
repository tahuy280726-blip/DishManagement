using Microsoft.AspNetCore.Mvc;

namespace DishManagement.Controllers;

public abstract class AppController : Controller
{
    /// <summary>Trang lỗi 404 thân thiện cho Id không tồn tại.</summary>
    protected IActionResult NotFoundPage(string messageKey)
    {
        Response.StatusCode = 404;
        return View("NotFound", messageKey);
    }
}
