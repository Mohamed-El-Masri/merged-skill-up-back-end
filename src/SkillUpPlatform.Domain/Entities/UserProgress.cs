using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class UserProgress : BaseEntity
{
    public int UserId { get; set; }
    public int ContentId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentMinutes { get; set; }
    public int ProgressPercentage { get; set; } = 0;

    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Content Content { get; set; } = null!;
}
