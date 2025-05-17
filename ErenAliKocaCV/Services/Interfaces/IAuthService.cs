using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services.Interfaces
{
    public interface IAuthService
    {
        bool VerifyCredentials(string username, string password);
        bool ChangePassword(string username, string oldPassword, string newPassword);
        AdminUser GetUserByUsername(string username);
    }
} 