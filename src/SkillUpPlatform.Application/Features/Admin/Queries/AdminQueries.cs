using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.Admin.Queries;

public class GetAdminDashboardQuery : IRequest<Result<AdminDashboardDto>>
{
}

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalLearningPaths { get; set; }
    public int TotalContent { get; set; }
    public int ActiveUsers { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<AdminRecentActivityDto> RecentActivities { get; set; } = new();
    public List<SystemAlertDto> SystemAlerts { get; set; } = new();
}

public class AdminRecentActivityDto
{
    public string Action { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}

public class SystemAlertDto
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class GetSystemStatisticsQuery : IRequest<Result<SystemStatisticsDto>>
{
}

public class SystemStatisticsDto
{
    public UserStatistics Users { get; set; } = new();
    public ContentStatistics Content { get; set; } = new();
    public EngagementStatistics Engagement { get; set; } = new();
    public PerformanceStatistics Performance { get; set; } = new();
}

public class UserStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
}

public class ContentStatistics
{
    public int TotalLearningPaths { get; set; }
    public int TotalResources { get; set; }
    public int TotalAssessments { get; set; }
    public int PublishedContent { get; set; }
    public int DraftContent { get; set; }
}

public class EngagementStatistics
{
    public double AverageSessionDuration { get; set; }
    public int TotalSessions { get; set; }
    public double CompletionRate { get; set; }
    public int TotalInteractions { get; set; }
}

public class PerformanceStatistics
{
    public double AverageResponseTime { get; set; }
    public double SystemUptime { get; set; }
    public long TotalRequests { get; set; }
    public int ErrorRate { get; set; }
}

public class GetSystemHealthQuery : IRequest<Result<SystemHealthDto>>
{
}

public class SystemHealthDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime LastChecked { get; set; }
    public List<HealthCheckDto> HealthChecks { get; set; } = new();
    public Dictionary<string, object> Metrics { get; set; } = new();
}

public class HealthCheckDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeSpan ResponseTime { get; set; }
}

public class GetAuditLogsQuery : IRequest<Result<PagedResult<AuditLogDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Action { get; set; }
    public int? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class AuditLogDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

public class GetSystemConfigQuery : IRequest<Result<SystemConfigDto>>
{
}

public class SystemConfigDto
{
    public Dictionary<string, string> Settings { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class GetPerformanceMetricsQuery : IRequest<Result<PerformanceMetricsDto>>
{
    public string? Period { get; set; }
}

public class PerformanceMetricsDto
{
    public double AverageResponseTime { get; set; }
    public long TotalRequests { get; set; }
    public int ErrorCount { get; set; }
    public double ErrorRate { get; set; }
    public double Uptime { get; set; }
    public Dictionary<string, double> EndpointMetrics { get; set; } = new();
}

public class GetErrorLogsQuery : IRequest<Result<PagedResult<ErrorLogDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Severity { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ErrorLogDto
{
    public Guid Id { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Exception { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? UserId { get; set; }
}

public class GetPlatformAnalyticsQuery : IRequest<Result<PlatformAnalyticsDto>>
{
    public string? Period { get; set; }
}

public class PlatformAnalyticsDto
{
    public UserGrowthDto UserGrowth { get; set; } = new();
    public AdminContentEngagementDto ContentEngagement { get; set; } = new();
    public RevenueAnalyticsDto Revenue { get; set; } = new();
    public List<PopularContentDto> PopularContent { get; set; } = new();
}

public class UserGrowthDto
{
    public int NewUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int RetentionRate { get; set; }
    public List<DailyGrowthDto> DailyGrowth { get; set; } = new();
}

public class DailyGrowthDto
{
    public DateTime Date { get; set; }
    public int NewUsers { get; set; }
    public int ActiveUsers { get; set; }
}

public class AdminContentEngagementDto
{
    public int TotalViews { get; set; }
    public int TotalCompletions { get; set; }
    public double AverageEngagementTime { get; set; }
    public List<CategoryEngagementDto> CategoryEngagement { get; set; } = new();
}

public class CategoryEngagementDto
{
    public string Category { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Completions { get; set; }
    public double EngagementRate { get; set; }
}

public class PopularContentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Completions { get; set; }
    public double Rating { get; set; }
}

public class GetUsersQuery : IRequest<Result<PagedResult<AdminUserDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? Role { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}

public class GetUserDetailsQuery : IRequest<Result<AdminUserDetailsDto>>
{
    public int UserId { get; set; }
}

public class AdminUserDetailsDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public List<Common.Models.UserLearningPathDto> LearningPaths { get; set; } = new();
    public List<Common.Models.UserActivityDto> RecentActivity { get; set; } = new();
    public Common.Models.UserStatisticsDto Statistics { get; set; } = new();
}

public class GetUserAnalyticsQuery : IRequest<Result<UserAnalyticsDto>>
{
    public int UserId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}

public class UserAnalyticsDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public LearningProgressDto Progress { get; set; } = new();
    public EngagementMetricsDto Engagement { get; set; } = new();
    public List<LearningPathProgressDto> LearningPaths { get; set; } = new();
    public List<AssessmentPerformanceDto> Assessments { get; set; } = new();
}

public class EngagementMetricsDto
{
    public int TotalSessions { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public int LoginCount { get; set; }
    public DateTime? LastLogin { get; set; }
    public List<DailyEngagementDto> DailyEngagement { get; set; } = new();
}

public class DailyEngagementDto
{
    public DateTime Date { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public int Sessions { get; set; }
    public int ContentViewed { get; set; }
}

public class AssessmentPerformanceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public double Percentage { get; set; }
    public DateTime CompletedAt { get; set; }
    public int AttemptCount { get; set; }
}

public class GetUserActivitiesQuery : IRequest<PagedResult<Common.Models.UserActivityDto>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? ActivityType { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}

public class GetUsersAnalyticsQuery : IRequest<Result<UsersAnalyticsDto>>
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}

public class UsersAnalyticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsers { get; set; }
    public double RetentionRate { get; set; }
    public List<UserGrowthDto> UserGrowth { get; set; } = new();
    public List<UserEngagementDto> TopEngagedUsers { get; set; } = new();
    public Dictionary<string, int> UsersByRole { get; set; } = new();
}

public class UserEngagementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TimeSpan TotalTimeSpent { get; set; }
    public int TotalSessions { get; set; }
    public int CompletedLearningPaths { get; set; }
    public DateTime? LastLogin { get; set; }
}
