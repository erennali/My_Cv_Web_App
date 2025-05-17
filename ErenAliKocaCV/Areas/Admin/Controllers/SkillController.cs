using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    public class SkillController : AdminControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        // GET: Admin/Skill
        public IActionResult Index()
        {
            return View(_skillService.GetAllSkills());
        }

        // GET: Admin/Skill/Details/5
        public IActionResult Details(int id)
        {
            var skill = _skillService.GetSkillById(id);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        // GET: Admin/Skill/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Skill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Proficiency,Category")] Skill skill)
        {
            if (ModelState.IsValid)
            {
                if (_skillService.AddSkill(skill))
                {
                    TempData["SuccessMessage"] = "Skill added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error adding skill.");
                }
            }
            return View(skill);
        }

        // GET: Admin/Skill/Edit/5
        public IActionResult Edit(int id)
        {
            var skill = _skillService.GetSkillById(id);
            if (skill == null)
            {
                return NotFound();
            }
            return View(skill);
        }

        // POST: Admin/Skill/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Proficiency,Category")] Skill skill)
        {
            if (id != skill.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (_skillService.UpdateSkill(skill))
                {
                    TempData["SuccessMessage"] = "Skill updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error updating skill.");
                }
            }
            return View(skill);
        }

        // GET: Admin/Skill/Delete/5
        public IActionResult Delete(int id)
        {
            var skill = _skillService.GetSkillById(id);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        // POST: Admin/Skill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_skillService.DeleteSkill(id))
            {
                TempData["SuccessMessage"] = "Skill deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting skill.";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
} 