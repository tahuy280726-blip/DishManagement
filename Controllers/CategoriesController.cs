using DishManagement.Data;
using DishManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DishManagement.Controllers;

public class CategoriesController : AppController
{
    private readonly AppDbContext _db;
    public CategoriesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var list = await _db.DishCategories
            .AsNoTracking()
            .Include(c => c.Dishes)
            .OrderBy(c => c.Name)
            .ToListAsync();
        return View(list);
    }

    public IActionResult Create() => View(new DishCategory_BIT240118());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] DishCategory_BIT240118 category)
    {
        await ValidateAsync(category, null);
        if (!ModelState.IsValid) return View(category);

        _db.DishCategories.Add(category);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, L.T("err.generic"));
            return View(category);
        }

        TempData["Success"] = "msg.created";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFoundPage("err.categoryNotFound");
        var category = await _db.DishCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id.Value);
        if (category == null) return NotFoundPage("err.categoryNotFound");
        return View(category);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Name,Description")] DishCategory_BIT240118 category)
    {
        if (id == null) return NotFoundPage("err.categoryNotFound");
        var existing = await _db.DishCategories.FirstOrDefaultAsync(c => c.Id == id.Value);
        if (existing == null) return NotFoundPage("err.categoryNotFound");

        await ValidateAsync(category, id.Value);
        if (!ModelState.IsValid)
        {
            category.Id = id.Value;
            return View(category);
        }

        existing.Name = category.Name;
        existing.Description = category.Description;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, L.T("err.generic"));
            category.Id = id.Value;
            return View(category);
        }

        TempData["Success"] = "msg.updated";
        return RedirectToAction(nameof(Index));
    }

    // Không cho phép xóa loại món đang có món sử dụng
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFoundPage("err.categoryNotFound");
        var category = await _db.DishCategories.FirstOrDefaultAsync(c => c.Id == id.Value);
        if (category == null) return NotFoundPage("err.categoryNotFound");

        bool inUse = await _db.Dishes.AnyAsync(d => d.DishCategoryId == id.Value);
        if (inUse)
        {
            TempData["Error"] = "err.categoryInUse";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _db.DishCategories.Remove(category);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "err.categoryInUse";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "msg.deleted";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateAsync(DishCategory_BIT240118 category, int? excludeId)
    {
        category.Name = category.Name?.Trim();
        category.Description = string.IsNullOrWhiteSpace(category.Description) ? null : category.Description.Trim();

        bool nameBad = ModelState[nameof(DishCategory_BIT240118.Name)]?.Errors.Count > 0;
        if (!nameBad && !string.IsNullOrEmpty(category.Name))
        {
            var name = category.Name;
            bool duplicate = await _db.DishCategories.AnyAsync(c =>
                c.Name == name && (excludeId == null || c.Id != excludeId));
            if (duplicate)
                ModelState.AddModelError(nameof(DishCategory_BIT240118.Name), L.T("err.duplicateCategory"));
        }
    }
}
