using System.Text.Json.Serialization;

namespace ErenAliKocaCV.Models
{
    public class GitHubRepository
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
        
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        
        [JsonPropertyName("stargazers_count")]
        public int StarsCount { get; set; }
        
        [JsonPropertyName("forks_count")]
        public int ForksCount { get; set; }
        
        [JsonPropertyName("language")]
        public string Language { get; set; }
        
        // Bu özellik seçilmiş bir repo olup olmadığını belirtir
        public bool IsFeatured { get; set; }
        
        // Sıralama için order değeri
        public int Order { get; set; }
    }
} 