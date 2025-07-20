using Microsoft.AspNetCore.Mvc;
using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected BaseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected int GetUserIdFromContext()
    {
        if (HttpContext.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is int userId)
        {
            return userId;
        }
        return 0;
    }

    protected int GetCurrentUserId()
    {
        return GetUserIdFromContext();
    }

    protected string GetCurrentUserEmail()
    {
        if (HttpContext.Items.TryGetValue("Email", out var emailObj) && emailObj is string email)
        {
            return email;
        }
        return string.Empty;
    }

    protected string GetCurrentUserRole()
    {
        if (HttpContext.Items.TryGetValue("Role", out var roleObj) && roleObj is string role)
        {
            return role;
        }
        return string.Empty;
    }

    protected bool IsUserInRole(string role)
    {
        return GetCurrentUserRole().Equals(role, StringComparison.OrdinalIgnoreCase);
    }

    protected bool IsCurrentUser(int userId)
    {
        return GetCurrentUserId() == userId;
    }

    protected string GetClientIpAddress()
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0];
        }
        return ipAddress ?? "Unknown";
    }

    protected string GetUserAgent()
    {
        return Request.Headers["User-Agent"].ToString();
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result == null)
            return NotFound();

        if (result.IsSuccess && result.Data is not null)
            return Ok(result.Data);

        if (result.IsSuccess && result.Data is null)
            return NotFound();

        if (!result.IsSuccess && result.Error is not null)
            return BadRequest(result.Error);

        if (!result.IsSuccess && result.Errors.Any())
            return BadRequest(result.Errors);

        return BadRequest("Unknown error occurred.");
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result == null)
            return NotFound();

        if (result.IsSuccess)
            return Ok();

        if (!result.IsSuccess && result.Error is not null)
            return BadRequest(result.Error);

        if (!result.IsSuccess && result.Errors.Any())
            return BadRequest(result.Errors);

        return BadRequest("Unknown error occurred.");
    }
}
