using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services
{
    public interface IGitHubService
    {
        /// <summary>
        /// Kullanıcının tüm GitHub repolarını getirir
        /// </summary>
        Task<IEnumerable<GitHubRepository>> GetUserRepositoriesAsync(string username);
        
        /// <summary>
        /// Veritabanında işaretlenmiş seçili repoları getirir
        /// </summary>
        Task<IEnumerable<FeaturedRepository>> GetFeaturedRepositoriesAsync();
        
        /// <summary>
        /// GitHub'dan ve veritabanından bilgileri birleştirerek işaretlenmiş repoları getirir
        /// </summary>
        Task<IEnumerable<GitHubRepository>> GetEnrichedFeaturedRepositoriesAsync(string username);
        
        /// <summary>
        /// Repository'i seçili olarak işaretler
        /// </summary>
        Task AddFeaturedRepositoryAsync(FeaturedRepository repository);
        
        /// <summary>
        /// Repository'i seçili listesinden kaldırır
        /// </summary>
        Task RemoveFeaturedRepositoryAsync(int id);
        
        /// <summary>
        /// Seçili repository bilgilerini günceller
        /// </summary>
        Task UpdateFeaturedRepositoryAsync(FeaturedRepository repository);
    }
} 