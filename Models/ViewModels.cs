namespace DishManagement.Models;

public class DishFilter
{
    public string Search { get; set; }
    public int? CategoryId { get; set; }
    public string Status { get; set; }      // "", "available", "unavailable"
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string Sort { get; set; }        // "name", "price_asc", "price_desc", "time_asc"
}

public class DishListViewModel
{
    public List<Dish_BIT240118> Items { get; set; } = new();
    public List<DishCategory_BIT240118> Categories { get; set; } = new();
}
