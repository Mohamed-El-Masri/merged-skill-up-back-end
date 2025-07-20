using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SkillUpPlatform.Application.Features.Progress.Commands;
using SkillUpPlatform.Application.Features.Progress.Queries;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Learning progress tracking endpoints for students
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
[Tags("👨‍🎓 Student - Progress Tracking")]
public class ProgressController : BaseController
{
    public ProgressController(IMediator mediator) : base(mediator)
    {
    }
    /// <summary>
    /// Mark content as started
    /// </summary>
    /// <param name="contentId">Content ID</param>
    /// <returns>Progress tracking result</returns>
    /// <response code="200">Content marked as started</response>
    /// <response code="404">Content not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("content/{contentId}/start")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> StartContent(int contentId)
    {
        var command = new StartContentCommand { ContentId = contentId };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Update content progress
    /// </summary>
    /// <param name="contentId">Content ID</param>
    /// <param name="command">Progress data</param>
    /// <returns>Progress update result</returns>
    /// <response code="200">Progress updated successfully</response>
    /// <response code="404">Content not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("content/{contentId}/progress")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateContentProgress(int contentId, [FromBody] UpdateContentProgressCommand command)
    {
        command.ContentId = contentId;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Mark content as completed
    /// </summary>
    /// <param name="contentId">Content ID</param>
    /// <returns>Completion result</returns>
    /// <response code="200">Content marked as completed</response>
    /// <response code="404">Content not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("content/{contentId}/complete")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CompleteContent(int contentId)
    {
        var command = new CompleteContentCommand { ContentId = contentId };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get user's overall progress
    /// </summary>
    /// <returns>Overall learning progress</returns>
    /// <response code="200">Progress retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("overall")]
    [ProducesResponseType(typeof(Result<OverallProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOverallProgress()
    {
        var query = new GetOverallProgressQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get learning path progress
    /// </summary>
    /// <param name="learningPathId">Learning path ID</param>
    /// <returns>Learning path progress details</returns>
    /// <response code="200">Progress retrieved successfully</response>
    /// <response code="404">Learning path not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("learning-path/{learningPathId}")]
    [ProducesResponseType(typeof(Result<LearningPathProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLearningPathProgress(int learningPathId)
    {
        var query = new GetLearningPathProgressQuery { LearningPathId = learningPathId };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get content progress details
    /// </summary>
    /// <param name="contentId">Content ID</param>
    /// <returns>Content progress details</returns>
    /// <response code="200">Progress retrieved successfully</response>
    /// <response code="404">Content not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("content/{contentId}")]
    [ProducesResponseType(typeof(Result<ContentProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetContentProgress(int contentId)
    {
        var query = new GetContentProgressQuery { ContentId = contentId };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get progress statistics
    /// </summary>
    /// <param name="period">Period for statistics (week, month, year)</param>
    /// <returns>Progress statistics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(Result<ProgressStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProgressStatistics([FromQuery] string period = "month")
    {
        var query = new GetProgressStatisticsQuery { Period = period };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get learning streaks
    /// </summary>
    /// <returns>Learning streak information</returns>
    /// <response code="200">Streaks retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("streaks")]
    [ProducesResponseType(typeof(Result<LearningStreakDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLearningStreaks()
    {
        var query = new GetLearningStreaksQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Set learning goals
    /// </summary>
    /// <param name="command">Learning goals data</param>
    /// <returns>Goal setting result</returns>
    /// <response code="200">Goals set successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("goals")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SetLearningGoals([FromBody] SetLearningGoalsCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get learning goals
    /// </summary>
    /// <returns>User's learning goals</returns>
    /// <response code="200">Goals retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("goals")]
    [ProducesResponseType(typeof(Result<LearningGoalsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLearningGoals()
    {
        var query = new GetLearningGoalsQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Track time spent on content
    /// </summary>
    /// <param name="contentId">Content ID</param>
    /// <param name="command">Time tracking data</param>
    /// <returns>Time tracking result</returns>
    /// <response code="200">Time tracked successfully</response>
    /// <response code="404">Content not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("content/{contentId}/time")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> TrackTimeSpent(int contentId, [FromBody] TrackTimeSpentCommand command)
    {
        command.ContentId = contentId;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get time spent analytics
    /// </summary>
    /// <param name="period">Period for analytics (week, month, year)</param>
    /// <returns>Time spent analytics</returns>
    /// <response code="200">Analytics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("time-analytics")]
    [ProducesResponseType(typeof(Result<SkillUpPlatform.Application.Features.Progress.Queries.TimeSpentAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTimeSpentAnalytics([FromQuery] string period = "month")
    {
        var query = new SkillUpPlatform.Application.Features.Progress.Queries.GetTimeSpentAnalyticsQuery { Period = period };
        return HandleResult(await _mediator.Send(query));
    }
}
