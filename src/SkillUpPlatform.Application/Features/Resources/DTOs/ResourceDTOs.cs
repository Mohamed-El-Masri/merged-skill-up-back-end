using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Application.Features.Resources.DTOs;

public class ResourceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ResourceType ResourceType { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public bool IsActive { get; set; }
    public int DownloadCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ResourceDetailDto : ResourceDto
{
    public string? FileUrl { get; set; }
    public string? TemplateContent { get; set; }
}
