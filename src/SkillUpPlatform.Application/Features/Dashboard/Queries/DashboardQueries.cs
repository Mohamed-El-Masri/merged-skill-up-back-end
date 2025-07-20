using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Progress.Queries;

namespace SkillUpPlatform.Application.Features.Dashboard.Queries;

public class GetStudentDashboardQuery : IRequest<Result<DashboardStudentDashboardDto>>
{
    public int UserId { get; set; }
}

public class GetDashboardOverviewQuery : IRequest<Result<DashboardOverviewDto>>
{
    public int UserId { get; set; }
}

public class GetRecentActivityQuery : IRequest<Result<List<RecentActivityDto>>>
{
    public int UserId { get; set; }
    public int Limit { get; set; } = 10;
}

public class GetUpcomingDeadlinesQuery : IRequest<Result<List<UpcomingDeadlineDto>>>
{
    public int UserId { get; set; }
    public int DaysAhead { get; set; } = 7;
}

public class GetRecommendationsQuery : IRequest<Result<List<RecommendationDto>>>
{
    public int UserId { get; set; }
    public string? Type { get; set; }
    public int Limit { get; set; } = 5;
}

public class GetLearningCalendarQuery : IRequest<Result<LearningCalendarDto>>
{
    public int? Month { get; set; }
    public int? Year { get; set; }
}

public class DashboardStudentDashboardDto
{
    public DashboardOverviewDto Overview { get; set; } = new();
    public List<RecentActivityDto> RecentActivity { get; set; } = new();
    public List<UpcomingDeadlineDto> UpcomingDeadlines { get; set; } = new();
    public List<RecommendationDto> Recommendations { get; set; } = new();
    public List<ProgressQueries_UserAchievementDto> RecentAchievements { get; set; } = new();
    public LearningStreakDto LearningStreak { get; set; } = new();
}

public class DashboardOverviewDto
{
    public int TotalLearningPaths { get; set; }
    public int CompletedLearningPaths { get; set; }
    public int InProgressLearningPaths { get; set; }
    public int TotalAssessments { get; set; }
    public int CompletedAssessments { get; set; }
    public int TotalTimeSpentMinutes { get; set; }
    public int UnreadNotifications { get; set; }
    public int TotalAchievements { get; set; }
    public int TotalPoints { get; set; }
    public double OverallProgress { get; set; }
}

public class RecentActivityDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public string? ActionUrl { get; set; }
}

public class UpcomingDeadlineDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int DaysRemaining { get; set; }
    public string Priority { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
}

public class RecommendationDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
    public double Score { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class LearningCalendarDto
{
    public List<CalendarEventDto> Events { get; set; } = new();
    public List<StudyStreakDto> StudyStreaks { get; set; } = new();
    public Dictionary<DateTime, int> DailyProgress { get; set; } = new();
}

public class CalendarEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class StudyStreakDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public bool IsActive { get; set; }
}

public class GetLearningProgressQuery : IRequest<Result<DashboardLearningProgressDto>>
{
    public int UserId { get; set; }
}

public class GetAchievementsQuery : IRequest<Result<List<DashboardAchievementDto>>>
{
    public int UserId { get; set; }
    public int Limit { get; set; } = 10;
}

public class GetPersonalizedRecommendationsQuery : IRequest<Result<List<PersonalizedRecommendationDto>>>
{
    public int UserId { get; set; }
    public string? Category { get; set; }
    public int Limit { get; set; } = 5;
}

public class GetRecentActivitiesQuery : IRequest<Result<List<ActivityDto>>>
{
    public int UserId { get; set; }
    public int Limit { get; set; } = 10;
}

public class GetLearningStatisticsQuery : IRequest<Result<LearningStatisticsDto>>
{
    public int UserId { get; set; }
    public string Period { get; set; } = "monthly"; // weekly, monthly, yearly
}

public class GetStudyStreakQuery : IRequest<Result<StudyStreakDto>>
{
    public int UserId { get; set; }
}

public class DashboardLearningProgressDto
{
    public int TotalLearningPaths { get; set; }
    public int CompletedLearningPaths { get; set; }
    public int InProgressLearningPaths { get; set; }
    public double OverallProgress { get; set; }
    public List<DashboardLearningPathProgressDto> LearningPathProgress { get; set; } = new();
}

public class DashboardLearningPathProgressDto
{
    public int LearningPathId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double Progress { get; set; }
    public TimeSpan TimeSpent { get; set; }
}

public class DashboardAssessmentProgressDto
{
    public int AssessmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double Score { get; set; }
    public TimeSpan TimeSpent { get; set; }
}

public class DashboardAchievementDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BadgeUrl { get; set; } = string.Empty;
    public DateTime DateEarned { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class PersonalizedRecommendationDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // LearningPath, Content, Resource
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double RecommendationScore { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ActivityDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? RelatedEntity { get; set; }
    public int? RelatedEntityId { get; set; }
}

public class LearningStatisticsDto
{
    public TimeSpan TotalTimeSpent { get; set; }
    public int TotalSessions { get; set; }
    public double AverageSessionTime { get; set; }
    public int CompletedContents { get; set; }
    public int TotalContents { get; set; }
    public double CompletionRate { get; set; }
    public List<DailyStatisticDto> DailyStats { get; set; } = new();
}

public class DailyStatisticDto
{
    public DateTime Date { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public int CompletedContents { get; set; }
    public int Sessions { get; set; }
}
