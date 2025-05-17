using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    public class ContactController : AdminControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        // GET: Admin/Contact/Messages
        public IActionResult Messages()
        {
            return View(_contactService.GetAllContactMessages());
        }

        // GET: Admin/Contact/Details/5
        public IActionResult Details(int id)
        {
            var message = _contactService.GetContactMessageById(id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Admin/Contact/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (_contactService.DeleteContactMessage(id))
            {
                TempData["SuccessMessage"] = "Message deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting message.";
            }
            
            return RedirectToAction(nameof(Messages));
        }
    }
} 