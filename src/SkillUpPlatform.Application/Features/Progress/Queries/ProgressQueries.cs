using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Contentt.DTOs;
using SkillUpPlatform.Application.Features.Progress.Commands;

namespace SkillUpPlatform.Application.Features.Progress.Queries;

public class GetUserProgressQuery : IRequest<Result<List<UserProgressDto>>>
{
    public int UserId { get; set; }
    public int? LearningPathId { get; set; }
    public bool? IsCompleted { get; set; }
}

public class GetProgressAnalyticsQuery : IRequest<Result<ProgressAnalyticsDto>>
{
    public int UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class GetLearningStreakQuery : IRequest<Result<LearningStreakDto>>
{
    public int UserId { get; set; }
}

public class GetUserGoalsQuery : IRequest<Result<List<Common.Models.UserGoalDto>>>
{
    public int UserId { get; set; }
    public int? Status { get; set; }
    public int? Type { get; set; }
}

public class GetGoalProgressQuery : IRequest<Result<Common.Models.UserGoalDto>>
{
    public int GoalId { get; set; }
    public int UserId { get; set; }
}

public class GetUserAchievementsQuery : IRequest<Result<List<ProgressQueries_UserAchievementDto>>>
{
    public int UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class GetTimeSpentAnalyticsQuery : IRequest<Result<TimeSpentAnalyticsDto>>
{
    public int UserId { get; set; }
    public string? Period { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}

public class ProgressAnalyticsDto
{
    public int TotalContentItems { get; set; }
    public int CompletedContentItems { get; set; }
    public int TotalTimeSpentMinutes { get; set; }
    public int TotalLearningPaths { get; set; }
    public int CompletedLearningPaths { get; set; }
    public double AverageProgressPercentage { get; set; }
    public List<DailyProgressDto> DailyProgress { get; set; } = new();
    public List<ProgressQueries_CategoryProgressDto> CategoryProgress { get; set; } = new();
}

public class LearningStreakDto
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastLearningDate { get; set; }
    public List<DateTime> LearningDates { get; set; } = new();
}

public class DailyProgressDto
{
    public DateTime Date { get; set; }
    public int TimeSpentMinutes { get; set; }
    public int CompletedItems { get; set; }
}

public class ProgressQueries_CategoryProgressDto
{
    public string Category { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int CompletedItems { get; set; }
    public double ProgressPercentage { get; set; }
}

public class ProgressQueries_UserAchievementDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime EarnedAt { get; set; }
    public int CurrentValue { get; set; }
    public int RequiredValue { get; set; }
}

public class TimeSpentAnalyticsDto
{
    public TimeSpan TotalTimeSpent { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public int TotalSessions { get; set; }
    public List<DailyTimeSpentDto> DailyTimeSpent { get; set; } = new();
    public List<ContentTypeTimeDto> TimeByContentType { get; set; } = new();
    public List<CategoryTimeDto> TimeByCategoryDto { get; set; } = new();
}

public class DailyTimeSpentDto
{
    public DateTime Date { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public int SessionCount { get; set; }
}

public class ContentTypeTimeDto
{
    public string ContentType { get; set; } = string.Empty;
    public TimeSpan TimeSpent { get; set; }
    public double Percentage { get; set; }
}

public class CategoryTimeDto
{
    public string Category { get; set; } = string.Empty;
    public TimeSpan TimeSpent { get; set; }
    public double Percentage { get; set; }
}

public class GetContentProgressQuery : IRequest<Result<ContentProgressDto>>
{
    public int ContentId { get; set; }
    public int UserId { get; set; }
}

public class GetProgressStatisticsQuery : IRequest<Result<ProgressStatisticsDto>>
{
    public int UserId { get; set; }
    public string? Period { get; set; }
}
