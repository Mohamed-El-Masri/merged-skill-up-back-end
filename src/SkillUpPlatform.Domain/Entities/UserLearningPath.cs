using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class UserLearningPath : BaseEntity
{
    public int UserId { get; set; }
    public int LearningPathId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public LearningPathStatus Status { get; set; } = LearningPathStatus.NotStarted;
    public int ProgressPercentage { get; set; } = 0;

    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual LearningPath LearningPath { get; set; } = null!;
}
