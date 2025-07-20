using AutoMapper;
using MediatR;
using SkillUpPlatform.Application.Common.Constants;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.LearningPaths.Commands;
using SkillUpPlatform.Application.Features.LearningPaths.DTOs;
using SkillUpPlatform.Application.Features.LearningPaths.Queries;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;

namespace SkillUpPlatform.Application.Features.LearningPaths.Handlers;

public class CreateLearningPathCommandHandler : IRequestHandler<CreateLearningPathCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateLearningPathCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateLearningPathCommand request, CancellationToken cancellationToken)
    {
        var learningPath = new LearningPath
        {
            Title = request.Title,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            EstimatedDurationHours = request.EstimatedDurationHours,
            DifficultyLevel = request.DifficultyLevel,
            Category = request.Category,
            Prerequisites = System.Text.Json.JsonSerializer.Serialize(request.Prerequisites),
            LearningObjectives = System.Text.Json.JsonSerializer.Serialize(request.LearningObjectives),
            IsActive = true,
            DisplayOrder = 0
        };

        await _unitOfWork.LearningPaths.AddAsync(learningPath);
        await _unitOfWork.SaveChangesAsync();

        return Result<int>.Success(learningPath.Id);
    }
}

public class GetLearningPathsQueryHandler : IRequestHandler<GetLearningPathsQuery, Result<List<LearningPathDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLearningPathsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<LearningPathDto>>> Handle(GetLearningPathsQuery request, CancellationToken cancellationToken)
    {
        var learningPaths = await _unitOfWork.LearningPaths.GetActiveLearningPathsAsync();

        // Apply filters
        if (!string.IsNullOrEmpty(request.Category))
        {
            learningPaths = learningPaths.Where(lp => lp.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(request.DifficultyLevel) && Enum.TryParse<DifficultyLevel>(request.DifficultyLevel, out var difficulty))
        {
            learningPaths = learningPaths.Where(lp => lp.DifficultyLevel == difficulty);
        }

        var learningPathDtos = _mapper.Map<List<LearningPathDto>>(learningPaths.ToList());
        return Result<List<LearningPathDto>>.Success(learningPathDtos);
    }
}

public class GetLearningPathByIdQueryHandler : IRequestHandler<GetLearningPathByIdQuery, Result<LearningPathDetailDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLearningPathByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<LearningPathDetailDto>> Handle(GetLearningPathByIdQuery request, CancellationToken cancellationToken)
    {
        var learningPath = await _unitOfWork.LearningPaths.GetLearningPathWithContentsAsync(request.LearningPathId);

        if (learningPath == null)
        {
            return Result<LearningPathDetailDto>.Failure(ErrorMessages.LearningPathNotFound);
        }

        var learningPathDto = _mapper.Map<LearningPathDetailDto>(learningPath);
        return Result<LearningPathDetailDto>.Success(learningPathDto);
    }
}

public class EnrollInLearningPathCommandHandler : IRequestHandler<EnrollInLearningPathCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollInLearningPathCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(EnrollInLearningPathCommand request, CancellationToken cancellationToken)
    {
        // Check if user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return Result<bool>.Failure(ErrorMessages.UserNotFound);
        }

        // Check if learning path exists
        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.LearningPathId);
        if (learningPath == null)
        {
            return Result<bool>.Failure(ErrorMessages.LearningPathNotFound);
        }

        // Check if already enrolled
        var existingEnrollment = await _unitOfWork.UserLearningPaths.SingleOrDefaultAsync(
            ulp => ulp.UserId == request.UserId && ulp.LearningPathId == request.LearningPathId);

        if (existingEnrollment != null)
        {
            return Result<bool>.Failure("Already enrolled in this learning path");
        }

        // Create enrollment
        var userLearningPath = new UserLearningPath
        {
            UserId = request.UserId,
            LearningPathId = request.LearningPathId,
            EnrolledAt = DateTime.UtcNow,
            Status = LearningPathStatus.NotStarted,
            ProgressPercentage = 0
        };

        await _unitOfWork.UserLearningPaths.AddAsync(userLearningPath);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}


public class RecommendLearningPathCommandHandler
    : IRequestHandler<RecommendLearningPathCommand, Result<List<LearningPathDto>>>
{
    private readonly IAIService _aiService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RecommendLearningPathCommandHandler(
        IAIService aiService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _aiService = aiService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<LearningPathDto>>> Handle(
        RecommendLearningPathCommand request,
        CancellationToken cancellationToken)
    {
        // Call AI service to get recommendation titles or IDs
        var recommendedTitles = await _aiService.GenerateRecommendationsAsync(
            request.UserId, "learning");

        var allPaths = await _unitOfWork.LearningPaths.GetAllAsync();

        var matchedPaths = allPaths
            .Where(lp => recommendedTitles.Any(r => lp.Title.Contains(r, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var dtoList = _mapper.Map<List<LearningPathDto>>(matchedPaths);

        return Result<List<LearningPathDto>>.Success(dtoList);
    }
}

public class GetUserLearningPathsQueryHandler : IRequestHandler<GetUserLearningPathsQuery, Result<List<Common.Models.UserLearningPathDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserLearningPathsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<Common.Models.UserLearningPathDto>>> Handle(GetUserLearningPathsQuery request, CancellationToken cancellationToken)
    {
        var userLearningPaths = await _unitOfWork.UserLearningPaths.GetByUserIdAsync(request.UserId);

        var result = _mapper.Map<List<Common.Models.UserLearningPathDto>>(userLearningPaths);

        return Result<List<Common.Models.UserLearningPathDto>>.Success(result);
    }
}

public class UpdateLearningPathCommandHandler : IRequestHandler<UpdateLearningPathCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLearningPathCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateLearningPathCommand request, CancellationToken cancellationToken)
    {
        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.LearningPathId);
        if (learningPath == null)
        {
            return Result.Failure("Learning path not found");
        }

        // Update the learning path properties
        if (!string.IsNullOrEmpty(request.Title))
            learningPath.Title = request.Title;
        
        if (!string.IsNullOrEmpty(request.Description))
            learningPath.Description = request.Description;
        
        if (!string.IsNullOrEmpty(request.ImageUrl))
            learningPath.ImageUrl = request.ImageUrl;
        
        if (!string.IsNullOrEmpty(request.Status))
            learningPath.IsPublished = request.Status.Equals("published", StringComparison.OrdinalIgnoreCase);
        else
            learningPath.IsPublished = request.IsPublished;

        learningPath.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.LearningPaths.Update(learningPath);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}

public class PublishLearningPathCommandHandler : IRequestHandler<PublishLearningPathCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PublishLearningPathCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(PublishLearningPathCommand request, CancellationToken cancellationToken)
    {
        // Check if learning path exists
        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.LearningPathId);
        if (learningPath == null)
        {
            return Result<bool>.Failure("Learning path not found");
        }

        // Check if the user is the creator of this learning path
        if (learningPath.CreatorId != request.CreatorId)
        {
            return Result<bool>.Failure("You don't have permission to publish this learning path");
        }

        // Publish the learning path
        learningPath.IsPublished = true;
        learningPath.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.LearningPaths.Update(learningPath);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}

public class UnpublishLearningPathCommandHandler : IRequestHandler<UnpublishLearningPathCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnpublishLearningPathCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UnpublishLearningPathCommand request, CancellationToken cancellationToken)
    {
        // Check if learning path exists
        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.LearningPathId);
        if (learningPath == null)
        {
            return Result<bool>.Failure("Learning path not found");
        }

        // Check if the user is the creator of this learning path
        if (learningPath.CreatorId != request.CreatorId)
        {
            return Result<bool>.Failure("You don't have permission to unpublish this learning path");
        }

        // Unpublish the learning path
        learningPath.IsPublished = false;
        learningPath.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.LearningPaths.Update(learningPath);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}

public class DeleteLearningPathCommandHandler : IRequestHandler<DeleteLearningPathCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLearningPathCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteLearningPathCommand request, CancellationToken cancellationToken)
    {
        // Check if learning path exists
        var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(request.LearningPathId);
        if (learningPath == null)
        {
            return Result<bool>.Failure("Learning path not found");
        }

        // Check if the user is the creator of this learning path
        if (learningPath.CreatorId != request.CreatorId)
        {
            return Result<bool>.Failure("You don't have permission to delete this learning path");
        }

        // Delete the learning path
        _unitOfWork.LearningPaths.Remove(learningPath);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}