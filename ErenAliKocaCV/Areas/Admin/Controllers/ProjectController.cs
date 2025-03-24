using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    public class ProjectController : AdminControllerBase
    {
        private readonly ICVRepository _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProjectController(ICVRepository repository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Project
        public IActionResult Index()
        {
            return View(_repository.GetAllProjects());
        }

        // GET: Admin/Project/Details/5
        public IActionResult Details(int id)
        {
            var project = _repository.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Admin/Project/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Project project, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Resim yükleme kontrolü
                if (imageFile == null || imageFile.Length == 0)
                {
                    ModelState.AddModelError("", "Please upload an image for the project.");
                    return View(project);
                }
                
                // Dosya adını güvenli hale getir
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                
                // Kaydetme yolunu belirle
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "projects");
                
                // Klasör yoksa oluştur
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                
                string filePath = Path.Combine(uploadsFolder, fileName);
                
                // Dosyayı kaydet
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }
                
                // ImageUrl'i güncelle
                project.ImageUrl = "/uploads/projects/" + fileName;
                
                // ProjectUrl'i boşalt (artık kullanmıyoruz)
                project.ProjectUrl = string.Empty;
                
                if (_repository.AddProject(project))
                {
                    TempData["SuccessMessage"] = "Project added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error adding project.");
                }
            }
            return View(project);
        }

        // GET: Admin/Project/Edit/5
        public IActionResult Edit(int id)
        {
            var project = _repository.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }
            
            return View(project);
        }

        // POST: Admin/Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Project project, IFormFile imageFile)
        {
            Console.WriteLine($"Edit POST başlatıldı - ID: {id}, Proje ID: {project.Id}, Title: {project.Title}");
            Console.WriteLine($"Alınan form verileri: Title={project.Title}, Description={project.Description?.Substring(0, Math.Min(project.Description?.Length ?? 0, 30))}...");
            Console.WriteLine($"ModelState geçerli mi: {ModelState.IsValid}");
            
            if (id != project.Id)
            {
                Console.WriteLine($"ID uyuşmazlığı: URL ID={id}, Form ID={project.Id}");
                return NotFound();
            }

            // ModelState hatalarını detaylı kontrol et
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState geçerli değil, hatalar:");
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"- {state.Key}: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
            }

            try
            {
                // Mevcut projeyi al
                var existingProject = _repository.GetProjectById(id);
                if (existingProject == null)
                {
                    Console.WriteLine($"Proje bulunamadı - ID: {id}");
                    return NotFound();
                }
                
                Console.WriteLine($"Mevcut proje bulundu - ID: {existingProject.Id}, Title: {existingProject.Title}");
                
                // ProjectUrl'i boşalt (artık kullanmıyoruz)
                project.ProjectUrl = string.Empty;
                
                // Eğer resim yüklendiyse
                if (imageFile != null && imageFile.Length > 0)
                {
                    Console.WriteLine($"Yeni resim yükleniyor: {imageFile.FileName}, Boyut: {imageFile.Length} bytes");
                    
                    // Eski resmi sil (eğer varsa ve uploads içindeyse)
                    if (!string.IsNullOrEmpty(existingProject.ImageUrl) && 
                        existingProject.ImageUrl.StartsWith("/uploads/"))
                    {
                        string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, 
                            existingProject.ImageUrl.TrimStart('/'));
                        
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            Console.WriteLine($"Eski resim siliniyor: {oldFilePath}");
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    
                    // Yeni dosyayı kaydet
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "projects");
                    
                    Console.WriteLine($"Yeni resim yolu: {uploadsFolder}/{fileName}");
                    
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }
                    
                    // ImageUrl'i güncelle
                    project.ImageUrl = "/uploads/projects/" + fileName;
                    Console.WriteLine($"Yeni ImageUrl: {project.ImageUrl}");
                }
                else
                {
                    // Resim yüklenmediyse mevcut URL'i koru
                    Console.WriteLine($"Yeni resim yüklenmedi, mevcut URL korunuyor: {existingProject.ImageUrl}");
                    project.ImageUrl = existingProject.ImageUrl;
                }
                
                // Değiştirilmiş projeyi yazdır
                Console.WriteLine("Veritabanına kaydedilecek güncellenmiş proje:");
                Console.WriteLine($"- ID: {project.Id}");
                Console.WriteLine($"- Title: {project.Title}");
                Console.WriteLine($"- Description: {project.Description?.Substring(0, Math.Min(project.Description?.Length ?? 0, 30))}...");
                Console.WriteLine($"- ImageUrl: {project.ImageUrl}");
                Console.WriteLine($"- ProjectUrl: {project.ProjectUrl}");
                Console.WriteLine($"- Category: {project.Category}");
                Console.WriteLine($"- Client: {project.Client}");
                Console.WriteLine($"- CompletionDate: {project.CompletionDate}");
                
                // Projeyi güncelle
                Console.WriteLine("UpdateProject metodu çağrılıyor...");
                bool success = _repository.UpdateProject(project);
                Console.WriteLine($"UpdateProject sonucu: {success}");
                
                if (success)
                {
                    Console.WriteLine("Güncelleme başarılı, yönlendiriliyor: Index");
                    TempData["SuccessMessage"] = "Project updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Console.WriteLine("Güncelleme başarısız, hata ekleniyor");
                    ModelState.AddModelError("", "Error updating project. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HATA: {ex.Message}");
                Console.WriteLine($"Hata tipi: {ex.GetType().Name}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"İç Hata: {ex.InnerException.Message}");
                    Console.WriteLine($"İç hata tipi: {ex.InnerException.GetType().Name}");
                }
                
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }
            
            Console.WriteLine("Edit POST sonlandı, View döndürülüyor");
            return View(project);
        }

        // GET: Admin/Project/Delete/5
        public IActionResult Delete(int id)
        {
            var project = _repository.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Admin/Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var project = _repository.GetProjectById(id);
            if (project != null)
            {
                // Dosya silinecekse
                if (!string.IsNullOrEmpty(project.ImageUrl) && 
                    project.ImageUrl.StartsWith("/uploads/"))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, 
                        project.ImageUrl.TrimStart('/'));
                    
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                
                if (_repository.DeleteProject(id))
                {
                    TempData["SuccessMessage"] = "Project deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting project.";
                }
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
} 
