using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class ContactController : Controller
    {
        private readonly ICVRepository _repository;

        public ContactController(ICVRepository repository)
        {
            _repository = repository;
        }
        
        public IActionResult Index()
        {
            var personalInfo = _repository.GetPersonalInfo();
            ViewBag.PersonalInfo = personalInfo;
            
            return View();
        }
        
        [HttpPost]
        public IActionResult SubmitMessage(ContactMessage contactMessage)
        {
            if (ModelState.IsValid)
            {
                bool success = _repository.SaveContactMessage(contactMessage);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Your message has been sent successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "There was an error saving your message. Please try again.");
                }
            }
            
            var personalInfo = _repository.GetPersonalInfo();
            ViewBag.PersonalInfo = personalInfo;
            
            return View("Index", contactMessage);
        }
    }
} 