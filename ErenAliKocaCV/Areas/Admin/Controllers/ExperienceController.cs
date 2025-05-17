using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    public class ExperienceController : AdminControllerBase
    {
        private readonly IExperienceService _experienceService;

        public ExperienceController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        // GET: Admin/Experience
        public IActionResult Index()
        {
            return View(_experienceService.GetAllExperience());
        }

        // GET: Admin/Experience/Details/5
        public IActionResult Details(int id)
        {
            var experience = _experienceService.GetExperienceById(id);
            if (experience == null)
            {
                return NotFound();
            }

            return View(experience);
        }

        // GET: Admin/Experience/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Experience/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Experience experience)
        {
            if (ModelState.IsValid)
            {
                if (_experienceService.AddExperience(experience))
                {
                    TempData["SuccessMessage"] = "Experience added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error adding experience.");
                }
            }
            return View(experience);
        }

        // GET: Admin/Experience/Edit/5
        public IActionResult Edit(int id)
        {
            var experience = _experienceService.GetExperienceById(id);
            if (experience == null)
            {
                return NotFound();
            }
            
            return View(experience);
        }

        // POST: Admin/Experience/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Experience experience)
        {
            if (id != experience.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (_experienceService.UpdateExperience(experience))
                {
                    TempData["SuccessMessage"] = "Experience updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error updating experience.");
                }
            }
            return View(experience);
        }

        // GET: Admin/Experience/Delete/5
        public IActionResult Delete(int id)
        {
            var experience = _experienceService.GetExperienceById(id);
            if (experience == null)
            {
                return NotFound();
            }

            return View(experience);
        }

        // POST: Admin/Experience/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_experienceService.DeleteExperience(id))
            {
                TempData["SuccessMessage"] = "Experience deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting experience.";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
} 