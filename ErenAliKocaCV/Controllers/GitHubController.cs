using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Services.Interfaces;
using ErenAliKocaCV.Filters;
using System.Threading.Tasks;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class GitHubController : Controller
    {
        private readonly IGitHubService _githubService;
        private readonly IConfiguration _configuration;

        public GitHubController(IGitHubService githubService, IConfiguration configuration)
        {
            _githubService = githubService;
            _configuration = configuration;
        }
        
        public async Task<IActionResult> Index()
        {
            // Get GitHub username from appsettings.json
            var username = _configuration["GitHub:Username"] ?? "erenalikoca";
            
            // Get featured GitHub repositories
            var repositories = await _githubService.GetEnrichedFeaturedRepositoriesAsync(username);
            
            return View(repositories);
        }
    }
} 