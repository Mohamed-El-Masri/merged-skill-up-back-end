/*using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Dashboard.Queries;
using SkillUpPlatform.Application.Features.Progress.Queries;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SkillUpPlatform.Application.Features.Dashboard.Handlers
{
    public class GetStudentDashboardQueryHandler : IRequestHandler<GetStudentDashboardQuery, Result<DashboardStudentDashboardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStudentDashboardQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DashboardStudentDashboardDto>> Handle(GetStudentDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch data for Overview
                var overview = await GetDashboardOverview(request.UserId);
                // Fetch Recent Activities
                var recentActivities = await GetRecentActivities(request.UserId, 10);
                // Fetch Upcoming Deadlines
                var upcomingDeadlines = await GetUpcomingDeadlines(request.UserId, 7);
                // Fetch Recommendations
                var recommendations = await GetRecommendations(request.UserId, null, 5);
                // Fetch Recent Achievements
                var recentAchievements = await GetRecentAchievements(request.UserId);
                // Fetch Learning Streak
                var learningStreak = await GetLearningStreak(request.UserId);

                var dashboard = new DashboardStudentDashboardDto
                {
                    Overview = overview,
                    RecentActivity = recentActivities,
                    UpcomingDeadlines = upcomingDeadlines,
                    Recommendations = recommendations,
                    RecentAchievements = recentAchievements,
                    LearningStreak = learningStreak
                };

                return Result<DashboardStudentDashboardDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return Result<DashboardStudentDashboardDto>.Failure($"Failed to retrieve student dashboard: {ex.Message}");
            }
        }

        private async Task<DashboardOverviewDto> GetDashboardOverview(int userId)
        {
            var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(userId);
            var userProgress = await _unitOfWork.UserProgress.GetAllAsync();
            var userAssessments = await _unitOfWork.AssessmentResults.GetByUserIdAsync(userId);
            var notifications = await _unitOfWork.NotificationRepository.GetUnreadByUserIdAsync(userId);
            var achievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(userId);

            return new DashboardOverviewDto
            {
                TotalLearningPaths = userLearningPaths.Count(),
                CompletedLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.Completed),
                InProgressLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.InProgress),
                TotalAssessments = userAssessments.Count(),
                CompletedAssessments = userAssessments.Count(ar => ar.CompletedAt != null),
                TotalTimeSpentMinutes = userProgress.Where(up => up.UserId == userId).Sum(up => up.TimeSpentMinutes),
                UnreadNotifications = notifications.Count,
                TotalAchievements = achievements.Count,
                TotalPoints = achievements.Sum(a => a.Achievement?.Points ?? 0),
                OverallProgress = userLearningPaths.Any() ? userLearningPaths.Average(ulp => ulp.ProgressPercentage) : 0.0
            };
        }

        private async Task<List<RecentActivityDto>> GetRecentActivities(int userId, int limit)
        {
            var activities = await _unitOfWork.UserActivityRepository.GetRecentActivityAsync(userId, limit);
            return activities.Select(a => new RecentActivityDto
            {
                Id = a.Id,
                Type = a.ActivityType,
                Title = a.ActivityType switch
                {
                    "ContentCompleted" => "Content Completed",
                    "AssessmentCompleted" => "Assessment Completed",
                    _ => a.ActivityType
                },
                Description = a.Description,
                Timestamp = a.Timestamp,
                Icon = a.ActivityType switch
                {
                    "ContentCompleted" => "check-circle",
                    "AssessmentCompleted" => "award",
                    _ => "activity"
                },
                Color = a.ActivityType switch
                {
                    "ContentCompleted" => "green",
                    "AssessmentCompleted" => "blue",
                    _ => "gray"
                },
                ActionUrl = null // Add logic for specific action URLs if needed
            }).ToList();
        }

        private async Task<List<UpcomingDeadlineDto>> GetUpcomingDeadlines(int userId, int daysAhead)
        {
            var deadlineDate = DateTime.Now.AddDays(daysAhead);
            var assessments = await _unitOfWork.Assessments.GetAllAsync();
            var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(userId);

            var deadlines = assessments
                .Where(a => a.LearningPathId.HasValue && userLearningPaths.Any(ulp => ulp.LearningPathId == a.LearningPathId) && a.DueDate.HasValue && a.DueDate.Value <= deadlineDate)
                .Select(a => new UpcomingDeadlineDto
                {
                    Id = a.Id,
                    Type = "Assessment",
                    Title = a.Title,
                    Description = a.Description,
                    DueDate = a.DueDate ?? DateTime.Now,
                    DaysRemaining = (a.DueDate.Value - DateTime.Now).Days,
                    Priority = (a.DueDate.Value - DateTime.Now).Days <= 2 ? "High" : "Medium",
                    IsOverdue = a.DueDate < DateTime.Now
                })
                .ToList();

            return deadlines;
        }

        private async Task<List<RecommendationDto>> GetRecommendations(int userId, string? type, int limit)
        {
            var recommendations = await _unitOfWork.LearningPaths.GetRecommendedLearningPathsAsync(userId);
            return recommendations.Take(limit).Select(lp => new RecommendationDto
            {
                Id = lp.Id,
                Type = "LearningPath",
                Title = lp.Title,
                Description = lp.Description,
                ImageUrl = null, // Add image URL logic if available
                ActionUrl = $"/learning-paths/{lp.Id}",
                ActionText = "Start Learning",
                Score = 0.8, // Placeholder score
                Tags = lp.Category?.Split(',').ToList() ?? new List<string>()
            }).ToList();
        }

        private async Task<List<ProgressQueries_UserAchievementDto>> GetRecentAchievements(int userId)
        {
            var achievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(userId);
            return achievements
                .OrderByDescending(a => a.EarnedAt)
                .Take(5)
                .Select(a => new ProgressQueries_UserAchievementDto
                {
                    Id = a.AchievementId,
                    Name = a.Achievement?.Name ?? "",
                    Description = a.Achievement?.Description ?? "",
                    EarnedAt = a.EarnedAt
                    // Map other properties as needed
                })
                .ToList();
        }

        private async Task<LearningStreakDto> GetLearningStreak(int userId)
        {
            var activities = await _unitOfWork.UserActivityRepository.GetByUserIdAsync(userId);
            return CalculateCurrentStreak(activities);
        }

        private LearningStreakDto CalculateCurrentStreak(IEnumerable<UserActivity> activities)
        {
            var orderedActivities = activities
                .Where(a => a.ActivityType == "ContentCompleted" || a.ActivityType == "AssessmentCompleted")
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            if (!orderedActivities.Any())
            {
                return new LearningStreakDto
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Days = 0,
                    IsActive = false
                };
            }

            int streakDays = 1;
            var currentDate = orderedActivities.First().Timestamp.Date;
            var previousDate = currentDate;

            foreach (var activity in orderedActivities.Skip(1))
            {
                var activityDate = activity.Timestamp.Date;
                if ((previousDate - activityDate).Days == 1)
                {
                    streakDays++;
                    previousDate = activityDate;
                }
                else if ((previousDate - activityDate).Days > 1)
                {
                    break;
                }
            }
            
            return new LearningStreakDto
            {
                StartDate = previousDate,
                EndDate = currentDate,
                Days = streakDays,
                IsActive = currentDate == DateTime.Now.Date
            };
        }
    }

    public class GetDashboardOverviewQueryHandler : IRequestHandler<GetDashboardOverviewQuery, Result<DashboardOverviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDashboardOverviewQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DashboardOverviewDto>> Handle(GetDashboardOverviewQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);
                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);
                var userAssessments = await _unitOfWork.AssessmentResults.GetByUserIdAsync(request.UserId);
                var notifications = await _unitOfWork.NotificationRepository.GetUnreadByUserIdAsync(request.UserId);
                var achievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(request.UserId);

                var dashboard = new DashboardOverviewDto
                {
                    TotalLearningPaths = userLearningPaths.Count(),
                    CompletedLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.Completed),
                    InProgressLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.InProgress),
                    TotalAssessments = userAssessments.Count(),
                    CompletedAssessments = userAssessments.Count(ar => ar.CompletedAt != null),
                    TotalTimeSpentMinutes = userProgress.Sum(up => up.TimeSpentMinutes),
                    UnreadNotifications = notifications.Count,
                    TotalAchievements = achievements.Count,
                    TotalPoints = achievements.Sum(a => a.Achievement?.Points ?? 0),
                    OverallProgress = userLearningPaths.Any() ? userLearningPaths.Average(ulp => ulp.ProgressPercentage) : 0.0
                };

                return Result<DashboardOverviewDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return Result<DashboardOverviewDto>.Failure($"Failed to retrieve dashboard overview: {ex.Message}");
            }
        }
    }

    public class GetLearningProgressQueryHandler : IRequestHandler<GetLearningProgressQuery, Result<DashboardLearningProgressDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLearningProgressQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DashboardLearningProgressDto>> Handle(GetLearningProgressQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);
                var learningPaths = await _unitOfWork.LearningPaths.GetAllAsync();
                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);

                var progress = new DashboardLearningProgressDto
                {
                    TotalLearningPaths = userLearningPaths.Count(),
                    CompletedLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.Completed),
                    InProgressLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.InProgress),
                    OverallProgress = userLearningPaths.Any() ? userLearningPaths.Average(ulp => ulp.ProgressPercentage) : 0.0,
                    LearningPathProgress = userLearningPaths.Select(ulp => new DashboardLearningPathProgressDto
                    {
                        LearningPathId = ulp.LearningPathId,
                        Title = learningPaths.FirstOrDefault(lp => lp.Id == ulp.LearningPathId)?.Title ?? "",
                        Progress = ulp.ProgressPercentage,
                        TimeSpent = TimeSpan.FromMinutes(userProgress
                            .Where(up => up.Content.LearningPathId == ulp.LearningPathId)
                            .Sum(up => up.TimeSpentMinutes))
                    }).ToList()
                };

                return Result<DashboardLearningProgressDto>.Success(progress);
            }
            catch (Exception ex)
            {
                return Result<DashboardLearningProgressDto>.Failure($"Failed to retrieve learning progress: {ex.Message}");
            }
        }
    }

    public class GetAchievementsQueryHandler : IRequestHandler<GetAchievementsQuery, Result<List<DashboardAchievementDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAchievementsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<DashboardAchievementDto>>> Handle(GetAchievementsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userAchievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(request.UserId);

                var achievements = userAchievements
                    .OrderByDescending(ua => ua.EarnedAt)
                    .Take(request.Limit)
                    .Select(ua => new DashboardAchievementDto
                    {
                        Id = ua.AchievementId,
                        Name = ua.Achievement?.Name ?? "",
                        Description = ua.Achievement?.Description ?? "",
                        //BadgeUrl = ua.Achievement?.BadgeUrl ?? "",
                        DateEarned = ua.EarnedAt,
                        //Category = ua.Achievement?.Type ?? ""
                    })
                    .ToList();

                return Result<List<DashboardAchievementDto>>.Success(achievements);
            }
            catch (Exception ex)
            {
                return Result<List<DashboardAchievementDto>>.Failure($"Failed to retrieve achievements: {ex.Message}");
            }
        }
    }

    public class GetPersonalizedRecommendationsQueryHandler : IRequestHandler<GetPersonalizedRecommendationsQuery, Result<List<PersonalizedRecommendationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPersonalizedRecommendationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<PersonalizedRecommendationDto>>> Handle(GetPersonalizedRecommendationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Expression<Func<LearningPath, bool>>? categoryFilter = string.IsNullOrEmpty(request.Category)
                    ? null
                    : lp => lp.Category == request.Category;

                var recommendations = await _unitOfWork.LearningPaths.GetRecommendedLearningPathsAsync(request.UserId);

                var result = recommendations
                    .Where(lp => categoryFilter == null || categoryFilter.Compile()(lp))
                    .Take(request.Limit)
                    .Select(lp => new PersonalizedRecommendationDto
                    {
                        Id = lp.Id,
                        Type = "LearningPath",
                        Title = lp.Title,
                        Description = lp.Description,
                        RecommendationScore = CalculateRecommendationScore(lp, request.UserId),
                        Reason = $"Recommended based on your learning history and interests in {lp.Category}"
                    })
                    .ToList();

                return Result<List<PersonalizedRecommendationDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<PersonalizedRecommendationDto>>.Failure($"Failed to retrieve recommendations: {ex.Message}");
            }
        }

        private double CalculateRecommendationScore(LearningPath learningPath, int userId)
        {
            // Placeholder for recommendation scoring logic based on user progress and interests
            return 0.8;
        }
    }

    public class GetLearningCalendarQueryHandler : IRequestHandler<GetLearningCalendarQuery, Result<LearningCalendarDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLearningCalendarQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<LearningCalendarDto>> Handle(GetLearningCalendarQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var targetDate = request.Month.HasValue && request.Year.HasValue
                    ? new DateTime(request.Year.Value, request.Month.Value, 1)
                    : DateTime.Now;

                var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);
                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);
                var assessments = await _unitOfWork.Assessments.FindAsync(a => a.IsActive);

                var calendar = new LearningCalendarDto
                {
                    Events = assessments
                        .Where(a => a.DueDate.HasValue && a.DueDate.Value.Year == targetDate.Year && a.DueDate.Value.Month == targetDate.Month)
                        .Select(a => new CalendarEventDto
                        {
                            Id = Guid.NewGuid(),
                            Title = a.Title,
                            Description = a.Description,
                            Date = a.DueDate ?? DateTime.Now,
                            Type = "Assessment",
                            Color = a.Category switch
                            {
                                "Technical" => "blue",
                                "Soft Skills" => "green",
                                _ => "purple"
                            }
                        })
                        .ToList(),
                    StudyStreaks = await CalculateStudyStreaks(request.UserId),
                    DailyProgress = userProgress
                        .Where(up => up.CompletedAt.HasValue && up.CompletedAt.Value.Year == targetDate.Year && up.CompletedAt.Value.Month == targetDate.Month)
                        .GroupBy(up => up.CompletedAt!.Value.Date)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Count(up => up.IsCompleted))
                };

                return Result<LearningCalendarDto>.Success(calendar);
            }
            catch (Exception ex)
            {
                return Result<LearningCalendarDto>.Failure($"Failed to retrieve learning calendar: {ex.Message}");
            }
        }

        private async Task<List<StudyStreakDto>> CalculateStudyStreaks(int userId)
        {
            var activities = await _unitOfWork.UserActivityRepository.GetByUserIdAsync(userId);
            var streaks = new List<StudyStreakDto>();
            var orderedActivities = activities
                .Where(a => a.ActivityType == "ContentCompleted" || a.ActivityType == "AssessmentCompleted")
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            if (!orderedActivities.Any())
            {
                return streaks;
            }

            var currentStreak = new StudyStreakDto
            {
                StartDate = orderedActivities.First().Timestamp.Date,
                EndDate = orderedActivities.First().Timestamp.Date,
                Days = 1,
                IsActive = orderedActivities.First().Timestamp.Date == DateTime.Now.Date
            };

            var previousDate = currentStreak.StartDate;
            foreach (var activity in orderedActivities.Skip(1))
            {
                var activityDate = activity.Timestamp.Date;
                if ((previousDate - activityDate).Days == 1)
                {
                    currentStreak.Days++;
                    currentStreak.StartDate = activityDate;
                    previousDate = activityDate;
                }
                else if ((previousDate - activityDate).Days > 1)
                {
                    break;
                }
            }

            streaks.Add(currentStreak);
            return streaks;
        }
    }

    public class GetRecentActivitiesQueryHandler : IRequestHandler<GetRecentActivitiesQuery, Result<List<ActivityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRecentActivitiesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ActivityDto>>> Handle(GetRecentActivitiesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var activities = await _unitOfWork.UserActivityRepository.GetRecentActivityAsync(request.UserId, request.Limit);

                var result = activities.Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Type = a.ActivityType,
                    Description = a.Description,
                    CreatedAt = a.Timestamp,
                    RelatedEntity = a.AdditionalData.ContainsKey("EntityType") ? a.AdditionalData["EntityType"].ToString() : null,
                    RelatedEntityId = a.AdditionalData.ContainsKey("EntityId") ? Convert.ToInt32(a.AdditionalData["EntityId"]) : null
                }).ToList();

                return Result<List<ActivityDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<ActivityDto>>.Failure($"Failed to retrieve recent activities: {ex.Message}");
            }
        }
    }

    public class GetLearningStatisticsQueryHandler : IRequestHandler<GetLearningStatisticsQuery, Result<LearningStatisticsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLearningStatisticsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<LearningStatisticsDto>> Handle(GetLearningStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var startDate = request.Period.ToLower() switch
                {
                    "weekly" => DateTime.Now.AddDays(-7),
                    "yearly" => DateTime.Now.AddYears(-1),
                    _ => DateTime.Now.AddMonths(-1) // monthly default
                };

                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId && up.CompletedAt >= startDate);
                var sessions = await _unitOfWork.UserSessions.GetActiveSessionsAsync(request.UserId);

                var totalTime = userProgress.Sum(up => up.TimeSpentMinutes);
                var totalSessions = sessions.Count;

                var stats = new LearningStatisticsDto
                {
                    TotalTimeSpent = TimeSpan.FromMinutes(totalTime),
                    TotalSessions = totalSessions,
                    AverageSessionTime = totalSessions > 0 ? totalTime / totalSessions : 0,
                    CompletedContents = userProgress.Count(up => up.IsCompleted),
                    TotalContents = userProgress.Count(),
                    CompletionRate = userProgress.Any() ? (double)userProgress.Count(up => up.IsCompleted) / userProgress.Count() * 100 : 0,
                    DailyStats = userProgress
                        .Where(up => up.CompletedAt.HasValue)
                        .GroupBy(up => up.CompletedAt!.Value.Date)
                        .Select(g => new DailyStatisticDto
                        {
                            Date = g.Key,
                            TimeSpent = TimeSpan.FromMinutes(g.Sum(up => up.TimeSpentMinutes)),
                            CompletedContents = g.Count(up => up.IsCompleted),
                            Sessions = g.Count()
                        })
                        .ToList()
                };

                return Result<LearningStatisticsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                return Result<LearningStatisticsDto>.Failure($"Failed to retrieve learning statistics: {ex.Message}");
            }
        }
    }

    public class GetStudyStreakQueryHandler : IRequestHandler<GetStudyStreakQuery, Result<StudyStreakDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStudyStreakQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StudyStreakDto>> Handle(GetStudyStreakQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var activities = await _unitOfWork.UserActivityRepository.GetByUserIdAsync(request.UserId);
                var streak = CalculateCurrentStreak(activities);

                return Result<StudyStreakDto>.Success(streak);
            }
            catch (Exception ex)
            {
                return Result<StudyStreakDto>.Failure($"Failed to retrieve study streak: {ex.Message}");
            }
        }

        private StudyStreakDto CalculateCurrentStreak(IEnumerable<UserActivity> activities)
        {
            var orderedActivities = activities
                .Where(a => a.ActivityType == "ContentCompleted" || a.ActivityType == "AssessmentCompleted")
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            if (!orderedActivities.Any())
            {
                return new StudyStreakDto
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Days = 0,
                    IsActive = false
                };
            }

            int streakDays = 1;
            var currentDate = orderedActivities.First().Timestamp.Date;
            var previousDate = currentDate;

            foreach (var activity in orderedActivities.Skip(1))
            {
                var activityDate = activity.Timestamp.Date;
                if ((previousDate - activityDate).Days == 1)
                {
                    streakDays++;
                    previousDate = activityDate;
                }
                else if ((previousDate - activityDate).Days > 1)
                {
                    break;
                }
            }

            return new StudyStreakDto
            {
                StartDate = previousDate,
                EndDate = currentDate,
                Days = streakDays,
                IsActive = currentDate == DateTime.Now.Date
            };
        }
    }
}*/
/*
using AutoMapper;
using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Dashboard.Queries;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SkillUpPlatform.Application.Features.Dashboard.Handlers
{
    public class GetStudentDashboardQueryHandler : IRequestHandler<GetStudentDashboardQuery, Result<DashboardStudentDashboardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetStudentDashboardQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DashboardStudentDashboardDto>> Handle(GetStudentDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var overview = await GetDashboardOverviewAsync(request.UserId);
                var recentActivities = await GetRecentActivitiesAsync(request.UserId, 10);
                var upcomingDeadlines = await GetUpcomingDeadlinesAsync(request.UserId, 7);
                var recommendations =  GetRecommendations(request.UserId, null, 5);
                var recentAchievements = await GetRecentAchievements(request.UserId);
                var learningStreak = await GetLearningStreak(request.UserId);

                var dashboard = new DashboardStudentDashboardDto
                {
                    Overview = overview,
                    RecentActivity = recentActivities,
                    UpcomingDeadlines = upcomingDeadlines,
                    Recommendations = recommendations,
                    RecentAchievements = recentAchievements,
                    LearningStreak = learningStreak
                };

                return Result<DashboardStudentDashboardDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return Result<DashboardStudentDashboardDto>.Failure($"Failed to retrieve student dashboard: {ex.Message}");
            }
        }

        private async Task<DashboardOverviewDto> GetDashboardOverviewAsync(int userId)
        {
            var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(userId);
            var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == userId);
            var userAssessments = await _unitOfWork.AssessmentResults.GetByUserIdAsync(userId);
            var notifications = await _unitOfWork.NotificationRepository.GetUnreadByUserIdAsync(userId);
            var achievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(userId);

            return new DashboardOverviewDto
            {
                TotalLearningPaths = userLearningPaths.Count(),
                CompletedLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.Completed),
                InProgressLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.InProgress),
                TotalAssessments = userAssessments.Count(),
                CompletedAssessments = userAssessments.Count(ar => ar.CompletedAt != null),
                TotalTimeSpentMinutes = userProgress.Sum(up => up.TimeSpentMinutes),
                UnreadNotifications = notifications.Count,
                TotalAchievements = achievements.Count,
                TotalPoints = achievements.Sum(a => a.Achievement?.Points ?? 0),
                OverallProgress = userLearningPaths.Any() ? userLearningPaths.Average(ulp => ulp.ProgressPercentage) : 0.0
            };
        }

        private async Task<List<RecentActivityDto>> GetRecentActivitiesAsync(int userId, int limit)
        {
            var activities = await _unitOfWork.UserActivityRepository.GetRecentActivityAsync(userId, limit);
            return _mapper.Map<List<RecentActivityDto>>(activities);
        }

        private async Task<List<UpcomingDeadlineDto>> GetUpcomingDeadlinesAsync(int userId, int daysAhead)
        {
            var deadlineDate = DateTime.Now.AddDays(daysAhead);
            var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(userId);
            var assessments = await _unitOfWork.Assessments.GetAllAsync();

            var deadlines = new List<UpcomingDeadlineDto>();
            foreach (var ulp in userLearningPaths.Where(ulp => ulp.Status != LearningPathStatus.Completed))
            {
                var relatedAssessments = assessments.Where(a => a.LearningPathId == ulp.LearningPathId).ToList();
                foreach (var assessment in relatedAssessments)
                {
                    var deadlineDto = _mapper.Map<UpcomingDeadlineDto>(assessment);
                    deadlineDto.DueDate = ulp.EnrolledAt.AddDays(30);
                    deadlineDto.DaysRemaining = (deadlineDto.DueDate - DateTime.Now).Days;
                    deadlineDto.Priority = deadlineDto.DaysRemaining <= 2 ? "High" : "Medium";
                    deadlineDto.IsOverdue = deadlineDto.DueDate < DateTime.Now;
                    deadlines.Add(deadlineDto);
                }
            }

            return deadlines.Where(d => d.DueDate <= deadlineDate).Take(5).ToList();
        }



        public async Task<Result<DashboardOverviewDto>> Handle(GetDashboardOverviewQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);
                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);
                var userAssessments = await _unitOfWork.AssessmentResults.GetByUserIdAsync(request.UserId);
                var notifications = await _unitOfWork.NotificationRepository.GetUnreadByUserIdAsync(request.UserId);
                var achievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(request.UserId);

                var dashboard = new DashboardOverviewDto
                {
                    TotalLearningPaths = userLearningPaths.Count(),
                    CompletedLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.Completed),
                    InProgressLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.InProgress),
                    TotalAssessments = userAssessments.Count(),
                    CompletedAssessments = userAssessments.Count(ar => ar.CompletedAt != null),
                    TotalTimeSpentMinutes = userProgress.Sum(up => up.TimeSpentMinutes),
                    UnreadNotifications = notifications.Count,
                    TotalAchievements = achievements.Count,
                    TotalPoints = achievements.Sum(a => a.Achievement?.Points ?? 0),
                    OverallProgress = userLearningPaths.Any() ? userLearningPaths.Average(ulp => ulp.ProgressPercentage) : 0.0
                };

                return Result<DashboardOverviewDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return Result<DashboardOverviewDto>.Failure($"Failed to retrieve dashboard overview: {ex.Message}");
            }
        }
    }

    public class GetLearningProgressQueryHandler : IRequestHandler<GetLearningProgressQuery, Result<DashboardLearningProgressDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetLearningProgressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DashboardLearningProgressDto>> Handle(GetLearningProgressQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);
                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);

                var progress = new DashboardLearningProgressDto
                {
                    TotalLearningPaths = userLearningPaths.Count(),
                    CompletedLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.Completed),
                    InProgressLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.InProgress),
                    OverallProgress = userLearningPaths.Any() ? userLearningPaths.Average(ulp => ulp.ProgressPercentage) : 0.0,
                    LearningPathProgress = _mapper.Map<List<DashboardLearningPathProgressDto>>(userLearningPaths)
                };

                // Calculate TimeSpent for each LearningPathProgress
                foreach (var lpProgress in progress.LearningPathProgress)
                {
                    lpProgress.TimeSpent = TimeSpan.FromMinutes(userProgress
                        .Where(up => up.Content.LearningPathId == lpProgress.LearningPathId)
                        .Sum(up => up.TimeSpentMinutes));
                }

                return Result<DashboardLearningProgressDto>.Success(progress);
            }
            catch (Exception ex)
            {
                return Result<DashboardLearningProgressDto>.Failure($"Failed to retrieve learning progress: {ex.Message}");
            }
        }
    }

    public class GetAchievementsQueryHandler : IRequestHandler<GetAchievementsQuery, Result<List<DashboardAchievementDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAchievementsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<DashboardAchievementDto>>> Handle(GetAchievementsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userAchievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(request.UserId);
                var achievements = _mapper.Map<List<DashboardAchievementDto>>(userAchievements
                    .OrderByDescending(ua => ua.EarnedAt)
                    .Take(request.Limit));

                return Result<List<DashboardAchievementDto>>.Success(achievements);
            }
            catch (Exception ex)
            {
                return Result<List<DashboardAchievementDto>>.Failure($"Failed to retrieve achievements: {ex.Message}");
            }
        }
    }

    public class GetPersonalizedRecommendationsQueryHandler : IRequestHandler<GetPersonalizedRecommendationsQuery, Result<List<PersonalizedRecommendationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPersonalizedRecommendationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<PersonalizedRecommendationDto>>> Handle(GetPersonalizedRecommendationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Expression<Func<LearningPath, bool>>? categoryFilter = string.IsNullOrEmpty(request.Category)
                    ? null
                    : lp => lp.Category == request.Category;

                var recommendations = await _unitOfWork.LearningPaths.GetRecommendedLearningPathsAsync(request.UserId);
                var filteredRecommendations = recommendations
                    .Where(lp => categoryFilter == null || categoryFilter.Compile()(lp))
                    .Take(request.Limit)
                    .ToList();

                var result = _mapper.Map<List<PersonalizedRecommendationDto>>(filteredRecommendations);
                foreach (var recommendation in result)
                {
                    recommendation.RecommendationScore = CalculateRecommendationScore(recommendation, request.UserId);
                    recommendation.Reason = $"Recommended based on your learning history and interests in {recommendation.Type}";
                }

                return Result<List<PersonalizedRecommendationDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<PersonalizedRecommendationDto>>.Failure($"Failed to retrieve recommendations: {ex.Message}");
            }
        }

        private double CalculateRecommendationScore(PersonalizedRecommendationDto recommendation, int userId)
        {
            // Placeholder for recommendation scoring logic
            return 0.8;
        }
    }

    public class GetLearningCalendarQueryHandler : IRequestHandler<GetLearningCalendarQuery, Result<LearningCalendarDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetLearningCalendarQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<LearningCalendarDto>> Handle(GetLearningCalendarQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var targetDate = request.Month.HasValue && request.Year.HasValue
                    ? new DateTime(request.Year.Value, request.Month.Value, 1)
                    : DateTime.Now;

                var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);
                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);
                var assessments = await _unitOfWork.Assessments.FindAsync(a => a.IsActive);

                var calendar = new LearningCalendarDto
                {
                    Events = _mapper.Map<List<CalendarEventDto>>(assessments)
                        .Where(a => a.DueDate.HasValue && a.DueDate.Value.Year == targetDate.Year && a.DueDate.Value.Month == targetDate.Month)),
                    StudyStreaks = await CalculateStudyStreaks(request.UserId),
                    DailyProgress = userProgress
                        .Where(up => up.CompletedAt.HasValue && up.CompletedAt.Value.Year == targetDate.Year && up.CompletedAt.Value.Month == targetDate.Month)
                        .GroupBy(up => up.CompletedAt!.Value.Date)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Count(up => up.IsCompleted))
                };

                return Result<LearningCalendarDto>.Success(calendar);
            }
            catch (Exception ex)
            {
                return Result<LearningCalendarDto>.Failure($"Failed to retrieve learning calendar: {ex.Message}");
            }
        }

        private async Task<List<StudyStreakDto>> CalculateStudyStreaks(int userId)
        {
            var activities = await _unitOfWork.UserActivityRepository.GetByUserIdAsync(userId);
            var streaks = new List<StudyStreakDto>();
            var orderedActivities = activities
                .Where(a => a.ActivityType == "ContentCompleted" || a.ActivityType == "AssessmentCompleted")
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            if (!orderedActivities.Any())
            {
                return streaks;
            }

            var currentStreak = CalculateCurrentStreak(orderedActivities);
            streaks.Add(currentStreak);
            return streaks;
        }

        private StudyStreakDto CalculateCurrentStreak(IEnumerable<UserActivity> activities)
        {
            var orderedActivities = activities.ToList();
            if (!orderedActivities.Any())
            {
                return new StudyStreakDto
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    Days = 0,
                    IsActive = false
                };
            }

            int streakDays = 1;
            var currentDate = orderedActivities.First().Timestamp.Date;
            var previousDate = currentDate;

            foreach (var activity in orderedActivities.Skip(1))
            {
                var activityDate = activity.Timestamp.Date;
                if ((previousDate - activityDate).Days == 1)
                {
                    streakDays++;
                    previousDate = activityDate;
                }
                else if ((previousDate - activityDate).Days > 1)
                {
                    break;
                }
            }

            return new StudyStreakDto
            {
                StartDate = previousDate,
                EndDate = currentDate,
                Days = streakDays,
                IsActive = currentDate == DateTime.Today
            };
        }
    }

    public class GetRecentActivitiesQueryHandler : IRequestHandler<GetRecentActivitiesQuery, Result<List<ActivityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRecentActivitiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<ActivityDto>>> Handle(GetRecentActivitiesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var activities = await _unitOfWork.UserActivityRepository.GetRecentActivityAsync(request.UserId, request.Limit);
                var result = _mapper.Map<List<ActivityDto>>(activities);

                return Result<List<ActivityDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<ActivityDto>>.Failure($"Failed to retrieve recent activities: {ex.Message}");
            }
        }
    }

    public class GetLearningStatisticsQueryHandler : IRequestHandler<GetLearningStatisticsQuery, Result<LearningStatisticsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetLearningStatisticsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<LearningStatisticsDto>> Handle(GetLearningStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var startDate = request.Period.ToLower() switch
                {
                    "weekly" => DateTime.Now.AddDays(-7),
                    "yearly" => DateTime.Now.AddYears(-1),
                    _ => DateTime.Now.AddMonths(-1)
                };

                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId && up.CompletedAt >= startDate);
                var sessions = await _unitOfWork.UserSessions.GetActiveSessionsAsync(request.UserId);

                var totalTime = userProgress.Sum(up => up.TimeSpentMinutes);
                var totalSessions = sessions.Count;

                var stats = new LearningStatisticsDto
                {
                    TotalTimeSpent = TimeSpan.FromMinutes(totalTime),
                    TotalSessions = totalSessions,
                    AverageSessionTime = totalSessions > 0 ? totalTime / totalSessions : 0,
                    CompletedContents = userProgress.Count(up => up.IsCompleted),
                    TotalContents = userProgress.Count(),
                    CompletionRate = userProgress.Any() ? (double)userProgress.Count(up => up.IsCompleted) / userProgress.Count() * 100 : 0,
                    DailyStats = userProgress
                        .Where(up => up.CompletedAt.HasValue)
                        .GroupBy(up => up.CompletedAt!.Value.Date)
                        .Select(g => new DailyStatisticDto
                        {
                            Date = g.Key,
                            TimeSpent = TimeSpan.FromMinutes(g.Sum(up => up.TimeSpentMinutes)),
                            CompletedContents = g.Count(up => up.IsCompleted),
                            Sessions = g.Count()
                        })
                        .ToList()
                };

                return Result<LearningStatisticsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                return Result<LearningStatisticsDto>.Failure($"Failed to retrieve learning statistics: {ex.Message}");
            }
        }
    }

    public class GetStudyStreakQueryHandler : IRequestHandler<GetStudyStreakQuery, Result<StudyStreakDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetStudyStreakQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<StudyStreakDto>> Handle(GetStudyStreakQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var activities = await _unitOfWork.UserActivityRepository.GetByUserIdAsync(request.UserId);
                var streak = CalculateCurrentStreak(activities);

                return Result<StudyStreakDto>.Success(streak);
            }
            catch (Exception ex)
            {
                return Result<StudyStreakDto>.Failure($"Failed to retrieve study streak: {ex.Message}");
            }
        }

        private StudyStreakDto CalculateCurrentStreak(IEnumerable<UserActivity> activities)
        {
            var orderedActivities = activities
                .Where(a => a.ActivityType == "ContentCompleted" || a.ActivityType == "AssessmentCompleted")
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            if (!orderedActivities.Any())
            {
                return new StudyStreakDto
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    Days = 0,
                    IsActive = false
                };
            }

            int streakDays = 1;
            var currentDate = orderedActivities.First().Timestamp.Date;
            var previousDate = currentDate;

            foreach (var activity in orderedActivities.Skip(1))
            {
                var activityDate = activity.Timestamp.Date;
                if ((previousDate - activityDate).Days == 1)
                {
                    streakDays++;
                    previousDate = activityDate;
                }
                else if ((previousDate - activityDate).Days > 1)
                {
                    break;
                }
            }

            return new StudyStreakDto
            {
                StartDate = previousDate,
                EndDate = currentDate,
                Days = streakDays,
                IsActive = currentDate == DateTime.Today
            };
        }
    }

}*/
using AutoMapper;
using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Dashboard.Queries;
using SkillUpPlatform.Application.Features.Progress.Queries;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SkillUpPlatform.Application.Features.Dashboard.Handlers
{
    public class GetStudentDashboardQueryHandler : IRequestHandler<GetStudentDashboardQuery, Result<DashboardStudentDashboardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetStudentDashboardQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DashboardStudentDashboardDto>> Handle(GetStudentDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var overview = await GetDashboardOverview(request.UserId);
                var recentActivities = await GetRecentActivities(request.UserId, 10);
                var upcomingDeadlines = await GetUpcomingDeadlines(request.UserId, 7);
                var recommendations = await GetRecommendations(request.UserId, null, 5);
                var recentAchievements = await GetRecentAchievements(request.UserId);
                var learningStreak = await GetLearningStreak(request.UserId);

                var dashboard = new DashboardStudentDashboardDto
                {
                    Overview = overview,
                    RecentActivity = recentActivities,
                    UpcomingDeadlines = upcomingDeadlines,
                    Recommendations = recommendations,
                    RecentAchievements = recentAchievements,
                    LearningStreak = learningStreak
                };

                return Result<DashboardStudentDashboardDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return Result<DashboardStudentDashboardDto>.Failure($"Failed to retrieve student dashboard: {ex.Message}");
            }
        }

        private async Task<DashboardOverviewDto> GetDashboardOverview(int userId)
        {
            var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(userId);
            var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == userId);
            var userAssessments = await _unitOfWork.AssessmentResults.GetByUserIdAsync(userId);
            var notifications = await _unitOfWork.NotificationRepository.GetUnreadByUserIdAsync(userId);
            var achievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(userId);

            return new DashboardOverviewDto
            {
                TotalLearningPaths = userLearningPaths.Count(),
                CompletedLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.Completed),
                InProgressLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.InProgress),
                TotalAssessments = userAssessments.Count(),
                CompletedAssessments = userAssessments.Count(ar => ar.CompletedAt != null),
                TotalTimeSpentMinutes = userProgress.Sum(up => up.TimeSpentMinutes),
                UnreadNotifications = notifications.Count,
                TotalAchievements = achievements.Count,
                TotalPoints = achievements.Sum(a => a.Achievement?.Points ?? 0),
                OverallProgress = userLearningPaths.Any() ? userLearningPaths.Average(ulp => ulp.ProgressPercentage) : 0.0
            };
        }

        private async Task<List<RecentActivityDto>> GetRecentActivities(int userId, int limit)
        {
            var activities = await _unitOfWork.UserActivityRepository.GetRecentActivityAsync(userId, limit);
            return _mapper.Map<List<RecentActivityDto>>(activities);
        }

        private async Task<List<UpcomingDeadlineDto>> GetUpcomingDeadlines(int userId, int daysAhead)
        {
            var deadlineDate = DateTime.Now.AddDays(daysAhead);
            var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(userId);
            var assessments = await _unitOfWork.Assessments.GetAllAsync();

            var deadlines = new List<UpcomingDeadlineDto>();
            foreach (var ulp in userLearningPaths.Where(ulp => ulp.Status != LearningPathStatus.Completed))
            {
                var relatedAssessments = assessments.Where(a => a.LearningPathId == ulp.LearningPathId).ToList();
                foreach (var assessment in relatedAssessments)
                {
                    var deadlineDto = _mapper.Map<UpcomingDeadlineDto>(assessment);
                    deadlineDto.DueDate = ulp.EnrolledAt.AddDays(30);
                    deadlineDto.DaysRemaining = (deadlineDto.DueDate - DateTime.Now).Days;
                    deadlineDto.Priority = deadlineDto.DaysRemaining <= 2 ? "High" : "Medium";
                    deadlineDto.IsOverdue = deadlineDto.DueDate < DateTime.Now;
                    deadlines.Add(deadlineDto);
                }
            }

            return deadlines.Where(d => d.DueDate <= deadlineDate).Take(5).ToList();
        }

        private async Task<List<RecommendationDto>> GetRecommendations(int userId, string? type, int limit)
        {
            var recommendations = await _unitOfWork.LearningPaths.GetRecommendedLearningPathsAsync(userId);
            var result = _mapper.Map<List<RecommendationDto>>(recommendations.Take(limit));
            foreach (var recommendation in result)
            {
                recommendation.ImageUrl = null;
                recommendation.ActionUrl = $"/learning-paths/{recommendation.Id}";
                recommendation.ActionText = "Start Learning";
                recommendation.Score = 0.8;
                recommendation.Tags = recommendations.FirstOrDefault(r => r.Id == recommendation.Id)?.Category?.Split(',').ToList() ?? new List<string>();
            }
            return result;
        }

        private async Task<List<ProgressQueries_UserAchievementDto>> GetRecentAchievements(int userId)
        {
            var achievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(userId);
            return _mapper.Map<List<ProgressQueries_UserAchievementDto>>(achievements
                .OrderByDescending(a => a.EarnedAt)
                .Take(5));
        }

        private async Task<StudyStreakDto> GetLearningStreak(int userId)
        {
            var activities = await _unitOfWork.UserActivityRepository.GetByUserIdAsync(userId);
            return CalculateCurrentStreak(activities);
        }

        private StudyStreakDto CalculateCurrentStreak(IEnumerable<UserActivity> activities)
        {
            var orderedActivities = activities
                .Where(a => a.ActivityType == "ContentCompleted" || a.ActivityType == "AssessmentCompleted")
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            if (!orderedActivities.Any())
            {
                return new StudyStreakDto
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    Days = 0,
                    IsActive = false
                };
            }

            int streakDays = 1;
            var currentDate = orderedActivities.First().Timestamp.Date;
            var previousDate = currentDate;

            foreach (var activity in orderedActivities.Skip(1))
            {
                var activityDate = activity.Timestamp.Date;
                if ((previousDate - activityDate).Days == 1)
                {
                    streakDays++;
                    previousDate = activityDate;
                }
                else if ((previousDate - activityDate).Days > 1)
                {
                    break;
                }
            }

            return new StudyStreakDto
            {
                StartDate = previousDate,
                EndDate = currentDate,
                Days = streakDays,
                IsActive = currentDate == DateTime.Today
            };
        }
    }

    public class GetDashboardOverviewQueryHandler : IRequestHandler<GetDashboardOverviewQuery, Result<DashboardOverviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDashboardOverviewQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DashboardOverviewDto>> Handle(GetDashboardOverviewQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);
                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);
                var userAssessments = await _unitOfWork.AssessmentResults.GetByUserIdAsync(request.UserId);
                var notifications = await _unitOfWork.NotificationRepository.GetUnreadByUserIdAsync(request.UserId);
                var achievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(request.UserId);

                var dashboard = new DashboardOverviewDto
                {
                    TotalLearningPaths = userLearningPaths.Count(),
                    CompletedLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.Completed),
                    InProgressLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.InProgress),
                    TotalAssessments = userAssessments.Count(),
                    CompletedAssessments = userAssessments.Count(ar => ar.CompletedAt != null),
                    TotalTimeSpentMinutes = userProgress.Sum(up => up.TimeSpentMinutes),
                    UnreadNotifications = notifications.Count,
                    TotalAchievements = achievements.Count,
                    TotalPoints = achievements.Sum(a => a.Achievement?.Points ?? 0),
                    OverallProgress = userLearningPaths.Any() ? userLearningPaths.Average(ulp => ulp.ProgressPercentage) : 0.0
                };

                return Result<DashboardOverviewDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return Result<DashboardOverviewDto>.Failure($"Failed to retrieve dashboard overview: {ex.Message}");
            }
        }
    }

    public class GetLearningProgressQueryHandler : IRequestHandler<GetLearningProgressQuery, Result<DashboardLearningProgressDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetLearningProgressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DashboardLearningProgressDto>> Handle(GetLearningProgressQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);
                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);

                var progress = new DashboardLearningProgressDto
                {
                    TotalLearningPaths = userLearningPaths.Count(),
                    CompletedLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.Completed),
                    InProgressLearningPaths = userLearningPaths.Count(ulp => ulp.Status == LearningPathStatus.InProgress),
                    OverallProgress = userLearningPaths.Any() ? userLearningPaths.Average(ulp => ulp.ProgressPercentage) : 0.0,
                    LearningPathProgress = _mapper.Map<List<DashboardLearningPathProgressDto>>(userLearningPaths)
                };

                foreach (var lpProgress in progress.LearningPathProgress)
                {
                    lpProgress.TimeSpent = TimeSpan.FromMinutes(userProgress
                        .Where(up => up.Content.LearningPathId == lpProgress.LearningPathId)
                        .Sum(up => up.TimeSpentMinutes));
                }

                return Result<DashboardLearningProgressDto>.Success(progress);
            }
            catch (Exception ex)
            {
                return Result<DashboardLearningProgressDto>.Failure($"Failed to retrieve learning progress: {ex.Message}");
            }
        }
    }

    public class GetAchievementsQueryHandler : IRequestHandler<GetAchievementsQuery, Result<List<DashboardAchievementDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAchievementsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<DashboardAchievementDto>>> Handle(GetAchievementsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userAchievements = await _unitOfWork.AchievementRepository.GetUserAchievementsAsync(request.UserId);
                var achievements = _mapper.Map<List<DashboardAchievementDto>>(userAchievements
                    .OrderByDescending(ua => ua.EarnedAt)
                    .Take(request.Limit));

                return Result<List<DashboardAchievementDto>>.Success(achievements);
            }
            catch (Exception ex)
            {
                return Result<List<DashboardAchievementDto>>.Failure($"Failed to retrieve achievements: {ex.Message}");
            }
        }
    }

    public class GetPersonalizedRecommendationsQueryHandler : IRequestHandler<GetPersonalizedRecommendationsQuery, Result<List<PersonalizedRecommendationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPersonalizedRecommendationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<PersonalizedRecommendationDto>>> Handle(GetPersonalizedRecommendationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Expression<Func<LearningPath, bool>>? categoryFilter = string.IsNullOrEmpty(request.Category)
                    ? null
                    : lp => lp.Category == request.Category;

                var recommendations = await _unitOfWork.LearningPaths.GetRecommendedLearningPathsAsync(request.UserId);
                var filteredRecommendations = recommendations
                    .Where(lp => categoryFilter == null || categoryFilter.Compile()(lp))
                    .Take(request.Limit)
                    .ToList();

                var result = _mapper.Map<List<PersonalizedRecommendationDto>>(filteredRecommendations);
                foreach (var recommendation in result)
                {
                    recommendation.RecommendationScore = CalculateRecommendationScore(recommendation, request.UserId);
                    recommendation.Reason = $"Recommended based on your learning history and interests in {recommendation.Type}";
                }

                return Result<List<PersonalizedRecommendationDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<PersonalizedRecommendationDto>>.Failure($"Failed to retrieve recommendations: {ex.Message}");
            }
        }

        private double CalculateRecommendationScore(PersonalizedRecommendationDto recommendation, int userId)
        {
            return 0.8;
        }
    }

    public class GetLearningCalendarQueryHandler : IRequestHandler<GetLearningCalendarQuery, Result<LearningCalendarDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetLearningCalendarQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<LearningCalendarDto>> Handle(GetLearningCalendarQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var targetDate = request.Month.HasValue && request.Year.HasValue
                    ? new DateTime(request.Year.Value, request.Month.Value, 1)
                    : DateTime.Now;

                var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);
                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);
                var assessments = await _unitOfWork.Assessments.FindAsync(a => a.IsActive);

                var calendar = new LearningCalendarDto
                {
                    Events = assessments
                        .Where(a => userLearningPaths.Any(ulp => ulp.LearningPathId == a.LearningPathId && ulp.Status != LearningPathStatus.Completed))
                        .Select(a =>
                        {
                            var eventDto = _mapper.Map<CalendarEventDto>(a);
                            var relatedUlp = userLearningPaths.FirstOrDefault(ulp => ulp.LearningPathId == a.LearningPathId);
                            eventDto.Date = relatedUlp != null ? relatedUlp.EnrolledAt.AddDays(30) : DateTime.Now;
                            return eventDto;
                        })
                        .Where(e => e.Date.Year == targetDate.Year && e.Date.Month == targetDate.Month)
                        .ToList(),
                    StudyStreaks = await CalculateStudyStreaks(request.UserId),
                    DailyProgress = userProgress
                        .Where(up => up.CompletedAt.HasValue && up.CompletedAt.Value.Year == targetDate.Year && up.CompletedAt.Value.Month == targetDate.Month)
                        .GroupBy(up => up.CompletedAt!.Value.Date)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Count(up => up.IsCompleted))
                };

                return Result<LearningCalendarDto>.Success(calendar);
            }
            catch (Exception ex)
            {
                return Result<LearningCalendarDto>.Failure($"Failed to retrieve learning calendar: {ex.Message}");
            }
        }

        private async Task<List<StudyStreakDto>> CalculateStudyStreaks(int userId)
        {
            var activities = await _unitOfWork.UserActivityRepository.GetByUserIdAsync(userId);
            var streaks = new List<StudyStreakDto>();
            var orderedActivities = activities
                .Where(a => a.ActivityType == "ContentCompleted" || a.ActivityType == "AssessmentCompleted")
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            if (!orderedActivities.Any())
            {
                return streaks;
            }

            var currentStreak = CalculateCurrentStreak(orderedActivities);
            streaks.Add(currentStreak);
            return streaks;
        }

        private StudyStreakDto CalculateCurrentStreak(IEnumerable<UserActivity> activities)
        {
            var orderedActivities = activities.ToList();
            if (!orderedActivities.Any())
            {
                return new StudyStreakDto
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    Days = 0,
                    IsActive = false
                };
            }

            int streakDays = 1;
            var currentDate = orderedActivities.First().Timestamp.Date;
            var previousDate = currentDate;

            foreach (var activity in orderedActivities.Skip(1))
            {
                var activityDate = activity.Timestamp.Date;
                if ((previousDate - activityDate).Days == 1)
                {
                    streakDays++;
                    previousDate = activityDate;
                }
                else if ((previousDate - activityDate).Days > 1)
                {
                    break;
                }
            }

            return new StudyStreakDto
            {
                StartDate = previousDate,
                EndDate = currentDate,
                Days = streakDays,
                IsActive = currentDate == DateTime.Today
            };
        }
    }

    public class GetRecentActivitiesQueryHandler : IRequestHandler<GetRecentActivitiesQuery, Result<List<ActivityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRecentActivitiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<ActivityDto>>> Handle(GetRecentActivitiesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var activities = await _unitOfWork.UserActivityRepository.GetRecentActivityAsync(request.UserId, request.Limit);
                var result = _mapper.Map<List<ActivityDto>>(activities);

                return Result<List<ActivityDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<ActivityDto>>.Failure($"Failed to retrieve recent activities: {ex.Message}");
            }
        }
    }

    public class GetLearningStatisticsQueryHandler : IRequestHandler<GetLearningStatisticsQuery, Result<LearningStatisticsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetLearningStatisticsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<LearningStatisticsDto>> Handle(GetLearningStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var startDate = request.Period.ToLower() switch
                {
                    "weekly" => DateTime.Now.AddDays(-7),
                    "yearly" => DateTime.Now.AddYears(-1),
                    _ => DateTime.Now.AddMonths(-1)
                };

                var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId && up.CompletedAt >= startDate);
                var sessions = await _unitOfWork.UserSessions.GetActiveSessionsAsync(request.UserId);

                var totalTime = userProgress.Sum(up => up.TimeSpentMinutes);
                var totalSessions = sessions.Count;

                var stats = new LearningStatisticsDto
                {
                    TotalTimeSpent = TimeSpan.FromMinutes(totalTime),
                    TotalSessions = totalSessions,
                    AverageSessionTime = totalSessions > 0 ? totalTime / totalSessions : 0,
                    CompletedContents = userProgress.Count(up => up.IsCompleted),
                    TotalContents = userProgress.Count(),
                    CompletionRate = userProgress.Any() ? (double)userProgress.Count(up => up.IsCompleted) / userProgress.Count() * 100 : 0,
                    DailyStats = userProgress
                        .Where(up => up.CompletedAt.HasValue)
                        .GroupBy(up => up.CompletedAt!.Value.Date)
                        .Select(g => new DailyStatisticDto
                        {
                            Date = g.Key,
                            TimeSpent = TimeSpan.FromMinutes(g.Sum(up => up.TimeSpentMinutes)),
                            CompletedContents = g.Count(up => up.IsCompleted),
                            Sessions = g.Count()
                        })
                        .ToList()
                };

                return Result<LearningStatisticsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                return Result<LearningStatisticsDto>.Failure($"Failed to retrieve learning statistics: {ex.Message}");
            }
        }
    }

    public class GetStudyStreakQueryHandler : IRequestHandler<GetStudyStreakQuery, Result<StudyStreakDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetStudyStreakQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<StudyStreakDto>> Handle(GetStudyStreakQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var activities = await _unitOfWork.UserActivityRepository.GetByUserIdAsync(request.UserId);
                var streak = CalculateCurrentStreak(activities);

                return Result<StudyStreakDto>.Success(streak);
            }
            catch (Exception ex)
            {
                return Result<StudyStreakDto>.Failure($"Failed to retrieve study streak: {ex.Message}");
            }
        }

        private StudyStreakDto CalculateCurrentStreak(IEnumerable<UserActivity> activities)
        {
            var orderedActivities = activities
                .Where(a => a.ActivityType == "ContentCompleted" || a.ActivityType == "AssessmentCompleted")
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            if (!orderedActivities.Any())
            {
                return new StudyStreakDto
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    Days = 0,
                    IsActive = false
                };
            }

            int streakDays = 1;
            var currentDate = orderedActivities.First().Timestamp.Date;
            var previousDate = currentDate;

            foreach (var activity in orderedActivities.Skip(1))
            {
                var activityDate = activity.Timestamp.Date;
                if ((previousDate - activityDate).Days == 1)
                {
                    streakDays++;
                    previousDate = activityDate;
                }
                else if ((previousDate - activityDate).Days > 1)
                {
                    break;
                }
            }

            return new StudyStreakDto
            {
                StartDate = previousDate,
                EndDate = currentDate,
                Days = streakDays,
                IsActive = currentDate == DateTime.Today
            };
        }
    }
}