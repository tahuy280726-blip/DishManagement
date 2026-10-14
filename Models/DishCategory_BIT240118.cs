namespace DishManagement.Models;

public class DishCategory_BIT240118
{
    public int Id { get; set; }

    [LRequired("val.category.name.required")]
    [LStringLength(100, "val.category.name.max")]
    public string Name { get; set; }

    [LStringLength(500, "val.desc.max")]
    public string Description { get; set; }

    // 1 loại món có nhiều món ăn
    public List<Dish_BIT240118> Dishes { get; set; } = new();
}
