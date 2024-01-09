
namespace CourseWorkAppDev1.Modules.Admin.service;
using CourseWorkAppDev1.Modules.Staff.model;
using CourseWorkAppDev1.Utils.HelperClass;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using CourseWorkAppDev1.Utils.HelperServices;
using CourseWorkAppDev1.Utils.Model;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;


public class AdminService
{
    private readonly AuthenticationService authentication;

    List<StaffModel> staffList;
    public AdminService(AuthenticationService authentication)
    {
        this.authentication = authentication;
    }


    public async Task readStaff()
    {
        try
        {
            var path = new FileManagement().DirectoryPath("database", "staff.json");
            if (File.Exists(path))
            {
                var existingData = await File.ReadAllTextAsync(path);
                Trace.WriteLine("This is Existing Data: " + existingData);
                staffList = JsonSerializer.Deserialize<List<StaffModel>>(existingData) ?? new List<StaffModel>();
            }
        }
        catch (Exception error)
        {
            Trace.WriteLine("This is Error: " + error.Message);
        }

    }
    public async Task<List<StaffModel>> getStaffList()
    {
        await this.readStaff();
        Trace.WriteLine("This is staff List: " + this.staffList);
        return this.staffList;
    }
    public async Task<CustomType> Edit(int id, UserModel model, string fileName)
    {
        try
        {
            var path = new FileManagement().DirectoryPath("database", fileName);
            if (!File.Exists(path))
            {
                return new CustomType { Success = false, Message = "File not found" };
            }

            var existingData = await File.ReadAllTextAsync(path);
            var list = JsonSerializer.Deserialize<List<UserModel>>(existingData) ?? new List<UserModel>();
            var itemToEdit = list.FirstOrDefault(c => c.Id == id);

            if (itemToEdit == null)
            {
                return new CustomType { Success = false, Message = "Not Found" };
            }

            itemToEdit.Username = model.Username ?? itemToEdit.Username;
            itemToEdit.Password = model.Password != null ? this.authentication.GenerateHash(model.Password) : itemToEdit.Password;

            int index = list.FindIndex(c => c.Id == id);
            list[index] = itemToEdit;

            var jsonData = JsonSerializer.Serialize(list);
            await File.WriteAllTextAsync(path, jsonData);

            return new CustomType { Success = true, Message = "Updated Successfully" };
        }
        catch (Exception error)
        {
            return new CustomType { Success = false, Message = error.Message };
        }
    }

    [Obsolete]
    public async Task<CustomType> GenerateReport(DateTime date, string reportType)
    {
        try
        {
            var fileManagement = new FileManagement();
            var path = fileManagement.DirectoryPath("database", "orderData.json");

            if (File.Exists(path))
            {
                var existingData = await File.ReadAllTextAsync(path);
                var list = JsonSerializer.Deserialize<List<OrderModel>>(existingData) ?? new List<OrderModel>();

                List<OrderModel> orders;
                string reportTitle;

                if (reportType == "daily")
                {
                    orders = list.Where(order => order.Date.Date == date.Date).ToList();
                    reportTitle = $"Coffee Report of Date: {date.Date.ToString("d")}";
                }
                else if (reportType == "monthly")
                {
                    orders = list.Where(order => order.Date.Month == date.Month && order.Date.Year == date.Year).ToList();
                    reportTitle = $"Coffee Report for Month: {date.Month}/{date.Year}";
                }
                else
                {
                    Trace.WriteLine("Invalid report type");
                    return new CustomType { Success = false, Message = "Invalid report type" };
                }

                if (orders.Any())
                {
                    var totalRevenue = orders.Sum(order =>
                        order.TotalPrice);

                    string reportDirPath = reportType == "daily"
                        ? fileManagement.DirectoryPath("reports", "days")
                        : fileManagement.DirectoryPath("reports", "month");
                    if (!Directory.Exists(reportDirPath))
                    {
                        Trace.WriteLine("Creating reports folder");
                        Directory.CreateDirectory(reportDirPath);
                    }

                    string reportPath = reportType == "daily"
                        ? Path.Combine(reportDirPath, $"OrderReport_{date.Month}_{date.Year}_{date.Day}.pdf")
                        : Path.Combine(reportDirPath, $"OrderReport_{date.Month}_{date.Year}.pdf");

                    Trace.WriteLine("Report Path: " + reportPath);
                    Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(1, Unit.Centimetre);
                            page.PageColor(Colors.White);
                            page.DefaultTextStyle(x => x.FontSize(12));
                            page.Header()
                                .Padding(10)
                                .Text(reportTitle)
                                .SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);

                            page.Content()
                                .PaddingVertical((float)0.5, Unit.Centimetre)
                                .Column(column =>
                                {
                                    column.Item().Text($"Total Revenue:  {totalRevenue}").FontSize(16).FontColor(Colors.Red.Darken1);

                                    if (reportType == "daily")
                                    {
                                        var coffeeGroup = orders
                                            .SelectMany(order => order.CoffeeData)
                                            .GroupBy(coffee => coffee.Name)
                                            .OrderByDescending(group => group.Count())
                                            .Take(5)
                                            .ToList();

                                        var addInsGroup = orders
                                            .SelectMany(order => order.CoffeeData.SelectMany(coffee => coffee.AddIns))
                                            .GroupBy(addIn => addIn.Name)
                                            .OrderByDescending(group => group.Count())
                                            .Take(5)
                                            .ToList();

                                        column.Item().Text("Top 5 Coffee's Sold:").FontSize(16).FontColor(Colors.Amber.Darken1);
                                        column.Item().Grid(grid =>
                                        {
                                            grid.Columns(2);
                                            grid.Item().Text("Coffee Name:").FontColor(Colors.Blue.Medium).SemiBold();
                                            grid.Item().Text("Coffee Quantity:").FontColor(Colors.Blue.Medium).SemiBold();
                                            foreach (var group in coffeeGroup)
                                            {
                                                grid.Item().Text(group.Key);
                                                grid.Item().Text(group.Count().ToString());
                                            }
                                        });

                                        column.Item().Text("Top 5 Add-ins Sold:").FontSize(16).FontColor(Colors.Amber.Darken1);
                                        column.Item().Grid(grid =>
                                        {
                                            grid.Columns(2);
                                            grid.Item().Text("Add-Ins Name").FontColor(Colors.Blue.Medium).SemiBold();
                                            grid.Item().Text("Add-Ins Quantity").FontColor(Colors.Blue.Medium).SemiBold();
                                            foreach (var addInGroup in addInsGroup)
                                            {
                                                grid.Item().Text(addInGroup.Key);
                                                grid.Item().Text(addInGroup.Count().ToString());
                                            }
                                        });
                                    }
                                });

                            page.Footer()
                                .AlignCenter()
                                .Padding(10)
                                .Text(x =>
                                {
                                    x.Span("Page ");
                                    x.CurrentPageNumber().FontColor(Colors.Grey.Medium);
                                });
                        });
                    })
                    .GeneratePdf(reportPath);

                    return new CustomType { Success = true, Message = "Report generated" };
                }
                else
                {
                    Trace.WriteLine($"No orders for the specified {reportType}");
                    return new CustomType { Success = false, Message = $"No orders for the specified {reportType}" };
                }
            }
            return new CustomType { Success = false, Message = "File not found" };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new CustomType { Success = false, Message = $"An error occurred: {ex.Message}" };
        }
    }
    public async Task<(List<OrderModel>, decimal)> GetSalesTransactionsAndTotalRevenue()
    {
        try
        {
            var path = new FileManagement().DirectoryPath("database", "orderData.json");
            if (File.Exists(path))
            {
                var existingData = await File.ReadAllTextAsync(path);
                var list = JsonSerializer.Deserialize<List<OrderModel>>(existingData) ?? new List<OrderModel>();

                var totalRevenue = list.Sum(order => order.TotalPrice);

                return (list, totalRevenue);
            }
            else
            {
                Trace.WriteLine("File not found");
                return (null, 0);
            }
        }
        catch (Exception error)
        {
            Trace.WriteLine("An error occurred: " + error.Message);
            return (null, 0);
        }
    }
}