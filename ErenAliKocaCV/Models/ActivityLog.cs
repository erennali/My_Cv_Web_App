using System;
using System.ComponentModel.DataAnnotations;

namespace ErenAliKocaCV.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Action { get; set; }
        
        [Required]
        public string IpAddress { get; set; }
        
        [Required]
        public string UserAgent { get; set; }
        
        public bool IsSuccess { get; set; }
        
        public string Details { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 