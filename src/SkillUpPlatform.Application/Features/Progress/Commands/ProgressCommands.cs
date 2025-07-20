using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.Progress.Commands;

public class UpdateProgressCommand : IRequest<Result<ProgressCommandsUserProgressDto>>
{
    public int UserId { get; set; }
    public int ContentId { get; set; }
    public int ProgressPercentage { get; set; }
    public int TimeSpentMinutes { get; set; }
    public bool IsCompleted { get; set; }
}

public class CompleteContentCommand : IRequest<Result<bool>>
{
    public int UserId { get; set; }
    public int ContentId { get; set; }
    public int TimeSpentMinutes { get; set; }
}

public class CreateUserGoalCommand : IRequest<Result<ProgressCommandsUserGoalDto>>
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; }
    public int TargetValue { get; set; }
    public DateTime TargetDate { get; set; }
}

public class UpdateUserGoalCommand : IRequest<Result<ProgressCommandsUserGoalDto>>
{
    public int GoalId { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? TargetValue { get; set; }
    public DateTime? TargetDate { get; set; }
    public int? Status { get; set; }
}

public class DeleteUserGoalCommand : IRequest<Result<bool>>
{
    public int GoalId { get; set; }
    public int UserId { get; set; }
}

public class ProgressCommandsUserProgressDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ContentId { get; set; }
    public string ContentTitle { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentMinutes { get; set; }
    public int ProgressPercentage { get; set; }
}

public class ProgressCommandsUserGoalDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TargetValue { get; set; }
    public int CurrentValue { get; set; }
    public DateTime TargetDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SetLearningGoalsCommand : IRequest<Result>
{
    public List<ProgressCommandsLearningGoalDto> Goals { get; set; } = new();
}

public class ProgressCommandsLearningGoalDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}

public class TrackTimeSpentCommand : IRequest<Result>
{
    public int ContentId { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public DateTime SessionStart { get; set; }
    public DateTime SessionEnd { get; set; }
}

// Progress Commands
public class StartContentCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public int ContentId { get; set; }
}

public class UpdateContentProgressCommand : IRequest<Result<bool>>
{
    public int ContentId { get; set; }
    public int UserId { get; set; }
    public int ProgressPercentage { get; set; }
    public int TimeSpentMinutes { get; set; }
    public bool IsCompleted { get; set; }
}

// Learning Path Commands
public class UpdateLearningPathCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public List<Guid> ContentIds { get; set; } = new();
}

// Notification Commands
public class UpdateNotificationSettingsCommand : IRequest<Result>
{
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public Dictionary<string, bool> NotificationTypes { get; set; } = new();
}

public class SendNotificationCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}

// Queries
public class GetOverallProgressQuery : IRequest<Result<OverallProgressDto>>
{
    public int UserId { get; set; }
}

public class GetLearningPathProgressQuery : IRequest<Result<LearningPathProgressDto>>
{
    public int UserId { get; set; }
    public int LearningPathId { get; set; }
}

public class GetLearningStreaksQuery : IRequest<Result<LearningStreaksDto>>
{
    public int UserId { get; set; }
}

public class GetLearningGoalsQuery : IRequest<Result<Common.Models.LearningGoalsDto>>
{
    public int UserId { get; set; }
}



public class OverallProgressDto
{
    public int TotalLearningPaths { get; set; }
    public int CompletedLearningPaths { get; set; }
    public int InProgressLearningPaths { get; set; }
    public double OverallCompletionRate { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public int CurrentStreak { get; set; }
}

public class ProgressStatisticsDto
{
    public int TotalContent { get; set; }
    public int CompletedContent { get; set; }
    public int InProgressContent { get; set; }
    public double CompletionRate { get; set; }
    public TimeSpan WeeklyTimeSpent { get; set; }
    public int WeeklyGoalHours { get; set; }
}

public class LearningStreaksDto
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastActivity { get; set; }
    public List<DateTime> ActivityDates { get; set; } = new();
}



public class DailyTimeDto
{
    public DateTime Date { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public int ContentViewed { get; set; }
}
