using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public bool VerifyCredentials(string username, string password)
        {
            var user = _context.AdminUsers.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return false;
            
            return _passwordHasher.VerifyPassword(user.Password, password);
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = _context.AdminUsers.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return false;
            
            if (!_passwordHasher.VerifyPassword(user.Password, oldPassword))
                return false;
            
            user.Password = _passwordHasher.HashPassword(newPassword);
            user.PasswordChangedDate = DateTime.UtcNow;
            return _context.SaveChanges() > 0;
        }

        public AdminUser GetUserByUsername(string username)
        {
            return _context.AdminUsers.FirstOrDefault(u => u.Username == username);
        }
    }
} 