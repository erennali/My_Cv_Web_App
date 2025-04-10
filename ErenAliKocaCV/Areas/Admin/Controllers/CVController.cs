using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    public class CVController : AdminControllerBase
    {
        private readonly ICVRepository _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CVController(ICVRepository repository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/CV
        public IActionResult Index()
        {
            return View(_repository.GetAllCVFiles());
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
                // Create directory if it doesn't exist
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Create unique filename
                var uniqueFileName = $"{Guid.NewGuid()}_{cvFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file to server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await cvFile.CopyToAsync(fileStream);
                }

                // Save file information to database
                var cvFileEntity = new CVFile
                {
                    FileName = cvFile.FileName,
                    FilePath = $"/uploads/{uniqueFileName}",
                    UploadDate = DateTime.Now,
                    IsActive = isActive
                };

                if (_repository.AddCVFile(cvFileEntity))
                {
                    TempData["SuccessMessage"] = "CV file uploaded successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // If database save fails, delete the file
                    System.IO.File.Delete(filePath);
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
            if (_repository.SetCVFileActive(id))
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
            var cvFile = _repository.GetAllCVFiles().FirstOrDefault(cf => cf.Id == id);
            if (cvFile != null)
            {
                // Delete file from server
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, cvFile.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Delete from database
                if (_repository.DeleteCVFile(id))
                {
                    TempData["SuccessMessage"] = "CV file deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting CV file from database.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "CV file not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 