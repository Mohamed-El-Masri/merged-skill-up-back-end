namespace SkillUpPlatform.Application.Common.Models;

public class ContentProgressDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public DateTime? CompletionDate { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? LastAccessed { get; set; }
}

public class LearningProgressDto
{
    public int TotalLearningPaths { get; set; }
    public int CompletedLearningPaths { get; set; }
    public int InProgressLearningPaths { get; set; }
    public double OverallProgress { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public int CurrentStreak { get; set; }
    public List<RecentLearningPathDto> RecentLearningPaths { get; set; } = new();
}

public class RecentLearningPathDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public DateTime LastAccessed { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class LearningPathProgressDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public List<ContentProgressDto> ContentProgress { get; set; } = new();
    public List<AssessmentProgressDto> AssessmentProgress { get; set; } = new();
}

public class AssessmentProgressDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? Score { get; set; }
    public int MaxScore { get; set; }
    public DateTime? CompletionDate { get; set; }
    public int AttemptCount { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPassed { get; set; }
}

public class RevenueAnalyticsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<MonthlyRevenueDto> MonthlyTrend { get; set; } = new();
    public List<LearningPathRevenueDto> TopEarningPaths { get; set; } = new();
}

public class MonthlyRevenueDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Revenue { get; set; }
    public int Sales { get; set; }
}

public class LearningPathRevenueDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Sales { get; set; }
    public decimal AveragePrice { get; set; }
}

public class AdminUserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public bool IsActive { get; set; }
}

public class CreatorLearningPathDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TotalContent { get; set; }
    public int CompletedContent { get; set; }
    public int EnrolledStudents { get; set; }
    public double AverageRating { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
}

public class UserLearningPathDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UserActivityDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
}

public class UserStatisticsDto
{
    public int TotalLearningPaths { get; set; }
    public int CompletedLearningPaths { get; set; }
    public int TotalAssessments { get; set; }
    public int PassedAssessments { get; set; }
    public double AverageScore { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public int CurrentStreak { get; set; }
    public int LoginCount { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class UserSessionDto
{
    public int Id { get; set; }
    public string DeviceInfo { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public DateTime? LogoutTime { get; set; }
    public bool IsActive { get; set; }
    public string Location { get; set; } = string.Empty;
}

public class StudentDashboardDto
{
    public UserDto User { get; set; } = new();
    public LearningProgressDto Progress { get; set; } = new();
    public List<RecentLearningPathDto> RecentLearningPaths { get; set; } = new();
    public List<UserActivityDto> RecentActivity { get; set; } = new();
    public List<string> Achievements { get; set; } = new();
    public List<string> Notifications { get; set; } = new();
}

public class UserGoalDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; }
    public int TargetValue { get; set; }
    public int CurrentValue { get; set; }
    public DateTime TargetDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsCompleted { get; set; }
    public int Status { get; set; }
}

public class AchievementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
}

public class FileDetailsDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class LearningGoalsDto
{
    public List<LearningGoalDto> Goals { get; set; } = new();
    public int CompletedGoals { get; set; }
    public int TotalGoals { get; set; }
    public double CompletionRate { get; set; }
}

public class LearningGoalDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int Target { get; set; }
    public DateTime TargetDate { get; set; }
    public bool IsCompleted { get; set; }
}

public class AdminUserDetailsDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public bool IsActive { get; set; }
    public List<UserLearningPathDto> LearningPaths { get; set; } = new();
    public List<UserActivityDto> RecentActivity { get; set; } = new();
    public UserStatisticsDto Statistics { get; set; } = new();
    public List<UserSessionDto> Sessions { get; set; } = new();
}

public class UserAnalyticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsers { get; set; }
    public double UserGrowthRate { get; set; }
    public List<UserRegistrationTrendDto> RegistrationTrend { get; set; } = new();
    public List<UserActivityTrendDto> ActivityTrend { get; set; } = new();
}

public class UsersAnalyticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public double UserGrowthRate { get; set; }
    public List<UserRegistrationTrendDto> RegistrationTrend { get; set; } = new();
    public List<UserActivityTrendDto> ActivityTrend { get; set; } = new();
    public List<UserEngagementDto> TopEngagedUsers { get; set; } = new();
}

public class UserRegistrationTrendDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int NewUsers { get; set; }
    public int TotalUsers { get; set; }
}

public class UserActivityTrendDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalSessions { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
}

public class UserEngagementDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TotalSessions { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public int CompletedPaths { get; set; }
    public double EngagementScore { get; set; }
}

/*public class FileDownloadDto
{
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}
*/

