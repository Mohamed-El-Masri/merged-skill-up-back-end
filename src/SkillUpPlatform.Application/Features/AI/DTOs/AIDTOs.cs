namespace SkillUpPlatform.Application.Features.AI.DTOs;

public class UserProgressAnalysisDto
{
    public int UserId { get; set; }
    public int CompletedCourses { get; set; }
    public int TotalEnrolledCourses { get; set; }
    public double OverallProgress { get; set; }
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public Dictionary<string, double> SkillLevels { get; set; } = new();
}

public class RecommendationDto
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string? Url { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
