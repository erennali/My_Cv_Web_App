using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace ErenAliKocaCV.Services.Implementations
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly string _gitHubApiUrl = "https://api.github.com";

        public GitHubService(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "CV-Application");
        }

        public async Task<IEnumerable<GitHubRepository>> GetUserRepositoriesAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_gitHubApiUrl}/users/{username}/repos?sort=updated&per_page=100");
                response.EnsureSuccessStatusCode();
                
                var repos = await response.Content.ReadFromJsonAsync<List<GitHubRepository>>();
                return repos ?? new List<GitHubRepository>();
            }
            catch (Exception)
            {
                return new List<GitHubRepository>();
            }
        }

        public async Task<IEnumerable<FeaturedRepository>> GetFeaturedRepositoriesAsync()
        {
            return await _context.FeaturedRepositories.ToListAsync();
        }

        public async Task<IEnumerable<GitHubRepository>> GetEnrichedFeaturedRepositoriesAsync(string username)
        {
            var featuredRepos = await _context.FeaturedRepositories.ToListAsync();
            var allRepos = await GetUserRepositoriesAsync(username);
            
            var enrichedRepos = new List<GitHubRepository>();
            
            foreach (var featuredRepo in featuredRepos)
            {
                var githubRepo = allRepos.FirstOrDefault(r => r.Name == featuredRepo.Name);
                if (githubRepo != null)
                {
                    githubRepo.IsFeatured = true;
                    githubRepo.Order = featuredRepo.Order;
                    if (!string.IsNullOrEmpty(featuredRepo.CustomDescription))
                    {
                        githubRepo.Description = featuredRepo.CustomDescription;
                    }
                    
                    enrichedRepos.Add(githubRepo);
                }
            }
            
            return enrichedRepos.OrderBy(r => r.Order).ToList();
        }

        public async Task AddFeaturedRepositoryAsync(FeaturedRepository repository)
        {
            _context.FeaturedRepositories.Add(repository);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFeaturedRepositoryAsync(int id)
        {
            var repo = await _context.FeaturedRepositories.FindAsync(id);
            if (repo != null)
            {
                _context.FeaturedRepositories.Remove(repo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateFeaturedRepositoryAsync(FeaturedRepository repository)
        {
            _context.Entry(repository).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
} 