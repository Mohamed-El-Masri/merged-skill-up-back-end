using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Notifications.Commands;

namespace SkillUpPlatform.Application.Features.Notifications.Queries;

public class GetUserNotificationsQuery : IRequest<Result<List<NotificationDto>>>
{
    public int UserId { get; set; }
    public bool? IsRead { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetUnreadNotificationsQuery : IRequest<Result<List<NotificationDto>>>
{
    public int UserId { get; set; }
    public int? Limit { get; set; } = 10;
}

public class GetNotificationByIdQuery : IRequest<Result<NotificationDto>>
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }
}

public class GetNotificationCountQuery : IRequest<Result<NotificationCountDto>>
{
    public int UserId { get; set; }
}

public class GetNotificationSettingsQuery : IRequest<Result<NotificationSettingsDto>>
{
    public int UserId { get; set; }
}

public class NotificationCountDto
{
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public int ReadCount { get; set; }
}

public class GetNotificationsQuery : IRequest<Result<PagedResult<NotificationDto>>>
{
    public int UserId { get; set; }
    public bool? IsRead { get; set; }
    public bool UnreadOnly { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetUnreadNotificationsCountQuery : IRequest<Result<int>>
{
    public int UserId { get; set; }
}

public class GetNotificationTemplatesQuery : IRequest<Result<List<NotificationTemplateDto>>>
{
    public string? Category { get; set; }
    public bool? IsActive { get; set; }
}

public class NotificationTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class GetUnreadNotificationCountQuery : IRequest<int>
{
}

public class GetNotificationPreferencesQuery : IRequest<NotificationPreferencesDto>
{
}

public class NotificationPreferencesDto
{
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool LearningReminders { get; set; }
    public bool AchievementNotifications { get; set; }
    public bool NewsletterSubscription { get; set; }
}
