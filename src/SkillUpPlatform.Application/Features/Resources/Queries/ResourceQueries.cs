using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Resources.DTOs;
using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Application.Features.Resources.Queries;

public class GetResourcesQuery : IRequest<Result<List<ResourceDto>>>
{
    public ResourceType? Type { get; set; }
    public int? LearningPathId { get; set; }
    public string? Category { get; set; }
    public string? SearchTerm { get; set; }
}

public class GetResourceByIdQuery : IRequest<Result<ResourceDetailDto>>
{
    public int ResourceId { get; set; }
}

public class GetCVTemplatesQuery : IRequest<Result<List<ResourceDto>>>
{
}

public class GetCoverLetterTemplatesQuery : IRequest<Result<List<ResourceDto>>>
{
}

public class GetInterviewQuestionsQuery : IRequest<Result<List<ResourceDto>>>
{
    public string? Category { get; set; }
}

public class GetInterviewQuestionsByCategoryQuery : IRequest<Result<List<ResourceDto>>>
{
    public string Category { get; set; } = string.Empty;
}

public class DeleteResourceCommand : IRequest<Result>
{
    public int Id { get; set; }
}

public class DownloadResourceQuery : IRequest<Result<FileDownloadDto>>
{
    public int ResourceId { get; set; }
    public int UserId { get; set; }
}