using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class UserAnswer : BaseEntity
{
    public int UserId { get; set; }
    public int QuestionId { get; set; }
    public int AssessmentResultId { get; set; }
    public string UserAnswerText { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Question Question { get; set; } = null!;
    public virtual AssessmentResult AssessmentResult { get; set; } = null!;
}
