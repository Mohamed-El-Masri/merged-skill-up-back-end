using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Contentt.DTOs;

namespace SkillUpPlatform.Application.Features.Contentt.Queries;

public class GetContentQuery : IRequest<Result<List<ContentDto>>>
{
    public int? LearningPathId { get; set; }
}

public class GetContentByIdQuery : IRequest<Result<ContentDetailDto>>
{
    public int ContentId { get; set; }
    public int UserId { get; set; }
}


public class GetContentByLearningPathQuery : IRequest<Result<List<ContentDto>>>
{
    public int LearningPathId { get; set; }
}

public class GetNextContentQuery : IRequest<Result<ContentDto>>
{
    public int LearningPathId { get; set; }
    public int CurrentContentId { get; set; }
    public int UserId { get; set; }
}

public class GetPreviousContentQuery : IRequest<Result<ContentDto>>
{
    public int LearningPathId { get; set; }
    public int CurrentContentId { get; set; }
    public int UserId { get; set; }
}