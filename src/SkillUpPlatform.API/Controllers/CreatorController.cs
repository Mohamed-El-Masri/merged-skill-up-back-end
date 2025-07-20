using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SkillUpPlatform.Application.Features.ContentCreator.Commands;
using SkillUpPlatform.Application.Features.ContentCreator.Queries;
using SkillUpPlatform.Application.Features.LearningPaths.Commands;
using SkillUpPlatform.Application.Common.Models;
using Common = SkillUpPlatform.Application.Common;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Content creator management endpoints
/// </summary>
[ApiController]
[Route("api/creator")]
[Authorize(Roles = "ContentCreator")]
[Tags("👨‍🏫 Content Creator - Management")]
public class CreatorController : BaseController
{
    public CreatorController(IMediator mediator) : base(mediator)
    {
    }
    /// <summary>
    /// Get creator dashboard overview
    /// </summary>
    /// <returns>Creator dashboard data</returns>
    /// <response code="200">Dashboard data retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(Result<CreatorDashboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboard()
    {
        var query = new GetCreatorDashboardQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get creator's learning paths
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="status">Filter by status</param>
    /// <returns>Creator's learning paths</returns>
    /// <response code="200">Learning paths retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("learning-paths")]
    [ProducesResponseType(typeof(Result<PagedResult<Common.Models.CreatorLearningPathDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyLearningPaths(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null)
    {
        var query = new GetCreatorLearningPathsQuery
        {
            Page = page,
            PageSize = pageSize,
            Status = status
        };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Create new learning path
    /// </summary>
    /// <param name="command">Learning path data</param>
    /// <returns>Created learning path</returns>
    /// <response code="200">Learning path created successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("learning-paths")]
    [ProducesResponseType(typeof(Result<Common.Models.CreatorLearningPathDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateLearningPath([FromBody] SkillUpPlatform.Application.Features.LearningPaths.Commands.CreateLearningPathCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Update learning path
    /// </summary>
    /// <param name="id">Learning path ID</param>
    /// <param name="command">Learning path update data</param>
    /// <returns>Update result</returns>
    /// <response code="200">Learning path updated successfully</response>
    /// <response code="404">Learning path not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpPut("learning-paths/{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateLearningPath(int id, [FromBody] SkillUpPlatform.Application.Features.LearningPaths.Commands.UpdateLearningPathCommand command)
    {
        command.LearningPathId = id;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Delete learning path
    /// </summary>
    /// <param name="id">Learning path ID</param>
    /// <returns>Delete result</returns>
    /// <response code="200">Learning path deleted successfully</response>
    /// <response code="404">Learning path not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpDelete("learning-paths/{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteLearningPath(int id)
    {
        var command = new DeleteLearningPathCommand { LearningPathId = id };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Publish learning path
    /// </summary>
    /// <param name="id">Learning path ID</param>
    /// <returns>Publish result</returns>
    /// <response code="200">Learning path published successfully</response>
    /// <response code="404">Learning path not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpPost("learning-paths/{id}/publish")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PublishLearningPath(int id)
    {
        var command = new PublishLearningPathCommand { LearningPathId = id };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Unpublish learning path
    /// </summary>
    /// <param name="id">Learning path ID</param>
    /// <returns>Unpublish result</returns>
    /// <response code="200">Learning path unpublished successfully</response>
    /// <response code="404">Learning path not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpPost("learning-paths/{id}/unpublish")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UnpublishLearningPath(int id)
    {
        var command = new UnpublishLearningPathCommand { LearningPathId = id };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get learning path analytics
    /// </summary>
    /// <param name="id">Learning path ID</param>
    /// <returns>Learning path analytics</returns>
    /// <response code="200">Analytics retrieved successfully</response>
    /// <response code="404">Learning path not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpGet("learning-paths/{id}/analytics")]
    [ProducesResponseType(typeof(Result<LearningPathAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetLearningPathAnalytics(int id)
    {
        var query = new GetLearningPathAnalyticsQuery { LearningPathId = id };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get enrolled students
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="learningPathId">Filter by learning path ID</param>
    /// <returns>Enrolled students</returns>
    /// <response code="200">Students retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("students")]
    [ProducesResponseType(typeof(Result<PagedResult<CreatorStudentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEnrolledStudents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? learningPathId = null)
    {
        var query = new GetEnrolledStudentsQuery
        {
            Page = page,
            PageSize = pageSize,
            LearningPathId = learningPathId
        };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get student progress
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="learningPathId">Learning path ID</param>
    /// <returns>Student progress</returns>
    /// <response code="200">Progress retrieved successfully</response>
    /// <response code="404">Student or learning path not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpGet("students/{studentId}/progress")]
    [ProducesResponseType(typeof(Result<StudentProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetStudentProgress(int studentId, [FromQuery] int learningPathId)
    {
        var query = new GetStudentProgressQuery
        {
            StudentId = studentId,
            LearningPathId = learningPathId
        };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Provide feedback to student
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="command">Feedback data</param>
    /// <returns>Feedback result</returns>
    /// <response code="200">Feedback provided successfully</response>
    /// <response code="404">Student not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpPost("students/{studentId}/feedback")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ProvideFeedback(Guid studentId, [FromBody] ProvideFeedbackCommand command)
    {
        command.StudentId = studentId;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get engagement analytics
    /// </summary>
    /// <param name="period">Period for analytics</param>
    /// <returns>Engagement analytics</returns>
    /// <response code="200">Analytics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("engagement")]
    [ProducesResponseType(typeof(Result<EngagementAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEngagementAnalytics([FromQuery] string period = "month")
    {
        var query = new GetEngagementAnalyticsQuery { Period = period };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get creator analytics
    /// </summary>
    /// <param name="period">Period for analytics</param>
    /// <returns>Creator analytics</returns>
    /// <response code="200">Analytics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("analytics")]
    [ProducesResponseType(typeof(Result<CreatorAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCreatorAnalytics([FromQuery] string period = "month")
    {
        var query = new GetCreatorAnalyticsQuery { Period = period };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get revenue analytics
    /// </summary>
    /// <param name="period">Period for analytics</param>
    /// <returns>Revenue analytics</returns>
    /// <response code="200">Analytics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("revenue")]
    [ProducesResponseType(typeof(Result<Common.Models.RevenueAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRevenueAnalytics([FromQuery] string period = "month")
    {
        var query = new GetRevenueAnalyticsQuery { Period = period };
        return HandleResult(await _mediator.Send(query));
    }
}
