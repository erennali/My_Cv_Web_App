using ErenAliKocaCV.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RegisterSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RegisterSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var settings = _context.SiteSettings.FirstOrDefault();
            if (settings == null)
            {
                // Eğer hiç ayar yoksa oluştur
                settings = new Models.SiteSettings { IsRegisterActive = true };
                _context.SiteSettings.Add(settings);
                _context.SaveChanges();
            }
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleRegister()
        {
            var settings = _context.SiteSettings.FirstOrDefault();
            if (settings != null)
            {
                settings.IsRegisterActive = !settings.IsRegisterActive;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
} 