using System.ComponentModel.DataAnnotations;
namespace CourseWorkAppDev1.Utils.Model;
public class UserModel:BaseModel
{
    public UserType? userType { get; set; }

    [StringLength(20, MinimumLength = 4, ErrorMessage = "User must be at least 4 characters long")]
    public string? Username { get; set; }

    [StringLength(20, MinimumLength = 4, ErrorMessage = "Password must be at least 6 characters long")]
    public string? Password { get; set; }

    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }
}