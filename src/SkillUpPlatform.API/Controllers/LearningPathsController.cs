using Microsoft.AspNetCore.Mvc;
using SkillUpPlatform.Application.Features.LearningPaths.Commands;
using SkillUpPlatform.Application.Features.LearningPaths.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Learning paths management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LearningPathsController : BaseController
{
    public LearningPathsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Get all learning paths
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetLearningPaths([FromQuery] string? category = null, [FromQuery] string? difficultyLevel = null)
    {
        var query = new GetLearningPathsQuery { Category = category, DifficultyLevel = difficultyLevel };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get learning path by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLearningPathById(int id)
    {
        var query = new GetLearningPathByIdQuery { LearningPathId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create new learning path (Content Creator only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> CreateLearningPath([FromBody] CreateLearningPathCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update learning path (Content Creator only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> UpdateLearningPath(int id, [FromBody] UpdateLearningPathCommand command)
    {
        command.LearningPathId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete learning path (Content Creator only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> DeleteLearningPath(int id)
    {
        var command = new DeleteLearningPathCommand { LearningPathId = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Enroll in learning path (Student only)
    /// </summary>
    [HttpPost("{id}/enroll")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> EnrollInLearningPath(int id)
    {
        var command = new EnrollInLearningPathCommand { LearningPathId = id, UserId = GetCurrentUserId() };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get user's enrolled learning paths
    /// </summary>
    [HttpGet("my-learning-paths")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyLearningPaths()
    {
        var query = new GetUserLearningPathsQuery { UserId = GetCurrentUserId() };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
