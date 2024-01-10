using System.Security.Cryptography;
using System.Text;
using CourseWorkAppDev1.Utils.Model;

namespace CourseWorkAppDev1.Utils.HelperServices;
public class AuthenticationService
{
    public string GenerateHash(string data)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

    public bool Authenticate(UserModel existingData, UserModel comingData)
    {
        var hashedPassword = this.GenerateHash(comingData.Password);
        if (existingData.Username == comingData.Username && existingData.Password == hashedPassword)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
