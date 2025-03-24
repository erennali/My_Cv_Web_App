using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services;
using Microsoft.AspNetCore.Mvc;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    public class ContactController : AdminControllerBase
    {
        private readonly ICVRepository _repository;

        public ContactController(ICVRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Contact/Messages
        public IActionResult Messages()
        {
            return View(_repository.GetAllContactMessages());
        }

        // GET: Admin/Contact/Details/5
        public IActionResult Details(int id)
        {
            var message = _repository.GetContactMessageById(id);
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
            if (_repository.DeleteContactMessage(id))
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