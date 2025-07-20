using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillUpPlatform.Domain.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string Message { get; set; } = null!;
        public string? StackTrace { get; set; }
        public string Severity { get; set; } = "Error"; // Info, Warning, Error
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
