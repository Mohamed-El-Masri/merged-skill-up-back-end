using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SkillUpPlatform.Application.Features.Admin.Commands;
using SkillUpPlatform.Application.Features.Admin.Queries;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Admin system management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[Tags("👨‍💼 Admin - System Management")]
public class AdminController : BaseController
{
    public AdminController(IMediator mediator) : base(mediator)
    {
    }
    /// <summary>
    /// Admin login
    /// </summary>
    /// <param name="command">Admin login credentials</param>
    /// <returns>Admin authentication result</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<AdminAuthResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AdminLoginCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
            return Ok(result);
        
        return BadRequest(result.Error);
    }

    /// <summary>
    /// Get admin dashboard overview
    /// </summary>
    /// <returns>Admin dashboard data</returns>
    /// <response code="200">Dashboard data retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(Result<AdminDashboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboard()
    {
        var query = new GetAdminDashboardQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get system statistics
    /// </summary>
    /// <returns>System statistics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(Result<SystemStatisticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSystemStatistics()
    {
        var query = new GetSystemStatisticsQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get system health status
    /// </summary>
    /// <returns>System health information</returns>
    /// <response code="200">Health status retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("health")]
    [ProducesResponseType(typeof(Result<SystemHealthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSystemHealth()
    {
        var query = new GetSystemHealthQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get audit logs
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="userId">Filter by user ID</param>
    /// <param name="action">Filter by action</param>
    /// <param name="startDate">Filter by start date</param>
    /// <param name="endDate">Filter by end date</param>
    /// <returns>Audit logs</returns>
    /// <response code="200">Audit logs retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(Result<PagedResult<AuditLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? userId = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = new GetAuditLogsQuery
        {
            Page = page,
            PageSize = pageSize,
            UserId = userId,
            Action = action,
            StartDate = startDate,
            EndDate = endDate
        };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get system configuration
    /// </summary>
    /// <returns>System configuration</returns>
    /// <response code="200">Configuration retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("config")]
    [ProducesResponseType(typeof(Result<SystemConfigDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSystemConfig()
    {
        var query = new GetSystemConfigQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Update system configuration
    /// </summary>
    /// <param name="command">Configuration data</param>
    /// <returns>Update result</returns>
    /// <response code="200">Configuration updated successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpPut("config")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateSystemConfig([FromBody] UpdateSystemConfigCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get performance metrics
    /// </summary>
    /// <param name="period">Period for metrics</param>
    /// <returns>Performance metrics</returns>
    /// <response code="200">Metrics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("performance")]
    [ProducesResponseType(typeof(Result<PerformanceMetricsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPerformanceMetrics([FromQuery] string period = "24h")
    {
        var query = new GetPerformanceMetricsQuery { Period = period };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get error logs
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="severity">Filter by severity</param>
    /// <param name="startDate">Filter by start date</param>
    /// <param name="endDate">Filter by end date</param>
    /// <returns>Error logs</returns>
    /// <response code="200">Error logs retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("errors")]
    [ProducesResponseType(typeof(Result<PagedResult<ErrorLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetErrorLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? severity = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = new GetErrorLogsQuery
        {
            Page = page,
            PageSize = pageSize,
            Severity = severity,
            StartDate = startDate,
            EndDate = endDate
        };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Create admin user
    /// </summary>
    /// <param name="command">Admin user data</param>
    /// <returns>Create result</returns>
    /// <response code="200">Admin user created successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("create-admin")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminUserCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get platform analytics
    /// </summary>
    /// <param name="period">Period for analytics</param>
    /// <returns>Platform analytics</returns>
    /// <response code="200">Analytics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("analytics")]
    [ProducesResponseType(typeof(Result<PlatformAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPlatformAnalytics([FromQuery] string period = "month")
    {
        var query = new GetPlatformAnalyticsQuery { Period = period };
        return HandleResult(await _mediator.Send(query));
    }
}
