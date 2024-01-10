using System.ComponentModel.DataAnnotations;

namespace CourseWorkAppDev1.Modules.Coffee.model;
public class CommonModel :BaseModel
{

    public CommonModel()
    {
        AddIns = new List<CommonModel>();
    }
    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }
    public string? CoffeeType { get; set; } 
   
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    [Required(ErrorMessage = "Price is required.")]
    public decimal Price { get; set; } 
    public int index { get; set; }
    public List<CommonModel> AddIns { get; set; }
}