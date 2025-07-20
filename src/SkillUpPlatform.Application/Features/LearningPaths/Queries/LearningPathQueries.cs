using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.LearningPaths.DTOs;

namespace SkillUpPlatform.Application.Features.LearningPaths.Queries;

public class GetLearningPathsQuery : IRequest<Result<List<LearningPathDto>>>
{
    public string? Category { get; set; }
    public string? DifficultyLevel { get; set; }
}

public class GetLearningPathByIdQuery : IRequest<Result<LearningPathDetailDto>>
{
    public int LearningPathId { get; set; }
}

public class GetUserLearningPathsQuery : IRequest<Result<List<Common.Models.UserLearningPathDto>>>
{
    public int UserId { get; set; }
}
