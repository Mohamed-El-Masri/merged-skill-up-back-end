using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Application.Features.Assessments.DTOs;

public class AssessmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AssessmentType AssessmentType { get; set; }
    public int TimeLimit { get; set; }
    public int PassingScore { get; set; }
    public bool IsActive { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}

public class QuestionDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public List<string> Options { get; set; } = new();
    public string? Explanation { get; set; }
    public int Points { get; set; }
}

public class AssessmentResultDto
{
    public int Id { get; set; }
    public string AssessmentTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int TimeSpentMinutes { get; set; }
    public bool IsPassed { get; set; }
    public DateTime CompletedAt { get; set; }
    public string? Feedback { get; set; }
}
