using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class PersonalInfo
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(100)]
        public string FullName { get; set; }
        
        [StringLength(100)]
        public string Title { get; set; }
        
        [StringLength(500)]
        public string Bio { get; set; }
        
        [StringLength(100)]
        public string Email { get; set; }
        
        [StringLength(20)]
        public string Phone { get; set; }
        
        [StringLength(200)]
        public string Address { get; set; }
        
        [StringLength(100)]
        public string Website { get; set; }
        
        [StringLength(255)]
        public string ProfileImage { get; set; }
        
        public DateTime? BirthDate { get; set; }
    }
} 