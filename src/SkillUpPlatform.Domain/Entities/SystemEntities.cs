using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class SystemHealth : BaseEntity
{
    public string Component { get; set; } = string.Empty;
    public HealthStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
    public int ResponseTimeMs { get; set; }
    public Dictionary<string, object> AdditionalInfo { get; set; } = new();
}

public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public virtual User? User { get; set; }
}

public class SystemSettings : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsEditable { get; set; } = true;
}

public class UserActivity : BaseEntity
{
    public int UserId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
    public virtual User User { get; set; } = null!;
}

public enum HealthStatus
{
    Healthy = 1,
    Warning = 2,
    Critical = 3,
    Unknown = 4
}
