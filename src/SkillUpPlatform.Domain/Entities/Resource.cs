using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class Resource : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ResourceType ResourceType { get; set; }
    public string? FileUrl { get; set; }
    public string? TemplateContent { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Tags { get; set; } // JSON string
    public bool IsActive { get; set; } = true;
    public int DownloadCount { get; set; } = 0;
}
