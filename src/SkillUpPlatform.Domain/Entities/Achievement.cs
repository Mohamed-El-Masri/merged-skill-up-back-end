using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class Achievement : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public AchievementType Type { get; set; }
    public int RequiredValue { get; set; }
    public int Points { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}

public class UserAchievement : BaseEntity
{
    public int UserId { get; set; }
    public int AchievementId { get; set; }
    public DateTime EarnedAt { get; set; }
    public int CurrentValue { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual Achievement Achievement { get; set; } = null!;
}

public class UserGoal : BaseEntity
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public GoalType Type { get; set; }
    public int TargetValue { get; set; }
    public int CurrentValue { get; set; }
    public DateTime TargetDate { get; set; }
    public GoalStatus Status { get; set; } = GoalStatus.Active;
    public virtual User User { get; set; } = null!;
}

public enum AchievementType
{
    LearningPath = 1,
    Assessment = 2,
    TimeSpent = 3,
    Streak = 4,
    Points = 5
}

public enum GoalType
{
    LearningPaths = 1,
    Assessments = 2,
    StudyHours = 3,
    Skills = 4
}

public enum GoalStatus
{
    Active = 1,
    Completed = 2,
    Paused = 3,
    Cancelled = 4
}
