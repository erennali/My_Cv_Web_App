using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Services;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class SkillsController : Controller
    {
        private readonly ICVRepository _repository;

        public SkillsController(ICVRepository repository)
        {
            _repository = repository;
        }
        
        public IActionResult Index()
        {
            var skills = _repository.GetAllSkills();
            ViewBag.Skills = skills;
            
            return View();
        }
    }
} 