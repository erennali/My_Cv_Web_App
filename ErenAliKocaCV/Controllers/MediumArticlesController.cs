using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class MediumArticlesController : Controller
    {
        private readonly IMediumArticleService _mediumArticleService;

        public MediumArticlesController(IMediumArticleService mediumArticleService)
        {
            _mediumArticleService = mediumArticleService;
        }

        public IActionResult Index()
        {
            var articles = _mediumArticleService.GetAllMediumArticles();
            return View(articles);
        }

        public IActionResult Details(int id)
        {
            var article = _mediumArticleService.GetMediumArticleById(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }
    }
} 