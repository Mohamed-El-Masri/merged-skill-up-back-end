using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class AssessmentResult : BaseEntity
{
    public int UserId { get; set; }
    public int AssessmentId { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int TimeSpentMinutes { get; set; }
    public bool IsPassed { get; set; }
    public DateTime CompletedAt { get; set; }
    public string? Feedback { get; set; }
    public string? AIFeedback { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Assessment Assessment { get; set; } = null!;
    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
