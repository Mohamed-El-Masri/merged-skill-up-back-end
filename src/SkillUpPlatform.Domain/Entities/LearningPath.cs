using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class LearningPath : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int EstimatedDurationHours { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Prerequisites { get; set; } = string.Empty; // JSON string
    public string LearningObjectives { get; set; } = string.Empty; // JSON string
    public bool IsActive { get; set; } = true;
    public int EstimatedDuration { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }
    public decimal Price { get; set; }
    public int CreatorId { get; set; }
    public string Tags { get; set; }
    // Navigation Properties
    public virtual User Creator { get; set; } = null!;
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
    public virtual ICollection<UserLearningPath> UserLearningPaths { get; set; } = new List<UserLearningPath>();
    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
}
