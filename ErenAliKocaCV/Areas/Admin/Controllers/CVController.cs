using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CVController : AdminControllerBase
    {
        private readonly ICVFileService _cvFileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CVController(ICVFileService cvFileService, IWebHostEnvironment webHostEnvironment)
        {
            _cvFileService = cvFileService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/CV
        public IActionResult Index()
        {
            return View(_cvFileService.GetAllCVFiles());
        }

        // GET: Admin/CV/Upload
        public IActionResult Upload()
        {
            return View();
        }

        // POST: Admin/CV/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile cvFile, bool isActive)
        {
            if (cvFile == null || cvFile.Length == 0)
            {
                ModelState.AddModelError("cvFile", "Please select a file.");
                return View();
            }

            // Validate file is a PDF
            if (!cvFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("cvFile", "Only PDF files are allowed.");
                return View();
            }

            try
            {
                // SECURITY: Güvenli dosya işlemleri için
                // Sadece güvenli karakterleri kabul et
                string originalFileName = cvFile.FileName;
                if (string.IsNullOrEmpty(originalFileName) || 
                    originalFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    ModelState.AddModelError("cvFile", "File name contains invalid characters.");
                    return View();
                }

                // Create directory if it doesn't exist
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Create unique filename - sadece safe dosya adı ve GUID kullan
                string safeFileName = Path.GetFileNameWithoutExtension(originalFileName);
                string extension = Path.GetExtension(originalFileName);
                
                // SECURITY: Uzantıyı kontrol et (.pdf olmalı)
                if (!extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("cvFile", "Only .pdf file extension allowed.");
                    return View();
                }
                
                // Regex ile dosya adını filtrele (sadece güvenli karakterler)
                if (!Regex.IsMatch(safeFileName, @"^[a-zA-Z0-9_\-\.]+$"))
                {
                    safeFileName = "cv_document"; // Güvenli bir default ad kullan
                }
                
                var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}{extension}";
                
                // SECURITY: Tam dosya yolunu güvenli bir şekilde oluştur
                var safeFilePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                // SECURITY: Normalize edilmiş yolun güvenli olup olmadığını kontrol et
                var normalizedPath = Path.GetFullPath(safeFilePath);
                var normalizedUploadsFolder = Path.GetFullPath(uploadsFolder);
                
                if (!normalizedPath.StartsWith(normalizedUploadsFolder))
                {
                    ModelState.AddModelError("", "Security violation detected in file path.");
                    return View();
                }

                // Save file to server
                using (var fileStream = new FileStream(normalizedPath, FileMode.Create))
                {
                    await cvFile.CopyToAsync(fileStream);
                }

                // Save file information to database
                var cvFileEntity = new CVFile
                {
                    FileName = originalFileName,
                    FilePath = $"/uploads/{uniqueFileName}",
                    UploadDate = DateTime.Now,
                    IsActive = isActive
                };

                if (_cvFileService.AddCVFile(cvFileEntity))
                {
                    TempData["SuccessMessage"] = "CV file uploaded successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // If database save fails, delete the file
                    System.IO.File.Delete(normalizedPath);
                    ModelState.AddModelError("", "Error saving CV file to database.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error uploading file: {ex.Message}");
            }

            return View();
        }

        // POST: Admin/CV/SetActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetActive(int id)
        {
            if (_cvFileService.SetCVFileActive(id))
            {
                TempData["SuccessMessage"] = "CV file set as active successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error setting CV file as active.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/CV/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // İlk olarak veritabanından dosyayı getir
            var cvFile = _cvFileService.GetAllCVFiles().FirstOrDefault(cf => cf.Id == id);
            if (cvFile == null)
            {
                TempData["ErrorMessage"] = "CV file not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // SECURITY: Path traversal saldırılarına karşı koruma
                // 1. Sadece `/uploads/` dizini içindeki dosyaları silmeye izin ver
                if (!cvFile.FilePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["ErrorMessage"] = "Invalid file path location.";
                    return RedirectToAction(nameof(Index));
                }

                // 2. Dosya adını çıkart, yolları temizle (path traversal önleme)
                string sanitizedPath = cvFile.FilePath.TrimStart('/');
                string fileName = Path.GetFileName(sanitizedPath.Substring("uploads/".Length));

                // 3. Dosya adının geçerli olduğunu kontrol et
                if (string.IsNullOrEmpty(fileName) || fileName.Contains("..") || 
                    !Regex.IsMatch(fileName, @"^[a-zA-Z0-9_\-\.]+$"))
                {
                    TempData["ErrorMessage"] = "Invalid filename detected.";
                    return RedirectToAction(nameof(Index));
                }

                // 4. Güvenli bir şekilde tam dosya yolunu oluştur
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                var safePath = Path.Combine(uploadsFolder, fileName);

                // 5. Normalize edilmiş dosya yolunun uploads klasörü içinde kaldığından emin ol
                var normalizedSafePath = Path.GetFullPath(safePath);
                var normalizedUploadsFolder = Path.GetFullPath(uploadsFolder);
                
                if (!normalizedSafePath.StartsWith(normalizedUploadsFolder))
                {
                    // SECURITY: Path traversal tespit edildi - engelle
                    TempData["ErrorMessage"] = "Security violation detected.";
                    // Burada güvenlik ihlalini loglayabilirsiniz
                    return RedirectToAction(nameof(Index));
                }

                // 6. Dosyayı sil (güvenli yol kullanarak)
                if (System.IO.File.Exists(normalizedSafePath))
                {
                    System.IO.File.Delete(normalizedSafePath);
                }

                // 7. Veritabanından kaydı sil
                if (_cvFileService.DeleteCVFile(id))
                {
                    TempData["SuccessMessage"] = "CV file deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting CV file from database.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                // Hataları loglayabilirsiniz
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 