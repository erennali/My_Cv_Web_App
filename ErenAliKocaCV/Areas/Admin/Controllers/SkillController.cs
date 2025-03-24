using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    public class SkillController : AdminControllerBase
    {
        private readonly ICVRepository _repository;

        public SkillController(ICVRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Skill
        public IActionResult Index()
        {
            return View(_repository.GetAllSkills());
        }

        // GET: Admin/Skill/Details/5
        public IActionResult Details(int id)
        {
            var skill = _repository.GetSkillById(id);
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
                if (_repository.AddSkill(skill))
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
            var skill = _repository.GetSkillById(id);
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
                if (_repository.UpdateSkill(skill))
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
            var skill = _repository.GetSkillById(id);
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
            if (_repository.DeleteSkill(id))
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