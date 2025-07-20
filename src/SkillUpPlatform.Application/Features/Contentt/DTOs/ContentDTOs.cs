using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Application.Features.Contentt.DTOs;

public class ContentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ContentType ContentType { get; set; }
    public int DurationMinutes { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsCompleted { get; set; }
}

public class ContentDetailDto : ContentDto
{
    public string? VideoUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public string? TextContent { get; set; }
    public int LearningPathId { get; set; }
    public string LearningPathTitle { get; set; } = string.Empty;
    public UserProgressDto? UserProgress { get; set; }
}

public class UserProgressDto
{
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentMinutes { get; set; }
    public int ProgressPercentage { get; set; }
}
