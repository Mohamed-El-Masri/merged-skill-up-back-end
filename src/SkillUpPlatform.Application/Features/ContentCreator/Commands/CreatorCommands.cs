using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.ContentCreator.Commands;

public class CreateLearningPathCommand : IRequest<Result<CreatorLearningPathDto>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public int EstimatedDuration { get; set; }
    public decimal Price { get; set; }
    public bool IsPublished { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public List<string> LearningObjectives { get; set; } = new();
}

public class UpdateLearningPathCommand : IRequest<Result<CreatorLearningPathDto>>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public int EstimatedDuration { get; set; }
    public decimal Price { get; set; }
    public bool IsPublished { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public List<string> LearningObjectives { get; set; } = new();
}

public class ProvideFeedbackCommand : IRequest<Result>
{
    public Guid StudentId { get; set; }
    public Guid LearningPathId { get; set; }
    public string FeedbackText { get; set; } = string.Empty;
    public int Rating { get; set; }
    public List<string> Suggestions { get; set; } = new();
}

public class CreatorLearningPathDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public int EstimatedDuration { get; set; }
    public decimal Price { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public List<string> LearningObjectives { get; set; } = new();
    public int EnrollmentCount { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}
