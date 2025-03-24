using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class SiteSettings
    {
        [Key]
        public int Id { get; set; }
        
        [StringLength(255)]
        public string? ContactImage { get; set; }
        
        // Add other site-wide settings here as needed
    }
} 