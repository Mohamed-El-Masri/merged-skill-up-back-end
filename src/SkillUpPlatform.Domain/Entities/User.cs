using SkillUpPlatform.Domain.Common;

namespace SkillUpPlatform.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Specialization { get; set; }
    public int? StudyYear { get; set; }
    public string? CareerGoals { get; set; }
    public UserRole Role { get; set; } = UserRole.Student;
    public bool IsEmailVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation Properties
    public virtual ICollection<UserLearningPath> UserLearningPaths { get; set; } = new List<UserLearningPath>();
    public virtual ICollection<AssessmentResult> AssessmentResults { get; set; } = new List<AssessmentResult>();
    public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
    public virtual UserProfile? UserProfile { get; set; }
}
