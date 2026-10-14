using Microsoft.AspNetCore.Mvc;

namespace DishManagement.Controllers;

public class LanguageController : Controller
{
    // GET /Language/Set?lang=vi&returnUrl=/Dishes?Search=pho
    [HttpGet]
    public IActionResult Set(string lang, string returnUrl)
    {
        lang = lang == "vi" ? "vi" : "en";

        Response.Cookies.Append("lang", lang, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });

        // Quay lại đúng trang (kèm query string => giữ điều kiện tìm kiếm/lọc/sắp xếp)
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Dishes");
    }
}
