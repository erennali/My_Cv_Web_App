using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class Experience
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(100)]
        public string Position { get; set; }
        
        [Required, StringLength(100)]
        public string Company { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public bool IsCurrentJob { get; set; }
    }
} 