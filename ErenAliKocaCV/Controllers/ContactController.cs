using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IPersonalInfoService _personalInfoService;

        public ContactController(IContactService contactService, IPersonalInfoService personalInfoService)
        {
            _contactService = contactService;
            _personalInfoService = personalInfoService;
        }
        
        public IActionResult Index()
        {
            var personalInfo = _personalInfoService.GetPersonalInfo();
            ViewBag.PersonalInfo = personalInfo;
            
            return View();
        }
        
        [HttpPost]
        public IActionResult SubmitMessage(ContactMessage contactMessage)
        {
            if (ModelState.IsValid)
            {
                bool success = _contactService.SaveContactMessage(contactMessage);
                
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
            
            var personalInfo = _personalInfoService.GetPersonalInfo();
            ViewBag.PersonalInfo = personalInfo;
            
            return View("Index", contactMessage);
        }
    }
} 