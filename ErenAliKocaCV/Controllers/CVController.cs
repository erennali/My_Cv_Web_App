using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class CVController : Controller
    {
        private readonly ICVFileService _cvFileService;
        private readonly IWebHostEnvironment _environment;

        public CVController(ICVFileService cvFileService, IWebHostEnvironment environment)
        {
            _cvFileService = cvFileService;
            _environment = environment;
        }

        // CV indirme özelliği doğrudan URL ile çalışabilmeli, bu yüzden attribute'ı geçersiz kılıyoruz
        [RedirectToHome(false)]
        public IActionResult Download()
        {
            var activeCV = _cvFileService.GetActiveCVFile();
            if (activeCV == null)
            {
                return NotFound("Aktif CV dosyası bulunamadı.");
            }

            // Get the file path from the database
            var filePath = activeCV.FilePath;
            
            // Debugging - show the original file path
            Console.WriteLine($"Orijinal dosya yolu: {filePath}");
            
            // Assume it's a relative path (Web URL) if it starts with "/"
            // but doesn't contain a drive letter (Windows) or doesn't start with "//"
            bool isWebPath = filePath.StartsWith("/") && 
                            !filePath.Contains("://") && 
                            !filePath.StartsWith("//") &&
                            !(filePath.Length > 2 && filePath[1] == ':');
                
            if (isWebPath)
            {
                // Remove leading slash 
                if (filePath.StartsWith("/"))
                {
                    filePath = filePath.Substring(1);
                }
                
                // Combine with wwwroot path
                filePath = Path.Combine(_environment.WebRootPath, filePath);
                
                Console.WriteLine($"Dönüştürülmüş dosya yolu: {filePath}");
            }
            
            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"CV dosyası sunucuda bulunamadı. Yol: {filePath}");
            }

            // Get file name and content type
            var fileName = activeCV.FileName;
            var contentType = "application/pdf"; // Default for CV, you may need to determine this dynamically
            
            // Return the file as a download
            return PhysicalFile(filePath, contentType, fileName);
        }
    }
} 