using ErenAliKocaCV.Data;
using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Services.Interfaces;
using System;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class AboutController : Controller
    {
        private readonly IPersonalInfoService _personalInfoService;
        private readonly ApplicationDbContext _context;

        public AboutController(IPersonalInfoService personalInfoService, ApplicationDbContext context)
        {
            _personalInfoService = personalInfoService;
            _context = context;
        }
        
        public IActionResult Index()
        {
            var personalInfo = _personalInfoService.GetPersonalInfo();
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