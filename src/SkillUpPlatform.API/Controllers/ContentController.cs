using Microsoft.AspNetCore.Mvc;
using SkillUpPlatform.Application.Features.Contentt.Commands;
using SkillUpPlatform.Application.Features.Contentt.Queries;
using SkillUpPlatform.Application.Features.Progress.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Content management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ContentController : BaseController
{
    public ContentController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Get all content
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetContent([FromQuery] int? learningPathId = null, [FromQuery] string? type = null)
    {
        var query = new GetContentQuery { LearningPathId = learningPathId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get content by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetContentById(int id)
    {
        var query = new GetContentByIdQuery { ContentId = id, UserId = GetCurrentUserId() };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create new content (Content Creator only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> CreateContent([FromBody] CreateContentCommand command)
    {
        command.CreatedBy = GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update content (Content Creator only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> UpdateContent(int id, [FromBody] UpdateContentCommand command)
    {
        command.Id = id;
        command.UpdatedBy = GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete content (Content Creator only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ContentCreator,Admin")]
    public async Task<IActionResult> DeleteContent(int id)
    {
        var command = new DeleteContentCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

        /// <summary>
        /// Mark content as completed (Student only)
        /// </summary>
        [HttpPost("{id}/complete")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CompleteContent(int id)
        {
            var command = new CompleteContentCommand { ContentId = id, UserId = GetCurrentUserId() };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Get content progress
        /// </summary>
        [HttpGet("{id}/progress")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetContentProgress(int id)
        {
            var query = new GetContentProgressQuery { ContentId = id, UserId = GetCurrentUserId() };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get next content in learning path
        /// </summary>
        [HttpGet("{id}/next")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetNextContent(int id, [FromQuery] int learningPathId)
        {
            var query = new SkillUpPlatform.Application.Features.Contentt.Queries.GetNextContentQuery { CurrentContentId = id, LearningPathId = learningPathId, UserId = GetCurrentUserId() };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get previous content in learning path
        /// </summary>
        [HttpGet("{id}/previous")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetPreviousContent(int id, [FromQuery] int learningPathId)
        {
            var query = new SkillUpPlatform.Application.Features.Contentt.Queries.GetPreviousContentQuery { CurrentContentId = id, LearningPathId = learningPathId, UserId = GetCurrentUserId() };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

}
