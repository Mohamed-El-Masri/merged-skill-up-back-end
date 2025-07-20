using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Application.Features.LearningPaths.DTOs;

public class LearningPathDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int EstimatedDurationHours { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Prerequisites { get; set; } = new();
    public List<string> LearningObjectives { get; set; } = new();
    public bool IsActive { get; set; }
}

public class LearningPathDetailDto : LearningPathDto
{
    public List<ContentSummaryDto> Contents { get; set; } = new();
    public List<AssessmentSummaryDto> Assessments { get; set; } = new();
}

public class ContentSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ContentType ContentType { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsRequired { get; set; }
}

public class AssessmentSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public AssessmentType AssessmentType { get; set; }
    public int TimeLimit { get; set; }
}

public class UserLearningPathDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public LearningPathStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
}
