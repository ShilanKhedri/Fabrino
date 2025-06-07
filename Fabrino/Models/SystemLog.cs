using System;
using System.ComponentModel.DataAnnotations;

namespace Fabrino.Models
{
    public class SystemLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public string Action { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string? UserId { get; set; }

        public string? Details { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }
    }
}
