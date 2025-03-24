using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(100)]
        public string Title { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        [StringLength(100)]
        public string IconName { get; set; }
        
        [StringLength(100)]
        public string IconClass { get; set; }
        
        public int Order { get; set; }
    }
} 