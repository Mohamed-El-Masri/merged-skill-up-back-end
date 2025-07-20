using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Contentt.DTOs;
using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Application.Features.Contentt.Commands;

public class CreateContentCommand : IRequest<Result<int>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ContentType ContentType { get; set; }
    public string? VideoUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public string? TextContent { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsRequired { get; set; } = true;
    public int LearningPathId { get; set; }
    public int CreatedBy { get; set; }
}

public class UpdateContentCommand : IRequest<Result>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public string? TextContent { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsRequired { get; set; }
    public int UpdatedBy { get; set; }
}

public class MarkContentAsCompletedCommand : IRequest<Result<bool>>
{
    public int UserId { get; set; }
    public int ContentId { get; set; }
    public int TimeSpentMinutes { get; set; }
}

public class DeleteContentCommand : IRequest<Result>
{
    public int Id { get; set; }
}



public class GetContentProgressQuery : IRequest<Result<ContentProgressDto>>
{
    public int ContentId { get; set; }
    public int UserId { get; set; }
}

public class GetNextContentQuery : IRequest<Result<ContentDto>>
{
    public int LearningPathId { get; set; }
    public int UserId { get; set; }
}

public class GetPreviousContentQuery : IRequest<Result<ContentDto>>
{
    public int LearningPathId { get; set; }
    public int UserId { get; set; }
}