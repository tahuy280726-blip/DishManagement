using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DishManagement.Models;

public class DishImage_BIT240118
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public bool IsThumbnail { get; set; }
    public int DishId { get; set; }

    [ValidateNever]
    public Dish_BIT240118 Dish { get; set; }
}
