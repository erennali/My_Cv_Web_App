using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private const int MaxLoginAttempts = 5;
        private const int LockoutMinutes = 15;

        public AccountController(
            ApplicationDbContext context, 
            IAuthService authService,
            IPasswordHasher passwordHasher, 
            IMemoryCache memoryCache, 
            IConfiguration configuration, 
            IWebHostEnvironment environment)
        {
            _context = context;
            _authService = authService;
            _passwordHasher = passwordHasher;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _environment = environment;
        }

        private string GetRegistrationKey()
        {
            // Önce ortam değişkeninden anahtarı almayı dene
            var key = Environment.GetEnvironmentVariable("ADMIN_REGISTRATION_KEY");
            
            // Eğer ortam değişkeni bulunamazsa, yapılandırma dosyasından al
            if (string.IsNullOrEmpty(key))
            {
                key = _configuration["AdminSettings:RegistrationKey"];
            }
            
            // Eğer hala bulunamazsa, varsayılan anahtarı kullan (sadece geliştirme için)
            if (string.IsNullOrEmpty(key) && _environment.IsDevelopment())
            {
                key = "Eren2024SecretRegistrationKey_DevOnly";
            }
            
            return key ?? string.Empty;
        }

        // Admin login blok kontrolü
        private bool IsAdminLoginBlocked()
        {
            var block = _context.AdminLoginBlocks.FirstOrDefault();
            return block != null && block.IsBlocked;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (IsAdminLoginBlocked())
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (IsAdminLoginBlocked())
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            // Brute force koruması - IP adresi kontrolü
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var cacheKey = $"LoginAttempt_{ipAddress}";
            // Giriş denemesi sayısını kontrol et
            if (!_memoryCache.TryGetValue(cacheKey, out int loginAttempts))
            {
                loginAttempts = 0;
            }
            // Kilitleme kontrolü
            var lockoutCacheKey = $"LoginLockout_{ipAddress}";
            if (_memoryCache.TryGetValue(lockoutCacheKey, out bool _))
            {
                ModelState.AddModelError("", $"Çok fazla başarısız giriş denemesi. Hesabınız {LockoutMinutes} dakika kilitlendi.");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                // IAuthService kullanarak kimlik doğrulama yapılıyor
                var user = _authService.GetUserByUsername(model.Username);
                if (user != null)
                {
                    // Modern güvenli hash doğrulama
                    bool passwordValid = _authService.VerifyCredentials(model.Username, model.Password);
                    if (passwordValid)
                    {
                        // Başarılı giriş, giriş denemesi sayısını sıfırla
                        _memoryCache.Remove(cacheKey);
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, "Admin"),
                            new Claim("UserId", user.Id.ToString())
                        };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                            new AuthenticationProperties
                            {
                                IsPersistent = model.RememberMe,
                                ExpiresUtc = DateTime.UtcNow.AddHours(3) // Oturum süresini 3 saate ayarla
                            });
                        // Login başarılı olduğunda aktivite logla
                        LogActivity(user.Username, "Login", true, ipAddress, GetUserAgent(), "Successful login");
                        // Update user's last login date
                        user.LastLoginDate = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                }
                // Başarısız giriş, giriş denemesi sayısını artır
                loginAttempts++;
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, loginAttempts, cacheOptions);
                // Max deneme sayısına ulaşıldıysa blokla
                if (loginAttempts >= MaxLoginAttempts)
                {
                    // SQL'de blokla
                    var block = _context.AdminLoginBlocks.FirstOrDefault();
                    if (block != null)
                    {
                        block.IsBlocked = true;
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.AdminLoginBlocks.Add(new Models.AdminLoginBlock { IsBlocked = true });
                        _context.SaveChanges();
                    }
                    // Başarısız girişi logla
                    LogActivity(model.Username, "Account Locked", false, ipAddress, GetUserAgent(), 
                        $"Account locked after {loginAttempts} failed attempts (SQL blok)");
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    // Başarısız girişi logla
                    LogActivity(model.Username, "Failed Login", false, ipAddress, GetUserAgent(), 
                        $"Failed login attempt {loginAttempts}");
                    ModelState.AddModelError("", "Invalid login attempt. Please check your username and password.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            // If already authenticated, redirect to dashboard
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            
            // Check if there are any admin users
            var adminExists = _context.AdminUsers.Any();
            ViewBag.FirstAdmin = !adminExists;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Check if there are any admin users
            var adminExists = _context.AdminUsers.Any();
            ViewBag.FirstAdmin = !adminExists;

            if (ModelState.IsValid)
            {
                // Validate registration key (skip for first admin)
                if (adminExists && model.RegistrationKey != GetRegistrationKey())
                {
                    ModelState.AddModelError("RegistrationKey", "Kayıt anahtarı geçersiz");
                    return View(model);
                }
                
                // Check if username already exists
                var existingUser = await _context.AdminUsers
                    .FirstOrDefaultAsync(u => u.Username == model.Username);
                    
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor");
                    return View(model);
                }
                
                // Check if email already exists
                var existingEmail = await _context.AdminUsers
                    .FirstOrDefaultAsync(u => u.Email == model.Email);
                    
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor");
                    return View(model);
                }
                
                // Create new admin user
                var newAdmin = new AdminUser
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = _passwordHasher.HashPassword(model.Password)
                };
                
                _context.AdminUsers.Add(newAdmin);
                await _context.SaveChangesAsync();
                
                // Log the registration
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var logMessage = $"New admin registered: User={model.Username}, Email={model.Email}, IP={ipAddress}, Time={DateTime.UtcNow}";
                Console.WriteLine(logMessage);
                
                TempData["SuccessMessage"] = "Kayıt işlemi başarılı. Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Çıkış aktivitesini logla
            if (username != null)
            {
                LogActivity(username, "Logout", true, ipAddress, GetUserAgent(), "User logged out");
            }
            
            return RedirectToAction("Login");
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult ChangePassword()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity?.Name;
                if (username == null)
                {
                    return RedirectToAction("Login");
                }
                
                // IAuthService kullanarak şifre değiştirme
                bool result = _authService.ChangePassword(username, model.CurrentPassword, model.NewPassword);
                
                if (result)
                {
                    // Başarılı şifre değişikliğini logla
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                    LogActivity(username, "ChangePassword", true, ipAddress, GetUserAgent(), "Password changed successfully");
                    
                    TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    // Başarısız şifre değişikliğini logla
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                    LogActivity(username, "ChangePassword", false, ipAddress, GetUserAgent(), "Failed to change password - incorrect current password");
                    
                    ModelState.AddModelError("", "Mevcut şifreniz doğru değil.");
                }
            }
            
            return View(model);
        }
        
        public IActionResult AccessDenied()
        {
            // Erişim reddedildi aktivitesini loglama
            var username = User.Identity?.Name ?? "Anonymous";
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            LogActivity(username, "AccessDenied", false, ipAddress, GetUserAgent(), "Yetkilendirme hatası - erişim engellendi");
            
            return View();
        }
        
        private string GetUserAgent()
        {
            return HttpContext.Request.Headers["User-Agent"].ToString();
        }
        
        private void LogActivity(string username, string action, bool isSuccess, string ipAddress, string userAgent, string details = null)
        {
            try
            {
                var log = new ActivityLog
                {
                    Username = username,
                    Action = action,
                    IsSuccess = isSuccess,
                    IpAddress = ipAddress,
                    UserAgent = userAgent?.Length > 500 ? userAgent.Substring(0, 500) : userAgent,
                    Details = details,
                    Timestamp = DateTime.UtcNow
                };
                
                _context.ActivityLogs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log dosyasına kaydet veya konsol çıktısı al
                Console.WriteLine($"Error logging activity: {ex.Message}");
            }
        }
    }
} 