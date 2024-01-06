
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using CourseWorkAppDev1.Utils.HelperClass;
using CourseWorkAppDev1.Utils.Model;
using CourseWorkAppDev1.Modules.Coffee.model;

namespace CourseWorkAppDev1.Modules.Staff.service;

public class StaffService
{

    public async Task<CustomType> TakeOrder(List<CommonModel> coffeeData, string Email, decimal totalPrice)
    {
        try
        {
            var path = new FileManagement().DirectoryPath("database", "orderData.json");
            Trace.WriteLine("This is Path: " + path);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            List<OrderModel> existingOrderData = new List<OrderModel>();
            if (File.Exists(path))
            {
                var existingJson = await File.ReadAllTextAsync(path);
                existingOrderData = JsonSerializer.Deserialize<List<OrderModel>>(existingJson) ?? new List<OrderModel>();
            }

            var newOrderData = new OrderModel
            {
                Email = Email,
                TotalPrice = totalPrice,
                CoffeeData = coffeeData
            };

            existingOrderData.Add(newOrderData);

            var jsonData = JsonSerializer.Serialize(existingOrderData);
            await File.WriteAllTextAsync(path, jsonData);
            Trace.WriteLine(path);

            return new CustomType { Success = true, Message = "Order placed successfully" };
        }
        catch (Exception ex)
        {
            Trace.WriteLine("Exception: {0}", ex.Message);
            return new CustomType { Success = false, Message = ex.Message };
        }
    }

    List<CommonModel> coffeeList = new List<CommonModel>();
    public void setOrderedList(CommonModel coffeeData)
    {
        Trace.WriteLine("This is CoffeeData in Staff: " + coffeeData);
        this.coffeeList.Add(coffeeData);

    }
    public Task<List<CommonModel>> getOrderedCoffee()
    {
        return Task.FromResult(this.coffeeList);
    }

    public async Task<List<CommonModel>> removeOrderList(int index)
    {
        var itemToRemove = this.coffeeList.FirstOrDefault(c => c.index == index);
        this.coffeeList.Remove(itemToRemove);
        return this.coffeeList;
    }

    public async Task<CustomType> clearOrderList()
    {
        this.coffeeList.Clear();
        return new CustomType { Success = true, Message = "Success" };
    }

    public async Task<CustomType> isUserRegistered(string email)
    {
        var path = new FileManagement().DirectoryPath("database", "user.json");
        List<UserModel> userList = new List<UserModel>();
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        if (File.Exists(path))
        {
            var existingData = await File.ReadAllTextAsync(path);
            userList = JsonSerializer.Deserialize<List<UserModel>>(existingData) ?? new List<UserModel>();
        }
        for (int i = 0; i < userList.Count; i++)
        {
            if (userList[i].Email == email)
            {
                Trace.WriteLine("This is Email: " + userList[i].Email);
                return new CustomType { Success = true, Message = "Success" };
            }
        }
        return new CustomType { Success = false, Message = "User Not Found" };
    }


}