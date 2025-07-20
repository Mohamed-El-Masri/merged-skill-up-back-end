using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SkillUpPlatform.Application.Features.Notifications.Commands;
using SkillUpPlatform.Application.Features.Notifications.Queries;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Notification management endpoints for all users
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("👨‍🎓 Student - Notifications")]
public class NotificationsController : BaseController
{
    public NotificationsController(IMediator mediator) : base(mediator)
    {
    }
    /// <summary>
    /// Get user notifications
    /// </summary>
    /// <param name="unreadOnly">Get only unread notifications (default: false)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>List of user notifications</returns>
    /// <response code="200">Notifications retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<NotificationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetNotificationsQuery
        {
            UnreadOnly = unreadOnly,
            Page = page,
            PageSize = pageSize
        };
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <returns>Mark as read result</returns>
    /// <response code="200">Notification marked as read</response>
    /// <response code="404">Notification not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpPost("{id}/read")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var command = new MarkNotificationAsReadCommand { NotificationId = id };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    /// <returns>Mark all as read result</returns>
    /// <response code="200">All notifications marked as read</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("mark-all-read")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var command = new MarkAllNotificationsAsReadCommand();
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <returns>Delete result</returns>
    /// <response code="200">Notification deleted successfully</response>
    /// <response code="404">Notification not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteNotificationCommand { NotificationId = id };
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get notification settings
    /// </summary>
    /// <returns>User notification settings</returns>
    /// <response code="200">Settings retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("settings")]
    [ProducesResponseType(typeof(Result<NotificationSettingsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSettings()
    {
        var query = new GetNotificationSettingsQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Update notification settings
    /// </summary>
    /// <param name="command">Notification settings</param>
    /// <returns>Update result</returns>
    /// <response code="200">Settings updated successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpPut("settings")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateNotificationSettingsCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get unread notifications count
    /// </summary>
    /// <returns>Count of unread notifications</returns>
    /// <response code="200">Count retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var query = new GetUnreadNotificationsCountQuery();
        return HandleResult(await _mediator.Send(query));
    }

    /// <summary>
    /// Send notification to user (Admin only)
    /// </summary>
    /// <param name="command">Notification data</param>
    /// <returns>Send result</returns>
    /// <response code="200">Notification sent successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpPost("send")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Send bulk notifications (Admin only)
    /// </summary>
    /// <param name="command">Bulk notification data</param>
    /// <returns>Send result</returns>
    /// <response code="200">Notifications sent successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpPost("send-bulk")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendBulkNotifications([FromBody] SendBulkNotificationCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Get notification templates (Admin only)
    /// </summary>
    /// <returns>List of notification templates</returns>
    /// <response code="200">Templates retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Access denied</response>
    [HttpGet("templates")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<List<NotificationTemplateDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetTemplates()
    {
        var query = new GetNotificationTemplatesQuery();
        return HandleResult(await _mediator.Send(query));
    }
}
