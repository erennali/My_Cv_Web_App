using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ISiteSettingsService _siteSettingsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SettingsController(ISiteSettingsService siteSettingsService, IWebHostEnvironment webHostEnvironment)
        {
            _siteSettingsService = siteSettingsService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            // Ayarlar sayfasını direkt Edit sayfasına yönlendiriyoruz
            return RedirectToAction(nameof(Edit));
        }

        public IActionResult Edit()
        {
            // Site ayarlarını veritabanından alıyoruz, yoksa yeni oluşturuyoruz
            var settings = _siteSettingsService.GetSiteSettings() ?? new SiteSettings();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SiteSettings siteSettings, IFormFile contactImageFile)
        {
            if (ModelState.IsValid)
            {
                // Eğer iletişim sayfası için resim yüklendiyse
                if (contactImageFile != null && contactImageFile.Length > 0)
                {
                    // Dosya adını güvenli hale getiriyoruz
                    var fileName = Path.GetFileNameWithoutExtension(contactImageFile.FileName);
                    var extension = Path.GetExtension(contactImageFile.FileName);
                    fileName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{extension}";
                    
                    // Dosyayı wwwroot/uploads klasörüne kaydediyoruz
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    
                    // Klasör yoksa oluşturuyoruz
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await contactImageFile.CopyToAsync(fileStream);
                    }
                    
                    // Dosya yolunu ayarlara kaydediyoruz
                    siteSettings.ContactImage = $"/uploads/{fileName}";
                }
                
                // Ayarları veritabanına kaydediyoruz
                bool success = _siteSettingsService.UpdateSiteSettings(siteSettings);
                
                if (success)
                {
                    TempData["StatusMessage"] = "Site ayarları başarıyla güncellendi.";
                    return RedirectToAction(nameof(Edit));
                }
                else
                {
                    ModelState.AddModelError("", "Ayarları kaydederken bir hata oluştu.");
                }
            }
            
            return View(siteSettings);
        }
    }
}