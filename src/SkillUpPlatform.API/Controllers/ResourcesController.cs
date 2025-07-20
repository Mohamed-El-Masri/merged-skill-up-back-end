using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SkillUpPlatform.Application.Features.Resources.Commands;
using SkillUpPlatform.Application.Features.Resources.Queries;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Domain.Entities;
using MediatR;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Resources management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ResourcesController : BaseController
{
    public ResourcesController(IMediator mediator) : base(mediator) { }

    [HttpGet("cv-templates")]
    public async Task<IActionResult> GetCVTemplates()
    {
        var result = await _mediator.Send(new GetCVTemplatesQuery());
        return HandleResult(result);
    }

    [HttpGet("cover-letter-templates")]
    public async Task<IActionResult> GetCoverLetterTemplates()
    {
        var result = await _mediator.Send(new GetCoverLetterTemplatesQuery());
        return HandleResult(result);
    }

    [HttpGet("interview-questions")]
    public async Task<IActionResult> GetInterviewQuestions()
    {
        var result = await _mediator.Send(new GetInterviewQuestionsQuery());
        return HandleResult(result);
    }

    [HttpGet("interview-questions/{category}")]
    public async Task<IActionResult> GetInterviewQuestionsByCategory([FromRoute] string category)
    {
        var result = await _mediator.Send(new GetInterviewQuestionsByCategoryQuery { Category = category });
        return HandleResult(result);
    }

    /// <summary>
    /// Get all resources
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetResources([FromQuery] string? type = null, [FromQuery] int? learningPathId = null)
    {
        ResourceType? resourceType = null;
        if (!string.IsNullOrEmpty(type) && Enum.TryParse<ResourceType>(type, true, out var parsedType))
        {
            resourceType = parsedType;
        }
        
        var query = new GetResourcesQuery { Type = resourceType, LearningPathId = learningPathId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get resource by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetResourceById(int id)
    {
        var query = new GetResourceByIdQuery { ResourceId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create new resource (Content Creator only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> CreateResource([FromBody] CreateResourceCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update resource (Content Creator only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> UpdateResource(int id, [FromBody] UpdateResourceCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Download resource file
    /// </summary>
    [HttpGet("{id}/download")]
    [Authorize]
    public async Task<IActionResult> DownloadResource(int id)
    {
        var query = new DownloadResourceQuery { ResourceId = id };
        var result = await _mediator.Send(query);
        if (result.IsSuccess && result.Data != null)
        {
            return File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
        }
        return NotFound();
    }

    /// <summary>
    /// Delete resource (Content Creator only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> DeleteResource(int id)
    {
        var command = new DeleteResourceCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
