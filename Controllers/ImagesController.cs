using DishManagement.Data;
using DishManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DishManagement.Controllers;

public class ImagesController : AppController
{
    private readonly AppDbContext _db;
    public ImagesController(AppDbContext db) => _db = db;

    // Thêm ảnh bằng đường dẫn ImageUrl
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int? dishId, string imageUrl, bool isThumbnail)
    {
        if (dishId == null) return NotFoundPage("err.dishNotFound");

        var dish = await _db.Dishes.Include(d => d.Images).FirstOrDefaultAsync(d => d.Id == dishId.Value);
        if (dish == null) return NotFoundPage("err.dishNotFound");

        imageUrl = imageUrl?.Trim();
        if (string.IsNullOrEmpty(imageUrl))
        {
            TempData["Error"] = "val.image.required";
            return BackToDetails(dish.Id);
        }
        if (!IsValidImageUrl(imageUrl))
        {
            TempData["Error"] = "err.imageUrlInvalid";
            return BackToDetails(dish.Id);
        }

        // Ảnh đầu tiên tự động là ảnh đại diện. Mỗi món tối đa 1 ảnh IsThumbnail = true.
        bool makeThumb = isThumbnail || !dish.Images.Any();
        if (makeThumb)
            foreach (var other in dish.Images) other.IsThumbnail = false;

        dish.Images.Add(new DishImage_BIT240118 { ImageUrl = imageUrl, IsThumbnail = makeThumb });
        await _db.SaveChangesAsync();

        TempData["Success"] = "msg.imageAdded";
        return BackToDetails(dish.Id);
    }

    // Chọn / đổi ảnh đại diện
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetThumbnail(int? id)
    {
        if (id == null) return NotFoundPage("err.imageNotFound");

        var image = await _db.DishImages.FirstOrDefaultAsync(i => i.Id == id.Value);
        if (image == null) return NotFoundPage("err.imageNotFound");

        var others = await _db.DishImages
            .Where(i => i.DishId == image.DishId && i.Id != image.Id && i.IsThumbnail)
            .ToListAsync();
        foreach (var o in others) o.IsThumbnail = false;
        image.IsThumbnail = true;
        await _db.SaveChangesAsync();

        TempData["Success"] = "msg.thumbSet";
        return BackToDetails(image.DishId);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFoundPage("err.imageNotFound");

        var image = await _db.DishImages.FirstOrDefaultAsync(i => i.Id == id.Value);
        if (image == null) return NotFoundPage("err.imageNotFound");

        int dishId = image.DishId;
        bool wasThumb = image.IsThumbnail;
        _db.DishImages.Remove(image);
        await _db.SaveChangesAsync();

        // Nếu xóa ảnh đại diện, tự chọn ảnh còn lại đầu tiên làm ảnh đại diện
        if (wasThumb)
        {
            var next = await _db.DishImages.Where(i => i.DishId == dishId).OrderBy(i => i.Id).FirstOrDefaultAsync();
            if (next != null)
            {
                next.IsThumbnail = true;
                await _db.SaveChangesAsync();
            }
        }

        TempData["Success"] = "msg.imageDeleted";
        return BackToDetails(dishId);
    }

    private IActionResult BackToDetails(int dishId) =>
        RedirectToAction("Details", "Dishes", new { id = dishId }, "images");

    private static bool IsValidImageUrl(string url)
    {
        if (url.Length > 1000) return false;
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        return url.StartsWith("/") && !url.StartsWith("//");
    }
}
