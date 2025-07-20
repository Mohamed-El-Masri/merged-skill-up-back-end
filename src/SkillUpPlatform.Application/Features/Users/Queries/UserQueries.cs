using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Users.DTOs;

namespace SkillUpPlatform.Application.Features.Users.Queries;

public class GetUserByIdQuery : IRequest<Result<Common.Models.UserDto>>
{
    public int UserId { get; set; }
}

public class GetUserProfileQuery : IRequest<Result<UserProfileDto>>
{
    public int UserId { get; set; }
}

public class GetUsersQuery : IRequest<Result<List<Common.Models.UserDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

public class GetUserNotificationSettingsQuery : IRequest<Result<UserNotificationSettingsDto>>
{
    public int UserId { get; set; }
}

public class UserNotificationSettingsDto
{
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool LearningReminders { get; set; }
    public bool AchievementNotifications { get; set; }
    public bool NewsletterSubscription { get; set; }
    public bool WeeklyProgressReport { get; set; }
}

public class GetUserStatisticsQuery : IRequest<Result<UserQueries_UserStatisticsDto>>
{
    public int UserId { get; set; }
}

public class UserQueries_UserStatisticsDto
{
    public TimeSpan TotalTimeSpent { get; set; }
    public int TotalLearningPaths { get; set; }
    public int CompletedLearningPaths { get; set; }
    public int TotalAssessments { get; set; }
    public double AverageScore { get; set; }
    public int LoginCount { get; set; }
    public DateTime? LastLogin { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public List<UserQueries_CategoryProgressDto> CategoryProgress { get; set; } = new();
}

public class UserQueries_CategoryProgressDto
{
    public string Category { get; set; } = string.Empty;
    public int TotalContent { get; set; }
    public int CompletedContent { get; set; }
    public double CompletionRate { get; set; }
    public TimeSpan TimeSpent { get; set; }
}

public class GetUserLearningHistoryQuery : IRequest<Result<PagedResult<UserLearningHistoryDto>>>
{
    public int UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class UserLearningHistoryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime AccessedAt { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public int ProgressPercentage { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class GetUserAchievementsQuery : IRequest<Result<List<UserQueries_UserAchievementDto>>>
{
    public int UserId { get; set; }
}

public class UserQueries_UserAchievementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime? UnlockedAt { get; set; }
    public bool IsUnlocked { get; set; }
    public int Progress { get; set; }
    public int Target { get; set; }
}

public class GetUserActivityQuery : IRequest<Result<PagedResult<UserQueries_UserActivityDto>>>
{
    public int UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class UserQueries_UserActivityDto
{
    public int Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Details { get; set; }
}
