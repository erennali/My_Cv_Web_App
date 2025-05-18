using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using System.Linq;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PersonalInfoController : AdminControllerBase
    {
        private readonly IPersonalInfoService _personalInfoService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PersonalInfoController(IPersonalInfoService personalInfoService, IWebHostEnvironment webHostEnvironment)
        {
            _personalInfoService = personalInfoService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/PersonalInfo
        public IActionResult Index()
        {
            var personalInfo = _personalInfoService.GetPersonalInfo();
            if (personalInfo == null)
            {
                personalInfo = new PersonalInfo
                {
                    FullName = "Your Name",
                    Title = "Your Title",
                    Email = "your.email@example.com"
                };
            }
            return View(personalInfo);
        }

        // GET: Admin/PersonalInfo/Edit
        public IActionResult Edit()
        {
            var personalInfo = _personalInfoService.GetPersonalInfo();
            if (personalInfo == null)
            {
                personalInfo = new PersonalInfo
                {
                    FullName = "Your Name",
                    Title = "Your Title",
                    Email = "your.email@example.com"
                };
            }
            return View(personalInfo);
        }

        // POST: Admin/PersonalInfo/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PersonalInfo personalInfo, IFormFile profileImageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle profile image upload if a file was selected
                if (profileImageFile != null && profileImageFile.Length > 0)
                {
                    // SECURITY: Dosya adını kontrol et
                    string originalFileName = profileImageFile.FileName;
                    if (string.IsNullOrEmpty(originalFileName) || 
                        originalFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    {
                        ModelState.AddModelError("profileImageFile", "File name contains invalid characters.");
                        return View(personalInfo);
                    }

                    // Only accept image files
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(originalFileName).ToLowerInvariant();
                    
                    // SECURITY: Uzantı listede var mı kontrol et
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("profileImageFile", "Only image files (jpg, jpeg, png, gif, webp) are allowed.");
                        return View(personalInfo);
                    }

                    try
                    {
                        // Create directory if it doesn't exist
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profile");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // SECURITY: Güvenli dosya adı oluşturma
                        string safeFileName = Path.GetFileNameWithoutExtension(originalFileName);
                        
                        // Regex ile dosya adını filtrele (sadece güvenli karakterler)
                        if (!Regex.IsMatch(safeFileName, @"^[a-zA-Z0-9_\-\.]+$"))
                        {
                            safeFileName = "profile_image"; // Güvenli bir default ad kullan
                        }
                        
                        var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}{fileExtension}";
                        
                        // SECURITY: Tam dosya yolunu güvenli bir şekilde oluştur
                        var safeFilePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        // SECURITY: Normalize edilmiş yolun güvenli olup olmadığını kontrol et
                        var normalizedPath = Path.GetFullPath(safeFilePath);
                        var normalizedUploadsFolder = Path.GetFullPath(uploadsFolder);
                        
                        if (!normalizedPath.StartsWith(normalizedUploadsFolder))
                        {
                            ModelState.AddModelError("", "Security violation detected in file path.");
                            return View(personalInfo);
                        }

                        // Save file to server - güvenli yol kullanarak
                        using (var fileStream = new FileStream(normalizedPath, FileMode.Create))
                        {
                            await profileImageFile.CopyToAsync(fileStream);
                        }

                        // Update the profile image path
                        personalInfo.ProfileImage = $"/uploads/profile/{uniqueFileName}";
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error uploading image: {ex.Message}");
                        return View(personalInfo);
                    }
                }

                if (_personalInfoService.UpdatePersonalInfo(personalInfo))
                {
                    TempData["SuccessMessage"] = "Personal information updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error updating personal information.");
                }
            }
            return View(personalInfo);
        }
    }
} 