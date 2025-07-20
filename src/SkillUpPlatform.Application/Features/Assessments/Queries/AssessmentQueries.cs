using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Assessments.DTOs;

namespace SkillUpPlatform.Application.Features.Assessments.Queries;

public class GetAssessmentByIdQuery : IRequest<Result<AssessmentDto>>
{
    public int AssessmentId { get; set; }
}

public class GetAssessmentsQuery : IRequest<Result<List<AssessmentDto>>>
{
    public string? Category { get; set; }
    public int? LearningPathId { get; set; }
}

public class GetUserAssessmentResultsQuery : IRequest<Result<List<AssessmentResultDto>>>
{
    public int UserId { get; set; }
}
