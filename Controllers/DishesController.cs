using DishManagement.Data;
using DishManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DishManagement.Controllers;

public class DishesController : AppController
{
    private readonly AppDbContext _db;
    public DishesController(AppDbContext db) => _db = db;

    // ===================== LIST + SEARCH / FILTER / SORT =====================
    public async Task<IActionResult> Index([FromQuery] DishFilter filter)
    {
        filter ??= new DishFilter();

        bool Bad(string field) => ModelState.TryGetValue(field, out var e) && e.Errors.Count > 0;

        // Giá trị nhập sai kiểu (vd: chữ ở ô giá) => đã có thông báo lỗi, bỏ qua điều kiện đó
        if (Bad(nameof(DishFilter.MinPrice))) filter.MinPrice = null;
        if (Bad(nameof(DishFilter.MaxPrice))) filter.MaxPrice = null;
        if (Bad(nameof(DishFilter.CategoryId))) filter.CategoryId = null;

        // Khoảng giá không hợp lệ
        bool negative = (filter.MinPrice.HasValue && filter.MinPrice.Value < 0)
                     || (filter.MaxPrice.HasValue && filter.MaxPrice.Value < 0);
        bool reversed = filter.MinPrice.HasValue && filter.MaxPrice.HasValue
                     && filter.MinPrice.Value > filter.MaxPrice.Value;
        if (negative || reversed)
        {
            ModelState.AddModelError(string.Empty, L.T("err.priceRange"));
            filter.MinPrice = null;
            filter.MaxPrice = null;
        }

        var categories = await _db.DishCategories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

        // Id loại món (trong bộ lọc) không tồn tại
        if (filter.CategoryId.HasValue && !categories.Any(c => c.Id == filter.CategoryId.Value))
        {
            ModelState.AddModelError(string.Empty, L.T("err.filterCategoryNotFound"));
            filter.CategoryId = null;
        }

        // Toàn bộ điều kiện lọc được dịch thành SQL (không tải hết rồi lọc bằng vòng lặp)
        IQueryable<Dish_BIT240118> query = _db.Dishes
            .AsNoTracking()
            .Include(d => d.DishCategory)
            .Include(d => d.Images);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(d => d.Name.Contains(term));
        }

        if (filter.CategoryId.HasValue)
        {
            var catId = filter.CategoryId.Value;
            query = query.Where(d => d.DishCategoryId == catId);
        }

        if (filter.Status == "available") query = query.Where(d => d.IsAvailable);
        else if (filter.Status == "unavailable") query = query.Where(d => !d.IsAvailable);

        if (filter.MinPrice.HasValue)
        {
            var min = filter.MinPrice.Value;
            query = query.Where(d => d.Price >= min);
        }
        if (filter.MaxPrice.HasValue)
        {
            var max = filter.MaxPrice.Value;
            query = query.Where(d => d.Price <= max);
        }

        query = filter.Sort switch
        {
            "price_asc" => query.OrderBy(d => d.Price).ThenBy(d => d.Name),
            "price_desc" => query.OrderByDescending(d => d.Price).ThenBy(d => d.Name),
            "time_asc" => query.OrderBy(d => d.PreparationTime).ThenBy(d => d.Name),
            _ => query.OrderBy(d => d.Name)
        };

        var vm = new DishListViewModel
        {
            Items = await query.ToListAsync(),
            Categories = categories
        };
        return View(vm);
    }

    // ===================== DETAILS =====================
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFoundPage("err.dishNotFound");

        var dish = await _db.Dishes
            .AsNoTracking()
            .Include(d => d.DishCategory)
            .Include(d => d.Images)
            .FirstOrDefaultAsync(d => d.Id == id.Value);

        if (dish == null) return NotFoundPage("err.dishNotFound");

        dish.Images = dish.Images.OrderByDescending(i => i.IsThumbnail).ThenBy(i => i.Id).ToList();
        return View(dish);
    }

    // ===================== CREATE =====================
    public async Task<IActionResult> Create()
    {
        await LoadCategories();
        return View(new Dish_BIT240118 { IsAvailable = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Name,Price,PreparationTime,IsAvailable,Description,DishCategoryId")] Dish_BIT240118 dish)
    {
        await ValidateDishAsync(dish, null);
        if (!ModelState.IsValid)
        {
            await LoadCategories();
            return View(dish);
        }

        _db.Dishes.Add(dish);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, L.T("err.generic"));
            await LoadCategories();
            return View(dish);
        }

        TempData["Success"] = "msg.created";
        return RedirectToAction(nameof(Details), new { id = dish.Id });
    }

    // ===================== EDIT =====================
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFoundPage("err.dishNotFound");

        var dish = await _db.Dishes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id.Value);
        if (dish == null) return NotFoundPage("err.dishNotFound");

        await LoadCategories();
        return View(dish);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id,
        [Bind("Name,Price,PreparationTime,IsAvailable,Description,DishCategoryId")] Dish_BIT240118 dish)
    {
        if (id == null) return NotFoundPage("err.dishNotFound");

        var existing = await _db.Dishes.FirstOrDefaultAsync(d => d.Id == id.Value);
        if (existing == null) return NotFoundPage("err.dishNotFound");

        await ValidateDishAsync(dish, id.Value);
        if (!ModelState.IsValid)
        {
            dish.Id = id.Value;
            await LoadCategories();
            return View(dish);
        }

        existing.Name = dish.Name;
        existing.Price = dish.Price;
        existing.PreparationTime = dish.PreparationTime;
        existing.IsAvailable = dish.IsAvailable;
        existing.Description = dish.Description;
        existing.DishCategoryId = dish.DishCategoryId;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, L.T("err.generic"));
            dish.Id = id.Value;
            await LoadCategories();
            return View(dish);
        }

        TempData["Success"] = "msg.updated";
        return RedirectToAction(nameof(Details), new { id = existing.Id });
    }

    // ===================== DELETE =====================
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFoundPage("err.dishNotFound");

        var dish = await _db.Dishes.FirstOrDefaultAsync(d => d.Id == id.Value);
        if (dish == null) return NotFoundPage("err.dishNotFound");

        _db.Dishes.Remove(dish); // ảnh bị xóa theo (cascade)
        await _db.SaveChangesAsync();

        TempData["Success"] = "msg.deleted";
        return RedirectToAction(nameof(Index));
    }

    // ===================== HELPERS =====================
    private async Task LoadCategories()
    {
        var cats = await _db.DishCategories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        ViewBag.Categories = new SelectList(cats, nameof(DishCategory_BIT240118.Id), nameof(DishCategory_BIT240118.Name));
        ViewBag.HasCategories = cats.Count > 0;
    }

    /// <summary>Kiểm tra: DishCategoryId phải tồn tại; tên món không trùng trong cùng một loại.</summary>
    private async Task ValidateDishAsync(Dish_BIT240118 dish, int? excludeId)
    {
        dish.Name = dish.Name?.Trim();
        dish.Description = string.IsNullOrWhiteSpace(dish.Description) ? null : dish.Description.Trim();

        bool catBad = ModelState[nameof(Dish_BIT240118.DishCategoryId)]?.Errors.Count > 0;
        if (!catBad)
        {
            bool exists = await _db.DishCategories.AnyAsync(c => c.Id == dish.DishCategoryId);
            if (!exists)
            {
                ModelState.AddModelError(nameof(Dish_BIT240118.DishCategoryId), L.T("val.category.notFound"));
                catBad = true;
            }
        }

        bool nameBad = ModelState[nameof(Dish_BIT240118.Name)]?.Errors.Count > 0;
        if (!catBad && !nameBad && !string.IsNullOrEmpty(dish.Name))
        {
            var name = dish.Name;
            var catId = dish.DishCategoryId;
            bool duplicate = await _db.Dishes.AnyAsync(d =>
                d.DishCategoryId == catId && d.Name == name && (excludeId == null || d.Id != excludeId));
            if (duplicate)
                ModelState.AddModelError(nameof(Dish_BIT240118.Name), L.T("err.duplicateDish"));
        }
    }
}
