
namespace CourseWorkAppDev1.Modules.Admin.service;
using CourseWorkAppDev1.Modules.Staff.model;
using CourseWorkAppDev1.Utils.HelperClass;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using CourseWorkAppDev1.Utils.HelperServices;
using CourseWorkAppDev1.Utils.Model;

public class AdminService
{
    private readonly AuthenticationService authentication;

    List<StaffModel> staffList;
    public AdminService(AuthenticationService authentication)
    {
        this.authentication = authentication;
    }

    public Task<CustomType> logOut()
    {
        return Task.FromResult(new CustomType { Success = true, Message = "Logout Success" });
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
            Trace.WriteLine("This is password: " + model.Password);
            var path = new FileManagement().DirectoryPath("database", fileName);
            if (File.Exists(path))
            {
                var existingData = await File.ReadAllTextAsync(path);
                var list = JsonSerializer.Deserialize<List<UserModel>>(existingData) ?? new List<UserModel>();
                var itemToEdit = list.FirstOrDefault(c => c.Id == id);
                Trace.WriteLine("This is ItemToEdit: " + itemToEdit);
                if (itemToEdit != null)
                {
                    if (itemToEdit.GetType().GetProperty("Username") != null)
                    {
                        itemToEdit.Username = model.Username;
                    }
                    if (itemToEdit.GetType().GetProperty("Password") != null)
                    {
                        model.Password = this.authentication.GenerateHash(model.Password);
                        itemToEdit.Password = model.Password;
                    }
                    int index = list.FindIndex(c => c.Id == id);
                    list[index] = itemToEdit;
                    var jsonData = JsonSerializer.Serialize(list);
                    await File.WriteAllTextAsync(path, jsonData);
                    return new CustomType { Success = true, Message = "Updated" };
                }
                else
                {
                    return new CustomType { Success = false, Message = "Item not found" };
                }
            }
            else
            {
                return new CustomType { Success = false, Message = "File not found" };
            }
        }
        catch (Exception error)
        {
            return new CustomType { Success = false, Message = error.Message };
        }
    }
}
