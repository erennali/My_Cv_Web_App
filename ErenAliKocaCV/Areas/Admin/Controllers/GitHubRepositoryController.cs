using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Services.Interfaces;
using ErenAliKocaCV.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class GitHubRepositoryController : Controller
    {
        private readonly IGitHubService _githubService;
        private readonly IConfiguration _configuration;

        public GitHubRepositoryController(IGitHubService githubService, IConfiguration configuration)
        {
            _githubService = githubService;
            _configuration = configuration;
        }
        
        public async Task<IActionResult> Index()
        {
            // Seçilmiş repoları getir
            var featuredRepos = await _githubService.GetFeaturedRepositoriesAsync();
            return View(featuredRepos);
        }
        
        public async Task<IActionResult> SelectRepositories()
        {
            // GitHub kullanıcı adını al
            var username = _configuration["GitHub:Username"] ?? "erenalikoca";
            
            // Tüm GitHub repolarını getir
            var allRepos = await _githubService.GetUserRepositoriesAsync(username);
            
            // Veritabanındaki seçilmiş repoları getir
            var featuredRepos = await _githubService.GetFeaturedRepositoriesAsync();
            
            // Zaten seçilmiş olanları işaretle
            foreach (var repo in allRepos)
            {
                repo.IsFeatured = featuredRepos.Any(fr => fr.GitHubId == repo.Id);
            }
            
            return View(allRepos);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddFeatured(long githubId, string name)
        {
            // Yeni bir seçilmiş repo ekle
            var featuredRepo = new FeaturedRepository
            {
                GitHubId = githubId,
                Name = name,
                Order = (await _githubService.GetFeaturedRepositoriesAsync()).Count() + 1,
                CustomDescription = null // Açıkça null olarak ayarla
            };
            
            await _githubService.AddFeaturedRepositoryAsync(featuredRepo);
            
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveFeatured(int id)
        {
            await _githubService.RemoveFeaturedRepositoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var featuredRepos = await _githubService.GetFeaturedRepositoriesAsync();
            var repo = featuredRepos.FirstOrDefault(r => r.Id == id);
            
            if (repo == null)
            {
                return NotFound();
            }
            
            return View(repo);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(FeaturedRepository repository)
        {
            if (ModelState.IsValid)
            {
                await _githubService.UpdateFeaturedRepositoryAsync(repository);
                return RedirectToAction(nameof(Index));
            }
            
            return View(repository);
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(int id, int newOrder)
        {
            // Tüm seçili repoları al
            var repos = (await _githubService.GetFeaturedRepositoriesAsync()).ToList();
            
            // Güncellenecek repo
            var repo = repos.FirstOrDefault(r => r.Id == id);
            if (repo == null)
            {
                return NotFound();
            }
            
            // Sıralama güncelleme
            var oldOrder = repo.Order;
            repo.Order = newOrder;
            
            // Diğer repoların sıralamasını güncelle
            if (oldOrder < newOrder)
            {
                // Yukarı taşıma
                foreach (var otherRepo in repos.Where(r => r.Order > oldOrder && r.Order <= newOrder && r.Id != id))
                {
                    otherRepo.Order--;
                    await _githubService.UpdateFeaturedRepositoryAsync(otherRepo);
                }
            }
            else if (oldOrder > newOrder)
            {
                // Aşağı taşıma
                foreach (var otherRepo in repos.Where(r => r.Order >= newOrder && r.Order < oldOrder && r.Id != id))
                {
                    otherRepo.Order++;
                    await _githubService.UpdateFeaturedRepositoryAsync(otherRepo);
                }
            }
            
            await _githubService.UpdateFeaturedRepositoryAsync(repo);
            
            return RedirectToAction(nameof(Index));
        }
    }
} 