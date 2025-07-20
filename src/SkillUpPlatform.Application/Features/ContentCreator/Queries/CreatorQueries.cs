using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.ContentCreator.Queries;

public class GetCreatorDashboardQuery : IRequest<Result<CreatorDashboardDto>>
{
}

public class CreatorDashboardDto
{
    public int TotalLearningPaths { get; set; }
    public int TotalStudents { get; set; }
    public int TotalRevenue { get; set; }
    public int TotalRatings { get; set; }
    public double AverageRating { get; set; }
    public int NewEnrollmentsThisMonth { get; set; }
    public List<PopularLearningPathDto> PopularLearningPaths { get; set; } = new();
    public List<RecentEnrollmentDto> RecentEnrollments { get; set; } = new();
    public List<ReviewDto> RecentReviews { get; set; } = new();
}

public class PopularLearningPathDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public double AverageRating { get; set; }
    public decimal Revenue { get; set; }
}

public class RecentEnrollmentDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string LearningPathTitle { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public decimal AmountPaid { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string LearningPathTitle { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class GetCreatorLearningPathsQuery : IRequest<Result<PagedResult<CreatorLearningPathDto>>>
{
    public int CreatorId { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetEnrolledStudentsQuery : IRequest<Result<PagedResult<EnrolledStudentDto>>>
{
    public int? LearningPathId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetCreatorAnalyticsQuery : IRequest<Result<CreatorAnalyticsDto>>
{
    public int CreatorId { get; set; }
    public string Period { get; set; } = "monthly";
}

public class GetEngagementAnalyticsQuery : IRequest<Result<EngagementAnalyticsDto>>
{
    public int CreatorId { get; set; }
    public string Period { get; set; } = "monthly";
}

public class GetRevenueAnalyticsQuery : IRequest<Result<CreatorRevenueAnalyticsDto>>
{
    public int CreatorId { get; set; }
    public string Period { get; set; } = "monthly";
}

public class LearningPathAnalyticsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public double CompletionRate { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageTimeToComplete { get; set; }
    public List<DailyEnrollmentDto> DailyEnrollments { get; set; } = new();
    public List<RatingDistributionDto> RatingDistribution { get; set; } = new();
}

public class DailyEnrollmentDto
{
    public DateTime Date { get; set; }
    public int Enrollments { get; set; }
    public int Completions { get; set; }
}

public class RatingDistributionDto
{
    public int Stars { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class GetCreatorStudentsQuery : IRequest<PagedResult<CreatorStudentDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? LearningPathId { get; set; }
    public string? Search { get; set; }
    public string? ProgressStatus { get; set; }
}

public class CreatorStudentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string LearningPathTitle { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime? LastAccessed { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class GetStudentProgressQuery : IRequest<Result<StudentProgressDto>>
{
    public int StudentId { get; set; }
    public int LearningPathId { get; set; }
}

public class StudentProgressDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public Guid LearningPathId { get; set; }
    public string LearningPathTitle { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime? LastAccessed { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public List<ContentProgressDto> ContentProgress { get; set; } = new();
    public List<CreatorAssessmentResultDto> AssessmentResults { get; set; } = new();
}

public class CreatorAssessmentResultDto
{
    public Guid Id { get; set; }
    public string AssessmentTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public double Percentage { get; set; }
    public DateTime CompletedAt { get; set; }
    public int AttemptNumber { get; set; }
}

public class CreatorAnalyticsDto
{
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public double AverageCompletion { get; set; }
    public double AverageRating { get; set; }
    public int TotalRevenue { get; set; }
    public List<MonthlyStatsDto> MonthlyStats { get; set; } = new();
}

public class EngagementAnalyticsDto
{
    public int TotalSessions { get; set; }
    public TimeSpan AverageSessionTime { get; set; }
    public double RetentionRate { get; set; }
    public int DropOffRate { get; set; }
    public List<CreatorContentEngagementDto> ContentEngagement { get; set; } = new();
}

public class CreatorRevenueAnalyticsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal AverageRevenuePerStudent { get; set; }
    public List<CreatorMonthlyRevenueDto> MonthlyBreakdown { get; set; } = new();
}

public class MonthlyStatsDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int NewStudents { get; set; }
    public int ActiveStudents { get; set; }
    public double CompletionRate { get; set; }
}

public class CreatorContentEngagementDto
{
    public int ContentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Views { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public double CompletionRate { get; set; }
}

public class CreatorMonthlyRevenueDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal Revenue { get; set; }
    public int NewEnrollments { get; set; }
}

public class EngagementTrendDto
{
    public DateTime Date { get; set; }
    public int ActiveStudents { get; set; }
    public double AverageEngagementTime { get; set; }
}

public class CreatorLearningPathRevenueDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Sales { get; set; }
    public decimal AveragePrice { get; set; }
}

public class EnrolledStudentDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public double Progress { get; set; }
    public DateTime LastActivity { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
}

public class GetLearningPathAnalyticsQuery : IRequest<Result<LearningPathAnalyticsDto>>
{
    public int LearningPathId { get; set; }
}
