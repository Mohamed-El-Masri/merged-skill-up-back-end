using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Application.Features.Assessments.Commands;

public class CreateAssessmentCommand : IRequest<Result<int>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AssessmentType AssessmentType { get; set; }
    public int TimeLimit { get; set; }
    public int PassingScore { get; set; }
    public int? LearningPathId { get; set; }
    public List<CreateQuestionDto> Questions { get; set; } = new();
    public int CreatedBy { get; set; }
}

public class SubmitAssessmentCommand : IRequest<Result<int>>
{
    public int AssessmentId { get; set; }
    public int UserId { get; set; }
    public List<SubmitAnswerDto> Answers { get; set; } = new();
    public int TimeSpentMinutes { get; set; }
}

public class CreateQuestionDto
{
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public List<string> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int Points { get; set; } = 1;
}

public class SubmitAnswerDto
{
    public int QuestionId { get; set; }
    public string Answer { get; set; } = string.Empty;
}

public class UpdateAssessmentCommand : IRequest<Result<DTOs.AssessmentDto>>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int TimeLimit { get; set; }
    public int MaxAttempts { get; set; }
    public int PassingScore { get; set; }
    public int UpdatedBy { get; set; }
    public bool IsActive { get; set; }
    public List<DTOs.QuestionDto> Questions { get; set; } = new();
}

public class QuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Points { get; set; }
    public List<AnswerOptionDto> Options { get; set; } = new();
    public string? CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
}

public class AnswerOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class DeleteAssessmentCommand : IRequest<Result>
{
    public int Id { get; set; }
}

public class GetAssessmentResultsQuery : IRequest<Result<List<AssessmentResultDto>>>
{
    public int AssessmentId { get; set; }
    public int? UserId { get; set; }
}

public class StartAssessmentCommand : IRequest<Result<int>>
{
    public int AssessmentId { get; set; }
    public int UserId { get; set; }
}

public class GetAssessmentQuestionsQuery : IRequest<Result<List<QuestionDto>>>
{
    public int AssessmentId { get; set; }
}

public class GetUserAssessmentsQuery : IRequest<Result<List<UserAssessmentDto>>>
{
    public int UserId { get; set; }
}

public class AssessmentResultDto
{
    public int Id { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public double Percentage { get; set; }
    public DateTime CompletedAt { get; set; }
    public bool IsPassed { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class UserAssessmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? Score { get; set; }
    public int MaxScore { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPassed { get; set; }
    public int AttemptsLeft { get; set; }
}
