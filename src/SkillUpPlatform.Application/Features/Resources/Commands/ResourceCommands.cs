using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Application.Features.Resources.Commands;

public class CreateResourceCommand : IRequest<Result<int>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ResourceType ResourceType { get; set; }
    public string? FileUrl { get; set; }
    public string? TemplateContent { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}

public class UpdateResourceCommand : IRequest<Result>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? TemplateContent { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public bool IsActive { get; set; }
}
