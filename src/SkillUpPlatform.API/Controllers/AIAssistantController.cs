using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlatform.API.Controllers;
using SkillUpPlatform.Application.Features.AI.Commands;
using SkillUpPlatform.Application.Features.AI.Queries;
using System.Reflection.Metadata;

[ApiController]
[Route("api/v1/[controller]")]
public class AIAssistantController : BaseController
{
    public AIAssistantController(IMediator mediator) : base(mediator) { }

    [HttpPost("chat")]
    public async Task<IActionResult> ChatWithAssistant([FromBody] ChatWithAssistantCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("analyze-skills")]
    public async Task<IActionResult> AnalyzeSkills([FromBody] AnalyzeSkillsCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("recommend-career")]
    public async Task<IActionResult> RecommendCareer([FromBody] RecommendCareerCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("generate-cv")]
    public async Task<IActionResult> GenerateCV([FromBody] GenerateCVCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("user/{userId}/progress")]
    public async Task<IActionResult> GetUserProgress([FromRoute] int userId)
    {
        var result = await _mediator.Send(new GetUserProgressAnalysisQuery { UserId = userId });
        return HandleResult(result);
    }
}
