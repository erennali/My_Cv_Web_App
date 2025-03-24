using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class CVFile
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(100)]
        public string FileName { get; set; }
        
        [Required, StringLength(255)]
        public string FilePath { get; set; }
        
        public DateTime UploadDate { get; set; }
        
        public bool IsActive { get; set; }
    }
} 