using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    public class MediumArticleController : AdminControllerBase
    {
        private readonly IMediumArticleService _mediumArticleService;

        public MediumArticleController(IMediumArticleService mediumArticleService)
        {
            _mediumArticleService = mediumArticleService;
        }

        // GET: Admin/MediumArticle
        public IActionResult Index()
        {
            return View(_mediumArticleService.GetAllMediumArticles());
        }

        // GET: Admin/MediumArticle/Details/5
        public IActionResult Details(int id)
        {
            var article = _mediumArticleService.GetMediumArticleById(id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Admin/MediumArticle/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/MediumArticle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MediumArticle article)
        {
            if (ModelState.IsValid)
            {
                if (_mediumArticleService.AddMediumArticle(article))
                {
                    TempData["SuccessMessage"] = "Medium article added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error adding medium article.");
                }
            }
            return View(article);
        }

        // GET: Admin/MediumArticle/Edit/5
        public IActionResult Edit(int id)
        {
            var article = _mediumArticleService.GetMediumArticleById(id);
            if (article == null)
            {
                return NotFound();
            }
            
            return View(article);
        }

        // POST: Admin/MediumArticle/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MediumArticle article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (_mediumArticleService.UpdateMediumArticle(article))
                {
                    TempData["SuccessMessage"] = "Medium article updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error updating medium article.");
                }
            }
            return View(article);
        }

        // GET: Admin/MediumArticle/Delete/5
        public IActionResult Delete(int id)
        {
            var article = _mediumArticleService.GetMediumArticleById(id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Admin/MediumArticle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_mediumArticleService.DeleteMediumArticle(id))
            {
                TempData["SuccessMessage"] = "Medium article deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting medium article.";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
} 