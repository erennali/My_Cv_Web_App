namespace ErenAliKocaCV.Models
{
    public class FeaturedRepository
    {
        public int Id { get; set; }
        
        // GitHub Repository ID'si
        public long GitHubId { get; set; }
        
        // Repository adı
        public string Name { get; set; }
        
        // Gösterim sırası
        public int Order { get; set; }
        
        // Repository açıklaması (GitHub'dan farklı bir açıklama kullanmak istediğinizde)
        public string? CustomDescription { get; set; }
    }
} 