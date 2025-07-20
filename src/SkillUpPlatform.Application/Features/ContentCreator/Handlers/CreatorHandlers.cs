using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.ContentCreator.Commands;
using SkillUpPlatform.Application.Features.ContentCreator.Queries;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using CreatorLearningPathDto = SkillUpPlatform.Application.Common.Models.CreatorLearningPathDto;

namespace SkillUpPlatform.Application.Features.ContentCreator.Handlers;

public class CreatorCreateLearningPathCommandHandler : IRequestHandler<CreatorCreateLearningPathCommand, Result<CreatorLearningPathDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CreatorCreateLearningPathCommandHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<CreatorLearningPathDto>> Handle(
            CreatorCreateLearningPathCommand request,
            CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<CreatorLearningPathDto>.Failure("User not authenticated");
        }

        var creator = await _unitOfWork.Users.GetByIdAsync(userId);
        if (creator == null || creator.Role != UserRole.ContentCreator)
        {
            return Result<CreatorLearningPathDto>.Failure("Content creator not found");
        }

        var learningPath = new LearningPath
        {
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            DifficultyLevel = request.DifficultyLevel,
            EstimatedDurationHours = request.EstimatedDuration,
            Price = request.Price,
            IsPublished = request.IsPublished,
            Tags = string.Join(",", request.Tags),
            Prerequisites = string.Join(",", request.Prerequisites),
            LearningObjectives = string.Join(",", request.LearningObjectives),
            CreatorId = creator.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.LearningPaths.AddAsync(learningPath);
        await _unitOfWork.SaveChangesAsync();

        var dto = new CreatorLearningPathDto
        {
            Id = learningPath.Id,
            Title = learningPath.Title,
            Description = learningPath.Description,
            Category = learningPath.Category,
            DifficultyLevel = learningPath.DifficultyLevel,
            EstimatedDuration = learningPath.EstimatedDuration,
            Price = learningPath.Price,
            IsPublished = learningPath.IsPublished,
            Tags = request.Tags,
            Prerequisites = learningPath.Prerequisites.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            LearningObjectives = learningPath.LearningObjectives.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            CreatedAt = learningPath.CreatedAt,
            EnrollmentCount = 0,
            AverageRating = 0,
            ReviewCount = 0
        };

        return Result<CreatorLearningPathDto>.Success(dto);
    }
}

public class UpdateLearningPathHandler
      : MediatR.IRequest<SkillUpPlatform.Application.Common.Models.Result<SkillUpPlatform.Application.Common.Models.CreatorLearningPathDto>>
//: IRequestHandler<UpdateLearningPathCommand, Result<CreatorLearningPathDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateLearningPathHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<CreatorLearningPathDto>> Handle(UpdateLearningPathCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<CreatorLearningPathDto>.Failure("User not authenticated");
        }

        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.Id);
        if (learningPath == null)
        {
            return Result<CreatorLearningPathDto>.Failure("Learning path not found");
        }

        var creator = await _unitOfWork.Users.GetByIdAsync(learningPath.CreatorId);
        if (creator == null || creator.Id != userId)
        {
            return Result<CreatorLearningPathDto>.Failure("You don't have permission to update this learning path");
        }
        {
            return Result<CreatorLearningPathDto>.Failure("You don't have permission to update this learning path");
        }

        learningPath.Title = request.Title;
        learningPath.Description = request.Description;
        learningPath.Category = request.Category;
        learningPath.DifficultyLevel = request.DifficultyLevel;
        learningPath.EstimatedDuration = request.EstimatedDuration;
        learningPath.Price = request.Price;
        learningPath.IsPublished = request.IsPublished;
        learningPath.Tags = string.Join(",", request.Tags);
        learningPath.Prerequisites = string.Join(",", request.Prerequisites);
        learningPath.LearningObjectives = string.Join(",", request.LearningObjectives);
        learningPath.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.LearningPaths.Update(learningPath);
        await _unitOfWork.SaveChangesAsync();

        // Get updated enrollment and review counts
        var enrollments = await _unitOfWork.UserLearningPaths.GetByLearningPathIdAsync(learningPath.Id);
        var reviews = await _unitOfWork.AssessmentResults.GetAssessmentResultsByAssessmentAsync(learningPath.Id); // Assuming assessments are linked to learning paths

        var dto = new CreatorLearningPathDto
        {
            Id = learningPath.Id,
            Title = learningPath.Title,
            Description = learningPath.Description,
            Category = learningPath.Category,
            DifficultyLevel = learningPath.DifficultyLevel,
            EstimatedDuration = learningPath.EstimatedDuration,
            Price = learningPath.Price,
            IsPublished = learningPath.IsPublished,
            Tags = request.Tags,
            Prerequisites = request.Prerequisites,
            LearningObjectives = request.LearningObjectives,
            CreatedAt = learningPath.CreatedAt,
            UpdatedAt = learningPath.UpdatedAt,
            EnrollmentCount = enrollments.Count(),
            AverageRating = reviews.Any() ? reviews.Average(r => r.Score) : 0,
            ReviewCount = reviews.Count()
        };

        return Result<CreatorLearningPathDto>.Success(dto);
    }
}

public class ProvideFeedbackHandler : IRequestHandler<ProvideFeedbackCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public ProvideFeedbackHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result> Handle(ProvideFeedbackCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result.Failure("User not authenticated");
        }

        // Verify the learning path belongs to the creator
        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.LearningPathId);
        if (learningPath == null || learningPath.CreatorId != userId)
        {
            return Result.Failure("Learning path not found or you don't have permission");
        }

        // Verify the student is enrolled in the learning path
        var enrollment = await _unitOfWork.UserLearningPaths.GetByUserAndLearningPathAsync(request.StudentId, request.LearningPathId);
        if (enrollment == null)
        {
            return Result.Failure("Student is not enrolled in this learning path");
        }

        var feedback = new AssessmentResult
        {
            AssessmentId = 0, // Or create a specific feedback assessment type
            UserId = request.StudentId,
            Score = request.Rating,
            Feedback = request.FeedbackText,
            Suggestions = string.Join(",", request.Suggestions),
            CompletedAt = DateTime.UtcNow
        };

        await _unitOfWork.AssessmentResults.AddAsync(feedback);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}

public class GetCreatorDashboardHandler : IRequestHandler<GetCreatorDashboardQuery, Result<CreatorDashboardDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public GetCreatorDashboardHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<CreatorDashboardDto>> Handle(GetCreatorDashboardQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<CreatorDashboardDto>.Failure("User not authenticated");
        }

        var creator = await _unitOfWork.Users.GetByIdAsync(userId);
        if (creator == null || creator.Role != UserRole.ContentCreator)
        {
            return Result<CreatorDashboardDto>.Failure("Content creator not found");
        }

        var learningPaths = await _unitOfWork.LearningPaths.FindAsync(lp => lp.CreatorId == creator.Id);
        var enrollments = await _unitOfWork.UserLearningPaths.FindAsync(e => learningPaths.Select(lp => lp.Id).Contains(e.LearningPathId));
        var assessments = await _unitOfWork.AssessmentResults.FindAsync(ar => ar.Assessment != null && learningPaths.Select(lp => lp.Id).Contains(ar.Assessment.LearningPathId ?? 0));

        var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

        var dashboard = new CreatorDashboardDto
        {
            TotalLearningPaths = learningPaths.Count(),
            TotalStudents = enrollments.Select(e => e.UserId).Distinct().Count(),
            TotalRevenue = (int)(enrollments.Any() ? enrollments.Sum(e => e.AmmountPaid) : 0),
            TotalRatings = assessments.Count(),
            //AverageRating = assessments.Any() ? assessments.Average(a => a.Score) : 0,
            NewEnrollmentsThisMonth = enrollments.Count(e => e.EnrolledAt >= oneMonthAgo),
            PopularLearningPaths = learningPaths
                .OrderByDescending(lp => enrollments.Count(e => e.LearningPathId == lp.Id))
                .Take(5)
                .Select(lp => new PopularLearningPathDto
                {
                    Id = lp.Id,
                    Title = lp.Title,
                    EnrollmentCount = enrollments.Count(e => e.LearningPathId == lp.Id),
                    AverageRating = assessments.Where(a => a.Assessment?.LearningPathId == lp.Id).Any() 
                        ? assessments.Where(a => a.Assessment?.LearningPathId == lp.Id).Average(a => a.Score) 
                        : 0,
                    Revenue = enrollments.Where(e => e.LearningPathId == lp.Id).Any() 
                        ? enrollments.Where(e => e.LearningPathId == lp.Id).Sum(e => e.AmmountPaid) 
                        : 0
                })
                .ToList(),
            RecentEnrollments = enrollments
                .OrderByDescending(e => e.EnrolledAt)
                .Take(5)
                .Select(async e => new RecentEnrollmentDto
                {
                    Id = e.Id,
                    StudentName = (await _unitOfWork.Users.GetByIdAsync(e.UserId))?.FirstName ?? "Unknown",
                    LearningPathTitle = (await _unitOfWork.LearningPaths.GetByIdAsync(e.LearningPathId))?.Title ?? "Unknown",
                    EnrollmentDate = e.EnrolledAt,
                    AmountPaid = e.AmmountPaid
                })
                .Select(t => t.Result)
                .ToList(),
            RecentReviews = assessments
                .OrderByDescending(a => a.CompletedAt)
                .Take(5)
                .Select(async a => new ReviewDto
                {
                    Id = a.Id,
                    StudentName = (await _unitOfWork.Users.GetByIdAsync(a.UserId))?.FirstName?? "Unknown",
                    LearningPathTitle = (await _unitOfWork.LearningPaths.GetByIdAsync(a.Assessment?.LearningPathId ?? 0))?.Title ?? "Unknown",
                    Rating = a.Score,
                    Comment = a.Feedback,
                    CreatedAt = a.CompletedAt
                })
                .Select(t => t.Result)
                .ToList()
        };

        return Result<CreatorDashboardDto>.Success(dashboard);
    }
}

public class GetCreatorLearningPathsHandler : IRequestHandler<GetCreatorLearningPathsQuery, Result<PagedResult<CreatorLearningPathDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public GetCreatorLearningPathsHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<PagedResult<CreatorLearningPathDto>>> Handle(GetCreatorLearningPathsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<PagedResult<CreatorLearningPathDto>>.Failure("User not authenticated");
        }

        var creator = await _unitOfWork.Users.GetByIdAsync(userId);
        if (creator == null || creator.Role != UserRole.ContentCreator)
        {
            return Result<PagedResult<CreatorLearningPathDto>>.Failure("Content creator not found");
        }

        var query = (await _unitOfWork.LearningPaths.FindAsync(lp => lp.CreatorId == creator.Id)).AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
        {
            if (request.Status.Equals("published", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(lp => lp.IsPublished);
            }
            else if (request.Status.Equals("draft", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(lp => !lp.IsPublished);
            }
        }

        var totalCount = query.Count();

        var learningPaths = query
            .OrderByDescending(lp => lp.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Get all enrollments and assessments for the learning paths
        var learningPathIds = learningPaths.Select(lp => lp.Id).ToList();
        var allEnrollments = await _unitOfWork.UserLearningPaths.FindAsync(e => learningPathIds.Contains(e.LearningPathId));
        var allAssessments = await _unitOfWork.AssessmentResults.FindAsync(a => a.Assessment != null && learningPathIds.Contains(a.Assessment.LearningPathId ?? 0));

        var dtos = learningPaths.Select(lp => new CreatorLearningPathDto
        {
            Id = lp.Id,
            Title = lp.Title,
            Description = lp.Description,
            Category = lp.Category,
            DifficultyLevel = lp.DifficultyLevel,
            EstimatedDuration = lp.EstimatedDuration,
            Price = lp.Price,
            IsPublished = lp.IsPublished,
            Tags = lp.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            Prerequisites = lp.Prerequisites.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            LearningObjectives = lp.LearningObjectives.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            CreatedAt = lp.CreatedAt,
            UpdatedAt = lp.UpdatedAt,
            EnrollmentCount = allEnrollments.Count(e => e.LearningPathId == lp.Id),
            AverageRating = allAssessments.Any(a => a.Assessment?.LearningPathId == lp.Id) 
                ? allAssessments.Where(a => a.Assessment?.LearningPathId == lp.Id).Average(a => a.Score) 
                : 0,
            ReviewCount = allAssessments.Count(a => a.Assessment?.LearningPathId == lp.Id)
        }).ToList();

        var pagedResult = new PagedResult<CreatorLearningPathDto>
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<CreatorLearningPathDto>>.Success(pagedResult);
    }
}

public class GetEnrolledStudentsHandler : IRequestHandler<GetEnrolledStudentsQuery, Result<PagedResult<EnrolledStudentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public GetEnrolledStudentsHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<PagedResult<EnrolledStudentDto>>> Handle(GetEnrolledStudentsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<PagedResult<EnrolledStudentDto>>.Failure("User not authenticated");
        }

        var creator = await _unitOfWork.Users.GetByIdAsync(userId);
        if (creator == null || creator.Role != UserRole.ContentCreator)
        {
            return Result<PagedResult<EnrolledStudentDto>>.Failure("Content creator not found");
        }

        // Get all learning paths created by this creator
        var learningPaths = await _unitOfWork.LearningPaths.FindAsync(lp => lp.CreatorId == creator.Id);
        var learningPathIds = learningPaths.Select(lp => lp.Id).ToList();

        var enrollmentsQuery = (await _unitOfWork.UserLearningPaths.FindAsync(e =>
            learningPathIds.Contains(e.LearningPathId))).AsQueryable();

        if (request.LearningPathId.HasValue)
        {
            enrollmentsQuery = enrollmentsQuery.Where(e => e.LearningPathId == request.LearningPathId.Value);
        }

        var totalCount = enrollmentsQuery.Count();

        var enrollments = enrollmentsQuery
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var studentProgress = await _unitOfWork.UserProgress.GetUserProgressByLearningPathAsync(0, 0); // You'll need to implement this properly

        var dtos = enrollments.Select(async e => new EnrolledStudentDto
        {
            Id = e.Id,
            StudentName = (await _unitOfWork.Users.GetByIdAsync(e.UserId))?.FirstName ?? "Unknown",
            StudentEmail = (await _unitOfWork.Users.GetByIdAsync(e.UserId))?.Email ?? "Unknown",
            LearningPathTitle = (await _unitOfWork.LearningPaths.GetByIdAsync(e.LearningPathId))?.Title ?? "Unknown",
            EnrollmentDate = e.EnrolledAt,
            ProgressPercentage = studentProgress.Any(sp => sp.UserId == e.UserId) ?
                (int)studentProgress.Where(sp => sp.UserId == e.UserId).Average(sp => sp.ProgressPercentage) : 0,
            LastAccessed = studentProgress.Any(sp => sp.UserId == e.UserId) ?
                studentProgress.Where(sp => sp.UserId == e.UserId).Max(sp => sp.LastAccessed) : e.EnrolledAt,
            Status = e.Status.ToString()
        }).Select(t => t.Result).ToList();

        var pagedResult = new PagedResult<EnrolledStudentDto>
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<EnrolledStudentDto>>.Success(pagedResult);
    }
}

public class GetLearningPathAnalyticsHandler : IRequestHandler<GetLearningPathAnalyticsQuery, Result<LearningPathAnalyticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public GetLearningPathAnalyticsHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<LearningPathAnalyticsDto>> Handle(GetLearningPathAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<LearningPathAnalyticsDto>.Failure("User not authenticated");
        }

        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.LearningPathId);
        if (learningPath == null)
        {
            return Result<LearningPathAnalyticsDto>.Failure("Learning path not found");
        }

        if (learningPath.CreatorId != userId)
        {
            return Result<LearningPathAnalyticsDto>.Failure("You don't have permission to view analytics for this learning path");
        }

        var enrollments = await _unitOfWork.UserLearningPaths.GetByLearningPathIdAsync(request.LearningPathId);
        var assessments = await _unitOfWork.AssessmentResults.GetAssessmentResultsByAssessmentAsync(request.LearningPathId);
        var userProgress = await _unitOfWork.UserProgress.GetUserProgressByLearningPathAsync(userId,request.LearningPathId); // Removed 0 parameter

        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var dailyEnrollments = enrollments
            .Where(e => e.EnrolledAt >= thirtyDaysAgo)
            .GroupBy(e => e.EnrolledAt.Date)
            .Select(g => new DailyEnrollmentDto
            {
                Date = g.Key,
                Enrollments = g.Count(),
                Completions = g.Count(e => userProgress.Any(up => up.UserId == e.UserId && up.ProgressPercentage == 100))
            })
            .OrderBy(d => d.Date)
            .ToList();

        var ratingDistribution = Enumerable.Range(1, 5)
           .Select(star =>
           {
               var totalAssessments = assessments.Count();
               var starCount = assessments.Count(a => Math.Round(a.Score / 20.0) == star);
               return new RatingDistributionDto
               {
                   Stars = star,
                   Count = starCount,
                   Percentage = totalAssessments > 0
                       ? (starCount * 100.0) / totalAssessments
                       : 0
               };
           });

        var completedEnrollments = enrollments
            .Where(e => userProgress.Any(up => up.UserId == e.UserId && up.ProgressPercentage == 100));

        var averageCompletionTime = completedEnrollments.Any()
            ? completedEnrollments.Average(e => (DateTime.UtcNow - e.EnrolledAt).TotalDays)
            : 0;

        var analytics = new LearningPathAnalyticsDto
        {
            Id = learningPath.Id,
            Title = learningPath.Title,
            TotalEnrollments = enrollments.Count(),
            CompletedEnrollments = completedEnrollments.Count(),
            CompletionRate = enrollments.Any()
                ? (double)completedEnrollments.Count() / enrollments.Count() * 100
                : 0,
            AverageRating = assessments.Any()
                ? assessments.Average(a => a.Score)
                : 0,
            TotalRatings = assessments.Count(),
            TotalRevenue = enrollments.Sum(e => e.AmmountPaid), // Fixed typo: AmmountPaid → AmountPaid
            AverageTimeToComplete = averageCompletionTime,
            DailyEnrollments = dailyEnrollments,
            RatingDistribution = ratingDistribution.ToList() 
        };

        return Result<LearningPathAnalyticsDto>.Success(analytics);
    }
}

public class GetStudentProgressHandler : IRequestHandler<GetStudentProgressQuery, Result<StudentProgressDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public GetStudentProgressHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<StudentProgressDto>> Handle(GetStudentProgressQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<StudentProgressDto>.Failure("User not authenticated");
        }

        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.LearningPathId);
        if (learningPath == null || learningPath.CreatorId != userId)
        {
            return Result<StudentProgressDto>.Failure("Learning path not found or you don't have permission");
        }

        var enrollment = await _unitOfWork.UserLearningPaths.GetByUserAndLearningPathAsync(request.StudentId, request.LearningPathId);
        if (enrollment == null)
        {
            return Result<StudentProgressDto>.Failure("Enrollment not found");
        }

        var student = await _unitOfWork.Users.GetByIdAsync(request.StudentId);
        var progressRecords = await _unitOfWork.UserProgress.GetUserProgressByLearningPathAsync(request.StudentId, request.LearningPathId);
        var assessmentResults = await _unitOfWork.AssessmentResults.GetByUserIdAsync(request.StudentId);

        var progress = new StudentProgressDto
        {
            StudentId = request.StudentId,
            StudentName = student?.FirstName ?? "Unknown",
            LearningPathId = request.LearningPathId,
            LearningPathTitle = learningPath.Title,
            EnrollmentDate = enrollment.EnrolledAt,
            ProgressPercentage = progressRecords.Any() ?
                (int)progressRecords.Average(p => p.ProgressPercentage) : 0,
            LastAccessed = progressRecords.Any() ?
                progressRecords.Max(p => p.LastAccessed) : enrollment.EnrolledAt,
            TotalTimeSpent = TimeSpan.FromMinutes(progressRecords.Sum(p => p.TimeSpentMinutes)),
            ContentProgress = progressRecords.Select(p => new CreatorContentProgressDto
            {
                ContentId = p.ContentId,
                ContentTitle = p.Content?.Title ?? "Unknown",
                ProgressPercentage = p.ProgressPercentage,
                IsCompleted = p.IsCompleted,
                TimeSpentMinutes = p.TimeSpentMinutes,
                LastAccessed = p.LastAccessed
            }).ToList(),
            AssessmentResults = assessmentResults
                .Where(ar => ar.Assessment?.LearningPathId == request.LearningPathId)
                .Select(ar => new CreatorAssessmentResultDto
                {
                    Id = ar.Id,
                    AssessmentTitle = ar.Assessment?.Title ?? "Unknown",
                    Score = ar.Score,
                    MaxScore = ar.Assessment?.MaxScore ?? 100,
                    Percentage = (double)ar.Score / (ar.Assessment?.MaxScore ?? 100) * 100,
                    CompletedAt = ar.CompletedAt,
                    AttemptNumber = ar.AttemptNumber
                }).ToList()
        };

        return Result<StudentProgressDto>.Success(progress);
    }
}

public class GetCreatorAnalyticsHandler : IRequestHandler<GetCreatorAnalyticsQuery, Result<CreatorAnalyticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public GetCreatorAnalyticsHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<CreatorAnalyticsDto>> Handle(GetCreatorAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<CreatorAnalyticsDto>.Failure("User not authenticated");
        }

        var learningPaths = await _unitOfWork.LearningPaths.FindAsync(lp => lp.CreatorId == userId);
        var enrollments = await _unitOfWork.UserLearningPaths.FindAsync(e => learningPaths.Select(lp => lp.Id).Contains(e.LearningPathId));
        var assessments = await _unitOfWork.AssessmentResults.FindAsync(ar => ar.Assessment != null && learningPaths.Select(lp => lp.Id).Contains(ar.Assessment.LearningPathId ?? 0));

        var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);
        var monthlyStats = enrollments
            .Where(e => e.EnrolledAt >= twelveMonthsAgo)
            .GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month })
            .Select(g => new MonthlyStatsDto
            {
                Month = g.Key.Month,
                Year = g.Key.Year,
                NewStudents = g.Select(e => e.UserId).Distinct().Count(),
                ActiveStudents = g.Count(),
                CompletionRate = GetCompletionRate(g.Select(e => e.UserId).Distinct().ToList(), learningPaths)
            })
            .OrderBy(s => s.Year)
            .ThenBy(s => s.Month)
            .ToList();

        var analytics = new CreatorAnalyticsDto
        {
            TotalStudents = enrollments.Select(e => e.UserId).Distinct().Count(),
            ActiveStudents = enrollments.Count(e => e.LastAccessed >= DateTime.UtcNow.AddDays(-30)),
            AverageCompletion = await GetAverageCompletion(enrollments, learningPaths),
            AverageRating = assessments.Any() ? assessments.Average(a => a.Score) : 0,
            TotalRevenue = (int)enrollments.Sum(e => e.AmmountPaid),
            MonthlyStats = monthlyStats
        };

        return Result<CreatorAnalyticsDto>.Success(analytics);
    }

    private async Task<double> GetAverageCompletion(IEnumerable<UserLearningPath> enrollments, IEnumerable<LearningPath> learningPaths)
    {
        double totalCompletion = 0;
        int count = 0;

        foreach (var enrollment in enrollments)
        {
            var progress = await _unitOfWork.UserProgress.GetUserProgressByLearningPathAsync(enrollment.UserId, enrollment.LearningPathId);
            if (progress.Any())
            {
                totalCompletion += progress.Average(p => p.ProgressPercentage);
                count++;
            }
        }

        return count > 0 ? totalCompletion / count : 0;
    }

    private double GetCompletionRate(List<int> studentIds, IEnumerable<LearningPath> learningPaths)
    {
        // Implement logic to calculate completion rate
        // This might need additional repository methods
        return 0; // Placeholder
    }
}

public class GetEngagementAnalyticsHandler : IRequestHandler<GetEngagementAnalyticsQuery, Result<EngagementAnalyticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public GetEngagementAnalyticsHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<EngagementAnalyticsDto>> Handle(GetEngagementAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<EngagementAnalyticsDto>.Failure("User not authenticated");
        }

        var learningPaths = await _unitOfWork.LearningPaths.FindAsync(lp => lp.CreatorId == userId);
        var enrollments = await _unitOfWork.UserLearningPaths.FindAsync(e => learningPaths.Select(lp => lp.Id).Contains(e.LearningPathId));
        var contents = await _unitOfWork.Contents.FindAsync(c => learningPaths.Select(lp => lp.Id).Contains(c.LearningPathId));

        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var activeStudents = enrollments
            .Where(e => e.LastAccessed >= thirtyDaysAgo)
            .Select(e => e.UserId)
            .Distinct()
            .Count();

        var totalSessions = enrollments.Sum(e => e.SessionCount);
        var averageSessionTime = enrollments.Any() ?
            enrollments.Average(e => e.TotalMinutesSpent / Math.Max(1, e.SessionCount)) : 0;

        var contentEngagement = new List<CreatorContentEngagementDto>();
        foreach (var content in contents)
        {
            var progresses = await _unitOfWork.UserProgress.FindAsync(p => p.ContentId == content.Id);
            contentEngagement.Add(new CreatorContentEngagementDto
            {
                ContentId = content.Id,
                Title = content.Title,
                Views = progresses.Count(),
                TotalTimeSpent = TimeSpan.FromMinutes(progresses.Sum(p => p.TimeSpentMinutes)),
                CompletionRate = progresses.Any() ?
                    (double)progresses.Count(p => p.IsCompleted) / progresses.Count() * 100 : 0
            });
        }

        var analytics = new EngagementAnalyticsDto
        {
            TotalSessions = totalSessions,
            AverageSessionTime = TimeSpan.FromMinutes(averageSessionTime),
            RetentionRate = CalculateRetentionRate(enrollments),
            DropOffRate = CalculateDropOffRate(enrollments),
            ContentEngagement = contentEngagement
        };

        return Result<EngagementAnalyticsDto>.Success(analytics);
    }

    private double CalculateRetentionRate(IEnumerable<UserLearningPath> enrollments)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var sixtyDaysAgo = DateTime.UtcNow.AddDays(-60);

        var activeStudents = enrollments
            .Where(e => e.LastAccessed >= thirtyDaysAgo)
            .Select(e => e.UserId)
            .Distinct()
            .Count();

        var previousActiveStudents = enrollments
            .Where(e => e.LastAccessed >= sixtyDaysAgo && e.LastAccessed < thirtyDaysAgo)
            .Select(e => e.UserId)
            .Distinct()
            .Count();

        return previousActiveStudents > 0 ? (double)activeStudents / previousActiveStudents * 100 : 0;
    }

    private int CalculateDropOffRate(IEnumerable<UserLearningPath> enrollments)
    {
        var totalEnrollments = enrollments.Count();
        var completedEnrollments = enrollments.Count(e =>
            _unitOfWork.UserProgress.GetUserProgressByLearningPathAsync(e.UserId, e.LearningPathId)
                .Result.Any(p => p.ProgressPercentage == 100));

        return totalEnrollments > 0 ? (int)((double)(totalEnrollments - completedEnrollments) / totalEnrollments * 100) : 0;
    }
}

public partial class GetRevenueAnalyticsHandler : IRequestHandler<GetRevenueAnalyticsQuery, Result<CreatorRevenueAnalyticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public GetRevenueAnalyticsHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<CreatorRevenueAnalyticsDto>> Handle(GetRevenueAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<CreatorRevenueAnalyticsDto>.Failure("User not authenticated");
        }

        var learningPaths = await _unitOfWork.LearningPaths.FindAsync(lp => lp.CreatorId == userId);
        var enrollments = await _unitOfWork.UserLearningPaths.FindAsync(e => learningPaths.Select(lp => lp.Id).Contains(e.LearningPathId));

        var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);
        var monthlyBreakdown = enrollments
            .Where(e => e.EnrolledAt >= twelveMonthsAgo)
            .GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month })
            .Select(g => new CreatorMonthlyRevenueDto
            {
                Month = g.Key.Month,
                Year = g.Key.Year,
                Revenue = g.Sum(e => e.AmmountPaid),
                NewEnrollments = g.Count()
            })
            .OrderBy(r => r.Year)
            .ThenBy(r => r.Month)
            .ToList();

        var totalStudents = enrollments.Select(e => e.UserId).Distinct().Count();

        var analytics = new CreatorRevenueAnalyticsDto
        {
            TotalRevenue = enrollments.Sum(e => e.AmmountPaid),
            MonthlyRevenue = monthlyBreakdown.LastOrDefault()?.Revenue ?? 0,
            AverageRevenuePerStudent = totalStudents > 0 ?
                enrollments.Sum(e => e.AmmountPaid) / totalStudents : 0,
            MonthlyBreakdown = monthlyBreakdown
        };

        return Result<CreatorRevenueAnalyticsDto>.Success(analytics);
    }
}

public class GetCreatorStudentsQueryHandler : IRequestHandler<GetCreatorStudentsQuery, Result<PagedResult<CreatorStudentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public GetCreatorStudentsQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<PagedResult<CreatorStudentDto>>> Handle(GetCreatorStudentsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
        {
            return Result<PagedResult<CreatorStudentDto>>.Failure("User not authenticated");
        }

        var creator = await _unitOfWork.Users.GetByIdAsync(userId);
        if (creator == null || creator.Role != UserRole.ContentCreator)
        {
            return Result<PagedResult<CreatorStudentDto>>.Failure("Content creator not found");
        }

        // Get all learning paths created by this creator
        var learningPaths = await _unitOfWork.LearningPaths.FindAsync(lp => lp.CreatorId == creator.Id);
        var learningPathIds = learningPaths.Select(lp => lp.Id).ToList();

        if (!learningPathIds.Any())
        {
            // No learning paths found, return empty result
            var emptyResult = new PagedResult<CreatorStudentDto>
            {
                Data = new List<CreatorStudentDto>(),
                TotalCount = 0,
                Page = request.Page,
                PageSize = request.PageSize
            };
            return Result<PagedResult<CreatorStudentDto>>.Success(emptyResult);
        }

        // Get enrollments for the creator's learning paths
        var enrollmentsQuery = (await _unitOfWork.UserLearningPaths.FindAsync(e => 
            learningPathIds.Contains(e.LearningPathId))).AsQueryable();

        // Apply learning path filter if specified
        if (request.LearningPathId.HasValue)
        {
            enrollmentsQuery = enrollmentsQuery.Where(e => e.LearningPathId == request.LearningPathId.Value);
        }

        // Apply search filter if specified
        if (!string.IsNullOrEmpty(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            var matchingUserIds = (await _unitOfWork.Users.FindAsync(u => 
                u.FirstName.ToLower().Contains(searchTerm) || 
                u.LastName.ToLower().Contains(searchTerm) || 
                u.Email.ToLower().Contains(searchTerm)))
                .Select(u => u.Id)
                .ToList();
            
            enrollmentsQuery = enrollmentsQuery.Where(e => matchingUserIds.Contains(e.UserId));
        }

        // Apply progress status filter if specified
        if (!string.IsNullOrEmpty(request.ProgressStatus))
        {
            // This would need to be implemented based on progress data
            // For now, we'll skip this filter
        }

        var totalCount = enrollmentsQuery.Count();

        var enrollments = enrollmentsQuery
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Get user details and progress for the enrollments
        var userIds = enrollments.Select(e => e.UserId).Distinct().ToList();
        var users = await _unitOfWork.Users.FindAsync(u => userIds.Contains(u.Id));
        var userProgress = await _unitOfWork.UserProgress.FindAsync(p => userIds.Contains(p.UserId));

        var dtos = enrollments.Select(e =>
        {
            var user = users.FirstOrDefault(u => u.Id == e.UserId);
            var progress = userProgress.Where(p => p.UserId == e.UserId).ToList();
            var averageProgress = progress.Any() ? (int)progress.Average(p => p.ProgressPercentage) : 0;
            var lastAccessed = progress.Any() ? progress.Max(p => p.LastAccessed) : e.EnrolledAt;

            return new CreatorStudentDto
            {
                Id = e.Id,
                FirstName = user?.FirstName ?? "Unknown",
                LastName = user?.LastName ?? "Unknown",
                Email = user?.Email ?? "Unknown",
                EnrolledAt = e.EnrolledAt,
                LastAccessed = lastAccessed,
                ProgressPercentage = averageProgress,
                IsActive = e.Status == LearningPathStatus.InProgress || e.Status == LearningPathStatus.Completed
            };
        }).ToList();

        var pagedResult = new PagedResult<CreatorStudentDto>
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<CreatorStudentDto>>.Success(pagedResult);
    }
}

public class GetCreatorLearningPathByIdQueryHandler : IRequestHandler<GetCreatorLearningPathByIdQuery, Result<CreatorLearningPathDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public GetCreatorLearningPathByIdQueryHandler(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result<CreatorLearningPathDto>> Handle(GetCreatorLearningPathByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        if (userId <= 0)
            return Result<CreatorLearningPathDto>.Failure("User not authenticated");

        var creator = await _unitOfWork.Users.GetByIdAsync(userId);
        if (creator == null || creator.Role != UserRole.ContentCreator)
            return Result<CreatorLearningPathDto>.Failure("Content creator not found");

        var lp = await _unitOfWork.LearningPaths.GetByIdAsync(request.Id);
        if (lp == null || lp.CreatorId != creator.Id)
            return Result<CreatorLearningPathDto>.Failure("Learning path not found or you don't have permission");

        var dto = new CreatorLearningPathDto
        {
            Id = lp.Id,
            Title = lp.Title,
            Description = lp.Description,
            Category = lp.Category,
            DifficultyLevel = lp.DifficultyLevel,
            EstimatedDuration = lp.EstimatedDuration,
            Price = lp.Price,
            IsPublished = lp.IsPublished,
            Tags = lp.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new(),
            Prerequisites = lp.Prerequisites?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new(),
            LearningObjectives = lp.LearningObjectives?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new(),
            CreatedAt = lp.CreatedAt,
            UpdatedAt = lp.UpdatedAt,
            EnrollmentCount = 0, // يمكن تحسينها لاحقاً
            AverageRating = 0,   // يمكن تحسينها لاحقاً
            ReviewCount = 0      // يمكن تحسينها لاحقاً
        };
        return Result<CreatorLearningPathDto>.Success(dto);
    }
}

public class CreatorLoginCommandHandler : IRequestHandler<CreatorLoginCommand, CreatorAuthResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public CreatorLoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<CreatorAuthResult> Handle(CreatorLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || user.Role != UserRole.ContentCreator)
        {
            return new CreatorAuthResult { IsSuccess = false, Error = "Invalid credentials" };
        }

        // يفترض تتحقق الباسورد هنا بالـ PasswordHasher
        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result != PasswordVerificationResult.Success)
        {
            return new CreatorAuthResult { IsSuccess = false, Error = "Invalid password" };
        }

        var token = _tokenService.GenerateJwtToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new CreatorAuthResult
        {
            IsSuccess = true,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new CreatorUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString()
            }
        };
    }
}


