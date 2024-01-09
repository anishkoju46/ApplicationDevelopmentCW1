using System.ComponentModel.DataAnnotations;

namespace CourseWorkAppDev1.Modules.Coffee.model;
public class CommonModel :BaseModel
{

    public CommonModel()
    {
        AddIns = new List<CommonModel>();
    }
    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; } // Name of the coffee, e.g., "Espresso", "Latte", "Cappuccino", etc.
    public string? CoffeeType { get; set; } // Type of the coffee, e.g., "Espresso", "Drip", "Cold Brew", etc.

    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    [Required(ErrorMessage = "Price is required.")]
    public decimal Price { get; set; } // Price of the coffee, e.g., 3.50, 4.00, 4.50, etc.
    public int index { get; set; }
    public List<CommonModel> AddIns { get; set; }
}