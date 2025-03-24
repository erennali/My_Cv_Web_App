using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using Microsoft.EntityFrameworkCore;
using Octokit;

namespace ErenAliKocaCV.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly ApplicationDbContext _context;
        private readonly GitHubClient _gitHubClient;
        
        public GitHubService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            
            // GitHub API için client oluşturma
            _gitHubClient = new GitHubClient(new ProductHeaderValue("ErenAliKocaCV"));
            
            // GitHub token varsa (opsiyonel) ekle
            var token = configuration["GitHub:Token"];
            if (!string.IsNullOrEmpty(token))
            {
                _gitHubClient.Credentials = new Credentials(token);
            }
        }
        
        public async Task<IEnumerable<GitHubRepository>> GetUserRepositoriesAsync(string username)
        {
            try
            {
                // GitHub API'den kullanıcının repolarını çekme
                var repositories = await _gitHubClient.Repository.GetAllForUser(username);
                
                // Octokit Repository nesnesini kendi GitHubRepository modelimize dönüştürme
                return repositories.Select(repo => new GitHubRepository
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    FullName = repo.FullName,
                    HtmlUrl = repo.HtmlUrl,
                    Description = repo.Description ?? "",
                    CreatedAt = repo.CreatedAt.DateTime,
                    UpdatedAt = repo.UpdatedAt.DateTime,
                    StarsCount = repo.StargazersCount,
                    ForksCount = repo.ForksCount,
                    Language = repo.Language ?? "Belirtilmemiş"
                });
            }
            catch (Exception)
            {
                // Hata durumunda boş liste döndür
                return Enumerable.Empty<GitHubRepository>();
            }
        }
        
        public async Task<IEnumerable<FeaturedRepository>> GetFeaturedRepositoriesAsync()
        {
            return await _context.FeaturedRepositories
                .OrderBy(r => r.Order)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<GitHubRepository>> GetEnrichedFeaturedRepositoriesAsync(string username)
        {
            // Veritabanından seçili repoları getir
            var featuredRepos = await GetFeaturedRepositoriesAsync();
            
            // GitHub'dan tüm repoları getir
            var allRepos = await GetUserRepositoriesAsync(username);
            
            // İki listeyi birleştir
            var result = new List<GitHubRepository>();
            
            foreach (var featuredRepo in featuredRepos)
            {
                var githubRepo = allRepos.FirstOrDefault(r => r.Id == featuredRepo.GitHubId);
                if (githubRepo != null)
                {
                    // Özel açıklama varsa, GitHub'dan gelen açıklamayı değiştir
                    if (!string.IsNullOrEmpty(featuredRepo.CustomDescription))
                    {
                        githubRepo.Description = featuredRepo.CustomDescription;
                    }
                    
                    githubRepo.IsFeatured = true;
                    githubRepo.Order = featuredRepo.Order;
                    
                    result.Add(githubRepo);
                }
            }
            
            return result.OrderBy(r => r.Order);
        }
        
        public async Task AddFeaturedRepositoryAsync(FeaturedRepository repository)
        {
            _context.FeaturedRepositories.Add(repository);
            await _context.SaveChangesAsync();
        }
        
        public async Task RemoveFeaturedRepositoryAsync(int id)
        {
            var repository = await _context.FeaturedRepositories.FindAsync(id);
            if (repository != null)
            {
                _context.FeaturedRepositories.Remove(repository);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task UpdateFeaturedRepositoryAsync(FeaturedRepository repository)
        {
            _context.FeaturedRepositories.Update(repository);
            await _context.SaveChangesAsync();
        }
    }
} 