using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.AI.Commands;

public class ChatWithAssistantCommand : IRequest<Result<string>>
{
    public int UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Context { get; set; }
}

public class AnalyzeSkillsCommand : IRequest<Result<SkillAnalysisResult>>
{
    public int UserId { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<int> AssessmentResults { get; set; } = new();
}

public class RecommendCareerCommand : IRequest<Result<List<CareerRecommendation>>>
{
    public int UserId { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<string> Interests { get; set; } = new();
    public string? EducationLevel { get; set; }
    public string? Experience { get; set; }
}

public class GenerateCVCommand : IRequest<Result<string>>
{
    public int UserId { get; set; }
    public string TemplateId { get; set; } = string.Empty;
}

public class SkillAnalysisResult
{
    public List<string> IdentifiedSkills { get; set; } = new();
    public List<string> SkillGaps { get; set; } = new();
    public List<string> RecommendedLearningPaths { get; set; } = new();
    public int OverallSkillLevel { get; set; }
}

public class CareerRecommendation
{
    public string CareerTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MatchPercentage { get; set; }
    public List<string> RequiredSkills { get; set; } = new();
    public List<string> RecommendedCourses { get; set; } = new();
}
