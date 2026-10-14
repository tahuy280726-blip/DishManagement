using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DishManagement.Models;

public class Dish_BIT240118
{
    public int Id { get; set; }

    [LRequired("val.name.required")]
    [LStringLength(150, "val.name.max")]
    public string Name { get; set; }

    [LPositive("val.price.positive", 1_000_000_000)]
    public decimal Price { get; set; }

    [LPositive("val.time.positive", 1440)]
    public int PreparationTime { get; set; }

    public bool IsAvailable { get; set; } = true;

    [LStringLength(1000, "val.desc.max")]
    public string Description { get; set; }

    public int DishCategoryId { get; set; }

    [ValidateNever]
    public DishCategory_BIT240118 DishCategory { get; set; }

    // 1 món ăn có nhiều ảnh
    [ValidateNever]
    public List<DishImage_BIT240118> Images { get; set; } = new();

    // Phân loại thời gian: tính khi hiển thị, KHÔNG có cột trong database
    public bool IsQuick => PreparationTime <= 15;
}
