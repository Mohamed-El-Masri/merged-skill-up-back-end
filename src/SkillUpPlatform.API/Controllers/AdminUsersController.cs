using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SkillUpPlatform.Application.Features.Admin.Commands;
using SkillUpPlatform.Application.Features.Admin.Queries;
using SkillUpPlatform.Application.Common.Models;
using Common = SkillUpPlatform.Application.Common;

using CommonModels = SkillUpPlatform.Application.Common.Models;
using AdminQueries = SkillUpPlatform.Application.Features.Admin.Queries;


namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Admin user management endpoints
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
[Tags("👨‍💼 Admin - User Management")]
public class AdminUsersController : BaseController
{
    public AdminUsersController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="search">Search term</param>
    /// <param name="role">Filter by role</param>
    /// <param name="status">Filter by status</param>
    /// <returns>List of users</returns>
    /// <response code="200">Users retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<Common.Models.AdminUserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null,
        [FromQuery] string? status = null)
    {
        // Validate pagination parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100; // Limit max page size

        var query = new GetUsersQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Role = role,
            Status = status
        };
        return HandleResult(await _mediator.Send(query));
    }



    /// <summary>
    /// Get user details
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    /// <response code="200">User details retrieved successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<Common.Models.AdminUserDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUser(int id)
    {
        var query = new GetUserDetailsQuery { UserId = id };
        return HandleResult(await _mediator.Send(query));
    }



    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">User update data</param>
    /// <returns>Update result</returns>
    /// <response code="200">User updated successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
    {
        command.UserId = id;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Delete result</returns>
    /// <response code="200">User deleted successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var command = new DeleteUserCommand { UserId = id };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Suspend user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">Suspension data</param>
    /// <returns>Suspension result</returns>
    /// <response code="200">User suspended successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("{id}/suspend")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SuspendUser(int id, [FromBody] SuspendUserCommand command)
    {
        command.UserId = id;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Activate user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Activation result</returns>
    /// <response code="200">User activated successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ActivateUser(int id)
    {
        var command = new ActivateUserCommand { UserId = id };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Update user role
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">Role update data</param>
    /// <returns>Role update result</returns>
    /// <response code="200">User role updated successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPut("{id}/role")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleCommand command)
    {
        command.UserId = id;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get user analytics
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User analytics</returns>
    /// <response code="200">Analytics retrieved successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("{id}/analytics")]
    [ProducesResponseType(typeof(Result<Common.Models.UserAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserAnalytics(int id)
    {
        var query = new GetUserAnalyticsQuery { UserId = id };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Get user activity log
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>User activity log</returns>
    /// <response code="200">Activity log retrieved successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("{id}/activity")]
    [ProducesResponseType(typeof(Result<PagedResult<UserActivityDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserActivity(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetUserActivityQuery
        {
            UserId = id,
            Page = page,
            PageSize = pageSize
        };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Reset user password
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">Password reset data</param>
    /// <returns>Password reset result</returns>
    /// <response code="200">Password reset successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("{id}/reset-password")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetUserPassword(int id, [FromBody] AdminResetPasswordCommand command)
    {
        command.UserId = id;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get users analytics summary
    /// </summary>
    /// <returns>Users analytics summary</returns>
    /// <response code="200">Analytics retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("analytics")]
    [ProducesResponseType(typeof(Result<Common.Models.UsersAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsersAnalytics()
    {
        var query = new GetUsersAnalyticsQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Send message to user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">Message data</param>
    /// <returns>Send result</returns>
    /// <response code="200">Message sent successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("{id}/message")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendMessageToUser(int id, [FromBody] SendMessageToUserCommand command)
    {
        command.UserId = id;
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Export users data
    /// </summary>
    /// <param name="format">Export format (csv, excel)</param>
    /// <returns>Exported users data</returns>
    /// <response code="200">Data exported successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("export")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportUsers([FromQuery] string format = "csv")
    {
        var query = new ExportUsersQuery { Format = format };
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
            return HandleResult(result);
            
        var exportData = result.Data!;
        return File(exportData.Content, exportData.ContentType, exportData.FileName);
    }
}
