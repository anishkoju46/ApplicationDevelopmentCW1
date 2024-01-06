

namespace CourseWorkAppDev1.Modules.Coffee.model;

public class CommonModel :BaseModel
{

    public CommonModel()
    {
        AddIns = new List<CommonModel>();
    }
    public string? Name { get; set; } // Name of the coffee, e.g., "Espresso", "Latte", "Cappuccino", etc.
    public string? CoffeeType { get; set; } // Type of the coffee, e.g., "Espresso", "Drip", "Cold Brew", etc.
    public string? Size { get; set; } // Size of the coffee, e.g., "Small", "Medium", "Large", etc. 
    public decimal Price { get; set; } // Price of the coffee, e.g., 3.50, 4.00, 4.50, etc.
    public int index { get; set; }
    public List<CommonModel> AddIns { get; set; }
}