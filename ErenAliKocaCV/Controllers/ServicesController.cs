using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Services;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class ServicesController : Controller
    {
        private readonly ICVRepository _repository;

        public ServicesController(ICVRepository repository)
        {
            _repository = repository;
        }
        
        public IActionResult Index()
        {
            var services = _repository.GetAllServices();
            ViewBag.Services = services;
            
            return View();
        }
    }
} 