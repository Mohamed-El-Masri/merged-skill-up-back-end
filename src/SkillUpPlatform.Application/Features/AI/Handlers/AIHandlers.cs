using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.AI.Commands;
using SkillUpPlatform.Application.Features.AI.DTOs;
using SkillUpPlatform.Application.Features.AI.Queries;
using SkillUpPlatform.Application.Interfaces;

namespace SkillUpPlatform.Application.Features.AI.Handlers;

public class AIHandlers :
    IRequestHandler<ChatWithAssistantCommand, Result<string>>,
    IRequestHandler<AnalyzeSkillsCommand, Result<SkillAnalysisResult>>,
    IRequestHandler<RecommendCareerCommand, Result<List<CareerRecommendation>>>,
    IRequestHandler<GenerateCVCommand, Result<string>>,
    IRequestHandler<GetUserProgressAnalysisQuery, Result<UserProgressAnalysisDto>>,
    IRequestHandler<GetRecommendationsQuery, Result<List<RecommendationDto>>>
{
    private readonly IAIService _aiService;

    public AIHandlers(IAIService aiService)
    {
        _aiService = aiService;
    }

    public async Task<Result<string>> Handle(ChatWithAssistantCommand request, CancellationToken cancellationToken)
    {
        var response = await _aiService.ChatWithAssistantAsync(request.Message, request.Context);
        return Result<string>.Success(response);
    }

    public async Task<Result<SkillAnalysisResult>> Handle(AnalyzeSkillsCommand request, CancellationToken cancellationToken)
    {
        var result = await _aiService.AnalyzeSkillsAsync(request.Skills, request.AssessmentResults);
        return Result<SkillAnalysisResult>.Success(result);
    }

    public async Task<Result<List<CareerRecommendation>>> Handle(RecommendCareerCommand request, CancellationToken cancellationToken)
    {
        var result = await _aiService.RecommendCareerAsync(request.Skills, request.Interests, request.EducationLevel, request.Experience);
        return Result<List<CareerRecommendation>>.Success(result);
    }

    public async Task<Result<string>> Handle(GenerateCVCommand request, CancellationToken cancellationToken)
    {
        var result = await _aiService.GenerateCVAsync(request.UserId, request.TemplateId);
        return Result<string>.Success(result);
    }

    public async Task<Result<UserProgressAnalysisDto>> Handle(GetUserProgressAnalysisQuery request, CancellationToken cancellationToken)
    {
        // Dummy data for now, adjust as needed
        var result = new UserProgressAnalysisDto
        {
            UserId = request.UserId,
            CompletedCourses = 3,
            TotalEnrolledCourses = 5,
            OverallProgress = 60.0,
            Strengths = new() { "Teamwork", "Python" },
            Weaknesses = new() { "Soft Skills" },
            Recommendations = new() { "Take communication course" },
            SkillLevels = new Dictionary<string, double>
            {
                { "Programming", 80.5 },
                { "Data Analysis", 70.2 }
            }
        };
        return Result<UserProgressAnalysisDto>.Success(result);
    }

    public async Task<Result<List<RecommendationDto>>> Handle(GetRecommendationsQuery request, CancellationToken cancellationToken)
    {
        // Dummy recommendations
        var result = new List<RecommendationDto>
        {
            new RecommendationDto
            {
                Type = request.RecommendationType,
                Title = "Advance to Fullstack Development",
                Description = "Learn backend skills with .NET",
                Priority = 1,
                Url = "https://example.com/fullstack-course"
            }
        };
        return Result<List<RecommendationDto>>.Success(result);
    }
}
