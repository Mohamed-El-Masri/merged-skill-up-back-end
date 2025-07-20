using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.Notifications.Commands;

public class CreateNotificationCommand : IRequest<Result<NotificationDto>>
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int Type { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
}

public class SendBulkNotificationCommand : IRequest<Result>
{
    public List<Guid> UserIds { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class MarkNotificationAsReadCommand : IRequest<Result>
{
    public Guid NotificationId { get; set; }
}

public class MarkAllNotificationsAsReadCommand : IRequest<Result>
{
}

public class DeleteNotificationCommand : IRequest<Result>
{
    public Guid NotificationId { get; set; }
}

public class UpdateNotificationPreferencesCommand : IRequest<Result>
{
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool LearningReminders { get; set; }
    public bool AchievementNotifications { get; set; }
    public bool NewsletterSubscription { get; set; }
}

public class UpdateNotificationSettingsCommand : IRequest<Result>
{
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public Dictionary<string, bool> NotificationTypes { get; set; } = new();
}

public class SendNotificationCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationSettingsDto
{
    public int Id { get; set; }
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool LearningReminders { get; set; }
    public bool AssessmentNotifications { get; set; }
    public bool SystemUpdates { get; set; }
    public bool MarketingEmails { get; set; }
}
