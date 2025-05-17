using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Services.Interfaces;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class SkillsController : Controller
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }
        
        public IActionResult Index()
        {
            var skills = _skillService.GetAllSkills();
            ViewBag.Skills = skills;
            
            return View();
        }
    }
} 