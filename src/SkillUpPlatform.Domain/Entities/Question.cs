using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class Question : BaseEntity
{
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string Options { get; set; } = string.Empty; // JSON string for multiple choice options
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int Points { get; set; } = 1;
    public int DisplayOrder { get; set; }
    public int AssessmentId { get; set; }

    // Navigation Properties
    public virtual Assessment Assessment { get; set; } = null!;
    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
