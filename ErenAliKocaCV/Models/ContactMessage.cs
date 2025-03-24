using System;
using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        
        [Required, StringLength(100)]
        public string Name { get; set; }
        
        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; }
        
        [Required, StringLength(200)]
        public string Subject { get; set; }
        
        [Required, StringLength(1000)]
        public string Message { get; set; }
        
        public DateTime DateSent { get; set; } = DateTime.Now;
        
        public bool IsRead { get; set; } = false;
    }
} 