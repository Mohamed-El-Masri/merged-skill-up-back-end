using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
    public virtual User User { get; set; } = null!;
}

public class NotificationSettings : BaseEntity
{
    public int UserId { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool LearningReminders { get; set; } = true;
    public bool AssessmentNotifications { get; set; } = true;
    public bool SystemUpdates { get; set; } = true;
    public bool MarketingEmails { get; set; } = false;
    public virtual User User { get; set; } = null!;
}

public enum NotificationType
{
    System = 1,
    Learning = 2,
    Assessment = 3,
    Achievement = 4,
    Reminder = 5,
    Social = 6,
    Marketing = 7
}
