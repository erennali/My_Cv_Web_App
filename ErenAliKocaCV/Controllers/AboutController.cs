using ErenAliKocaCV.Data;
using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Services;
using System;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class AboutController : Controller
    {
        private readonly ICVRepository _repository;
        private readonly ApplicationDbContext _context;

        public AboutController(ICVRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }
        
        public IActionResult Index()
        {
            var personalInfo = _repository.GetPersonalInfo();
            ViewBag.PersonalInfo = personalInfo;
            
            try
            {
                // Veritabanı bağlantısı ve proje sayısını güvenli bir şekilde al
                int projectCount = 0;
                
                if (_context.Database.CanConnect() && _context.Projects != null)
                {
                    projectCount = _context.Projects.Count();
                }
                
                ViewBag.ProjectCount = projectCount;
                // Ana sayfadaki partial view ile aynı davranışı sağlamak için
                ViewBag.ProjectCountForAbout = projectCount;
            }
            catch (Exception)
            {
                // Hata durumunda varsayılan değer kullan, hataları kaydetme
                ViewBag.ProjectCount = 0;
                ViewBag.ProjectCountForAbout = 0;
            }
            
            return View();
        }
    }
} 