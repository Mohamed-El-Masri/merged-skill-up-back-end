using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SkillUpPlatform.Application.Features.Dashboard.Queries;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Student dashboard endpoints for learning overview and progress
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
[Tags("👨‍🎓 Student - Dashboard")]
public class DashboardController : BaseController
{
    public DashboardController(IMediator mediator) : base(mediator)
    {
    }
    /// <summary>
    /// Get student dashboard overview
    /// </summary>
    /// <returns>Dashboard overview with learning statistics</returns>
    /// <response code="200">Dashboard overview retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(Result<DashboardOverviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOverview()
    {
        var query = new GetDashboardOverviewQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get student learning progress
    /// </summary>
    /// <returns>Detailed learning progress information</returns>
    /// <response code="200">Learning progress retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("progress")]
    [ProducesResponseType(typeof(Result<SkillUpPlatform.Application.Features.Dashboard.Queries.DashboardLearningProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProgress()
    {
        var query = new GetLearningProgressQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get student achievements
    /// </summary>
    /// <returns>List of earned achievements and badges</returns>
    /// <response code="200">Achievements retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("achievements")]
    [ProducesResponseType(typeof(Result<List<SkillUpPlatform.Application.Features.Dashboard.Queries.DashboardAchievementDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAchievements()
    {
        var query = new GetAchievementsQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get personalized recommendations
    /// </summary>
    /// <returns>AI-powered learning recommendations</returns>
    /// <response code="200">Recommendations retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("recommendations")]
    [ProducesResponseType(typeof(Result<List<RecommendationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecommendations()
    {
        var query = new GetPersonalizedRecommendationsQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get learning calendar
    /// </summary>
    /// <param name="month">Month to get calendar for (optional)</param>
    /// <param name="year">Year to get calendar for (optional)</param>
    /// <returns>Learning calendar with scheduled activities</returns>
    /// <response code="200">Calendar retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("calendar")]
    [ProducesResponseType(typeof(Result<LearningCalendarDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCalendar([FromQuery] int? month = null, [FromQuery] int? year = null)
    {
        var query = new GetLearningCalendarQuery { Month = month, Year = year };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get recent activities
    /// </summary>
    /// <param name="limit">Number of activities to return (default: 10)</param>
    /// <returns>Recent learning activities</returns>
    /// <response code="200">Activities retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("activities")]
    [ProducesResponseType(typeof(Result<List<ActivityDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int limit = 10)
    {
        var query = new GetRecentActivitiesQuery { Limit = limit };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get learning statistics
    /// </summary>
    /// <param name="period">Period for statistics (week, month, year)</param>
    /// <returns>Learning statistics for the specified period</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(Result<LearningStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetStatistics([FromQuery] string period = "month")
    {
        var query = new GetLearningStatisticsQuery { Period = period };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get study streak information
    /// </summary>
    /// <returns>Current study streak and history</returns>
    /// <response code="200">Streak information retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("streak")]
    [ProducesResponseType(typeof(Result<StudyStreakDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetStudyStreak()
    {
        var query = new GetStudyStreakQuery();
        return HandleResult(await _mediator.Send(query));
    }
}
