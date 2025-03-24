using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(100)]
        public string Name { get; set; }
        
        // Proficiency level from 0 to 100
        [Range(0, 100)]
        public int Proficiency { get; set; }
        
        // Category like "Programming", "Languages", etc.
        [StringLength(50)]
        public string Category { get; set; }
    }
} 