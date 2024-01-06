using System.Diagnostics;
using System.Text.Json;
using CourseWorkAppDev1.Utils.HelperClass;
using CourseWorkAppDev1.Utils.Model;
namespace CourseWorkAppDev1.Utils.HelperServices;
public class ActionService
{
    private readonly AuthenticationService authentication;

    public ActionService(AuthenticationService authentication)
    {
        this.authentication = authentication;
    }

    public async Task<CustomType> Register<T>(UserModel data) where T : UserModel, new()
    {
        try
        {
            string savePath = data.userType == UserType.admin ? "admin.json" : "staff.json";
            var path = new FileManagement().DirectoryPath("database", savePath);
            Trace.WriteLine("This is Path: " + path);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            List<UserModel> userList = new List<UserModel>();
            if (File.Exists(path))
            {
                var existingData = await File.ReadAllTextAsync(path);
                userList = JsonSerializer.Deserialize<List<UserModel>>(existingData) ?? new List<UserModel>();
            }
            int maxId = userList.Any() ? userList.Max(s => s.Id) : 0;
            data.Id = maxId + 1;

            data.Password = this.authentication.GenerateHash(data.Password);
            Trace.WriteLine("This is PASSWORD: ", data.Password);
            userList.Add(data);

            var jsonData = JsonSerializer.Serialize(userList);
            await File.WriteAllTextAsync(path, jsonData);
            Trace.WriteLine(path);
            return new CustomType { Success = true, Message = $"{data.userType} registered successfully" };
        }
        catch (Exception ex)
        {
            Trace.WriteLine("Exception: {0}", ex.Message);
            return new CustomType { Success = false, Message = ex.Message };
        }
    }
    //Login Logic
    public async Task<CustomType> Login<T>(UserModel data) where T : UserModel, new()
    {
        try
        {
            //Making sure converting to UpperCase before logging
            data.userType = (UserType)Enum.Parse(typeof(UserType), data.userType.ToString().ToLower());
            Trace.WriteLine("This is UserType: " + data.userType);
            string savePath = data.userType == UserType.admin ? "admin.json" : "staff.json";
            var path = new FileManagement().DirectoryPath("database", savePath);
            if (File.Exists(path))
            {
                Trace.WriteLine("This is Path: " + path);
                var existingData = await File.ReadAllTextAsync(path);
                Trace.WriteLine("This is Existing Data: " + existingData);
                if (!string.IsNullOrEmpty(existingData))
                {
                    var existingUsers = JsonSerializer.Deserialize<List<UserModel>>(existingData);
                    var existingUser = existingUsers.FirstOrDefault(user => user.Username == data.Username);

                    if (existingUser != null)
                    {
                        var isAuthenticatedUser = authentication.Authenticate(existingUser, data);
                        Trace.WriteLine("This is Authentication: " + isAuthenticatedUser);
                        if (isAuthenticatedUser)
                        {
                            return new CustomType { Success = true, Message = "Login Success" };
                        }
                        else
                        {
                            return new CustomType { Success = false, Message = "Invalid Credentials" };
                        }
                    }
                }
            }
            return new CustomType { Success = false, Message = "Please Make Sure User is Registered" };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new CustomType { Success = false, Message = ex.Message };
        }
    }

    public async Task<CustomType> Delete<T>(int id, string fileName) where T : BaseModel, new()
    {
        try
        {
            var path = new FileManagement().DirectoryPath("database", fileName);
            if (File.Exists(path))
            {
                var existingData = await File.ReadAllTextAsync(path);
                var list = JsonSerializer.Deserialize<List<T>>(existingData) ?? new List<T>();
                var itemToDelete = list.FirstOrDefault(c => c.Id == id);
                Trace.WriteLine(itemToDelete);
                if (itemToDelete != null)
                {
                    list.Remove(itemToDelete);
                    var jsonData = JsonSerializer.Serialize(list);
                    await File.WriteAllTextAsync(path, jsonData);
                    return new CustomType { Success = true, Message = "Deleted" };
                }
                return new CustomType { Success = false, Message = "Item not found" };
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