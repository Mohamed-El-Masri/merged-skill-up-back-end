using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class Assessment : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public AssessmentType AssessmentType { get; set; }
    public int TimeLimit { get; set; } // in minutes
    public int PassingScore { get; set; } // percentage
    public bool IsActive { get; set; } = true;
    public int? LearningPathId { get; set; }

    // Navigation Properties
    public virtual LearningPath? LearningPath { get; set; }
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<AssessmentResult> AssessmentResults { get; set; } = new List<AssessmentResult>();
}
