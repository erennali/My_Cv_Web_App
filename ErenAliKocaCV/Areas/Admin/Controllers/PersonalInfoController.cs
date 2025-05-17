using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
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
                    // Only accept image files
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(profileImageFile.FileName).ToLowerInvariant();
                    
                    if (!Array.Exists(allowedExtensions, ext => ext.Equals(fileExtension)))
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

                        // Create unique filename
                        var uniqueFileName = $"{Guid.NewGuid()}_{profileImageFile.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save file to server
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
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