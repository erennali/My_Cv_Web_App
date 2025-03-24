using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(100)]
        public string Title { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        [StringLength(255)]
        public string? ImageUrl { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? ProjectUrl { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Category { get; set; }
        
        [StringLength(100)]
        public string Client { get; set; }
        
        public DateTime? CompletionDate { get; set; }
    }
} 