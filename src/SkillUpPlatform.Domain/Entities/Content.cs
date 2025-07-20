using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class Content : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ContentType ContentType { get; set; }
    public string? VideoUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public string? TextContent { get; set; }
    public int DurationMinutes { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; } = true;
    public int LearningPathId { get; set; }

    // Navigation Properties
    public virtual LearningPath LearningPath { get; set; } = null!;
    public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
}
