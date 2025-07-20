using Microsoft.AspNetCore.Mvc;
using SkillUpPlatform.Application.Features.Assessments.Commands;
using SkillUpPlatform.Application.Features.Assessments.Queries;
using SkillUpPlatform.Application.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Assessment management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AssessmentsController : BaseController
{
    public AssessmentsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Get all assessments
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAssessments([FromQuery] string? category = null, [FromQuery] int? learningPathId = null)
    {
        var query = new GetAssessmentsQuery { Category = category, LearningPathId = learningPathId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get assessment by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetAssessmentById(int id)
    {
        var query = new GetAssessmentByIdQuery { AssessmentId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create new assessment (Content Creator only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> CreateAssessment([FromBody] CreateAssessmentCommand command)
    {
        command.CreatedBy = GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update assessment (Content Creator only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> UpdateAssessment(int id, [FromBody] UpdateAssessmentCommand command)
    {
        command.Id = id;
        command.UpdatedBy = GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete assessment (Content Creator only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> DeleteAssessment(int id)
    {
        var command = new DeleteAssessmentCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Submit assessment (Student only)
    /// </summary>
    [HttpPost("{id}/submit")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> SubmitAssessment(int id, [FromBody] SubmitAssessmentCommand command)
    {
        command.AssessmentId = id;
        command.UserId = GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get assessment results
    /// </summary>
    [HttpGet("{id}/results")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetAssessmentResults(int id)
    {
        var query = new GetAssessmentResultsQuery { AssessmentId = id, UserId = GetCurrentUserId() };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Start assessment
    /// </summary>
    [HttpPost("{id}/start")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> StartAssessment(int id)
    {
        var command = new StartAssessmentCommand { AssessmentId = id, UserId = GetCurrentUserId() };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get assessment questions
    /// </summary>
    [HttpGet("{id}/questions")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetAssessmentQuestions(int id)
    {
        var query = new GetAssessmentQuestionsQuery { AssessmentId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get user's assessment history
    /// </summary>
    [HttpGet("my-assessments")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyAssessments()
    {
        var query = new GetUserAssessmentsQuery { UserId = GetCurrentUserId() };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
