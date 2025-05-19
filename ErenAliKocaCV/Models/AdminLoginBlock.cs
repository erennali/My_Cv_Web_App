using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class AdminLoginBlock
    {
        [Key]
        public int Id { get; set; }
        public bool IsBlocked { get; set; }
    }
} 