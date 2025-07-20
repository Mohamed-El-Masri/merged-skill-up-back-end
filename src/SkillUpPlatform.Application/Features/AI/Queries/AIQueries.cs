using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.AI.DTOs;

namespace SkillUpPlatform.Application.Features.AI.Queries;

public class GetUserProgressAnalysisQuery : IRequest<Result<UserProgressAnalysisDto>>
{
    public int UserId { get; set; }
}

public class GetRecommendationsQuery : IRequest<Result<List<RecommendationDto>>>
{
    public int UserId { get; set; }
    public string RecommendationType { get; set; } = string.Empty; // "learning", "career", "content"
}
