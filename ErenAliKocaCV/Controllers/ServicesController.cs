using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Services.Interfaces;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class ServicesController : Controller
    {
        private readonly IProfessionalService _professionalService;

        public ServicesController(IProfessionalService professionalService)
        {
            _professionalService = professionalService;
        }
        
        public IActionResult Index()
        {
            var services = _professionalService.GetAllServices();
            ViewBag.Services = services;
            
            return View();
        }
    }
} 