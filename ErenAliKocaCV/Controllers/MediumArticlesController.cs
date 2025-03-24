using ErenAliKocaCV.Services;
using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class MediumArticlesController : Controller
    {
        private readonly ICVRepository _repository;

        public MediumArticlesController(ICVRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var articles = _repository.GetAllMediumArticles();
            return View(articles);
        }

        public IActionResult Details(int id)
        {
            var article = _repository.GetMediumArticleById(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }
    }
} 