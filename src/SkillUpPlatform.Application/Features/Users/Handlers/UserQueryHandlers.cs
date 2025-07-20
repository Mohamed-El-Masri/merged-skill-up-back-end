using AutoMapper;
using MediatR;
using SkillUpPlatform.Application.Common.Constants;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Users.DTOs;
using SkillUpPlatform.Application.Features.Users.Queries;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using System.Text.Json;

namespace SkillUpPlatform.Application.Features.Users.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<Common.Models.UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<Common.Models.UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result<Common.Models.UserDto>.Failure(ErrorMessages.UserNotFound);
        }

        var userDto = _mapper.Map<Common.Models.UserDto>(user);
        return Result<Common.Models.UserDto>.Success(userDto);
    }
}

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserWithProfileAsync(request.UserId);
        
        if (user == null)
        {
            return Result<UserProfileDto>.Failure(ErrorMessages.UserNotFound);
        }

        var profileDto = new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Bio = user.UserProfile?.Bio,
            LinkedInUrl = user.UserProfile?.LinkedInUrl,
            GitHubUrl = user.UserProfile?.GitHubUrl,
            PortfolioUrl = user.UserProfile?.PortfolioUrl,
            ProfilePictureUrl = user.UserProfile?.ProfilePictureUrl,
            Skills = !string.IsNullOrEmpty(user.UserProfile?.Skills) 
                ? JsonSerializer.Deserialize<List<string>>(user.UserProfile.Skills) ?? new List<string>()
                : new List<string>(),
            Interests = !string.IsNullOrEmpty(user.UserProfile?.Interests) 
                ? JsonSerializer.Deserialize<List<string>>(user.UserProfile.Interests) ?? new List<string>()
                : new List<string>(),
            Certifications = !string.IsNullOrEmpty(user.UserProfile?.Certifications) 
                ? JsonSerializer.Deserialize<List<string>>(user.UserProfile.Certifications) ?? new List<string>()
                : new List<string>()
        };

        return Result<UserProfileDto>.Success(profileDto);
    }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<Common.Models.UserDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<Common.Models.UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        
        // Apply search filter if provided
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            users = users.Where(u => 
                u.FirstName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        // Apply pagination
        users = users.Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize);

        var userDtos = _mapper.Map<List<Common.Models.UserDto>>(users.ToList());
        return Result<List<Common.Models.UserDto>>.Success(userDtos);
    }
}
public class GetUserStatisticsQueryHandler : IRequestHandler<GetUserStatisticsQuery, Result<UserQueries_UserStatisticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserStatisticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserQueries_UserStatisticsDto>> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return Result<UserQueries_UserStatisticsDto>.Failure("User not found");
        }

        // Get user learning paths
        var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);
        var learningPathIds = userLearningPaths.Select(ulp => ulp.LearningPathId).ToList();

        // Get progress for learning paths
        var completedLearningPaths = 0;
        foreach (var learningPathId in learningPathIds)
        {
            var progressPercentage = await _unitOfWork.UserProgress.GetLearningPathProgressPercentageAsync(request.UserId, learningPathId);
            if (progressPercentage >= 100) completedLearningPaths++;
        }

        // Get user progress for content
        var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);
        var completedContentCount = userProgress.Count(up => up.IsCompleted);
        var totalTimeSpent = TimeSpan.FromMinutes(userProgress.Sum(up => up.TimeSpentMinutes));

        // Get assessment results
        var assessmentResults = await _unitOfWork.AssessmentResults.GetByUserIdAsync(request.UserId);
        var completedAssessments = assessmentResults.Count(ar => ar.IsPassed);
        var averageScore = assessmentResults.Any() ? assessmentResults.Average(ar => ar.Score) : 0;

        // Get login count and streaks (assumes UserActivity tracking)
        var userActivities = await _unitOfWork.UserActivityRepository.GetByUserIdAsync(request.UserId);
        var loginCount = user.LastLoginAt.HasValue ? 1 : 0; // Placeholder: assumes at least one login if LastLoginAt is set
        var (currentStreak, longestStreak) = CalculateStreaks(userActivities);

        // Get category progress
        var categoryProgress = await CalculateCategoryProgress(request.UserId);

        // Construct DTO
        var statistics = new UserQueries_UserStatisticsDto
        {
            TotalTimeSpent = totalTimeSpent,
            TotalLearningPaths = userLearningPaths.Count(),
            CompletedLearningPaths = completedLearningPaths,
            TotalAssessments = assessmentResults.Count(),
            AverageScore = averageScore,
            LoginCount = loginCount,
            LastLogin = user.LastLoginAt,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            CategoryProgress = categoryProgress
        };

        return Result<UserQueries_UserStatisticsDto>.Success(statistics);
    }

    private async Task<List<UserQueries_CategoryProgressDto>> CalculateCategoryProgress(int userId)
    {
        var categoryProgress = new List<UserQueries_CategoryProgressDto>();

        // Get all learning paths and group by category
        var learningPaths = await _unitOfWork.LearningPaths.GetAllAsync();
        var categories = learningPaths.Select(lp => lp.Category).Distinct();

        foreach (var category in categories)
        {
            // Get learning paths for the category
            var categoryLearningPaths = await _unitOfWork.LearningPaths.GetLearningPathsByCategoryAsync(category);
            var learningPathIds = categoryLearningPaths.Select(lp => lp.Id).ToList();

            // Get content for these learning paths
            var contents = await _unitOfWork.Contents.FindAsync(c => learningPathIds.Contains(c.LearningPathId));
            var contentIds = contents.Select(c => c.Id).ToList();

            // Get user progress for these contents
            var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == userId && contentIds.Contains(up.ContentId));

            var totalContent = contentIds.Count;
            var completedContent = userProgress.Count(up => up.IsCompleted);
            var completionRate = totalContent > 0 ? (completedContent / (double)totalContent) * 100 : 0;
            var timeSpent = TimeSpan.FromMinutes(userProgress.Sum(up => up.TimeSpentMinutes));

            categoryProgress.Add(new UserQueries_CategoryProgressDto
            {
                Category = category,
                TotalContent = totalContent,
                CompletedContent = completedContent,
                CompletionRate = completionRate,
                TimeSpent = timeSpent
            });
        }

        return categoryProgress;
    }

    private (int CurrentStreak, int LongestStreak) CalculateStreaks(IEnumerable<UserActivity> activities)
    {
        // Assumes UserActivity has Timestamp and Action properties
        var loginActivities = activities
            .OrderByDescending(ua => ua.Timestamp)
            .Select(ua => ua.Timestamp.Date)
            .Distinct()
            .ToList();

        if (!loginActivities.Any()) return (0, 0);

        int currentStreak = 1;
        int longestStreak = 1;
        int tempStreak = 1;
        var today = DateTime.Today;

        for (int i = 1; i < loginActivities.Count; i++)
        {
            if (loginActivities[i - 1] == loginActivities[i].AddDays(1))
            {
                tempStreak++;
                if (loginActivities[i - 1] == today && tempStreak > currentStreak)
                {
                    currentStreak = tempStreak;
                }
                if (tempStreak > longestStreak)
                {
                    longestStreak = tempStreak;
                }
            }
            else
            {
                tempStreak = 1;
            }
        }

        return (currentStreak, longestStreak);
    }
}
