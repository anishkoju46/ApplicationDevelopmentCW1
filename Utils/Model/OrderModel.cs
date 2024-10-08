namespace CourseWorkAppDev1.Utils.Model;
using CourseWorkAppDev1.Modules.Coffee.model;

public class OrderModel
{
    public string Email { get; set; }
    public int Count { get; set; }
    public int Days { get; set; }
    public decimal TotalPrice { get; set; }
    public List<CommonModel> CoffeeData { get; set; }
    public DateTime Date { get; set; }
}