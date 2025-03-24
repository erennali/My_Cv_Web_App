using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class MediumArticle
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        
        [Required]
        [StringLength(500)]
        [Url]
        public string ArticleUrl { get; set; }
        
        [StringLength(500)]
        [Url]
        public string CoverImageUrl { get; set; }
        
        [Required]
        public DateTime PublishedDate { get; set; }
        
        // For ordering/filtering
        public int Order { get; set; }
    }
} 