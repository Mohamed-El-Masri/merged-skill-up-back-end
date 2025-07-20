using Microsoft.AspNetCore.Mvc;
using SkillUpPlatform.Application.Features.Users.Commands;
using SkillUpPlatform.Application.Features.Users.Queries;
using SkillUpPlatform.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// User profile management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Tags("👨‍🎓 Student - Authentication & Profile")]
public class UsersController : BaseController
{
    public UsersController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var query = new GetUserProfileQuery { UserId = GetUserIdFromContext() };
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileCommand command)
    {
        command.UserId = GetUserIdFromContext();
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(new { Message = "Profile updated successfully" });
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(new { Message = "Password changed successfully" });
    }

    /// <summary>
    /// Update user preferences
    /// </summary>
    [HttpPut("preferences")]
    [Authorize]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdateUserPreferencesCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(new { Message = "Preferences updated successfully" });
    }

    /// <summary>
    /// Get user statistics
    /// </summary>
    [HttpGet("statistics")]
    [Authorize]
    public async Task<IActionResult> GetUserStatistics()
    {
        var query = new GetUserStatisticsQuery { UserId = GetUserIdFromContext() };
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Get user achievements
    /// </summary>
    [HttpGet("achievements")]
    [Authorize]
    public async Task<IActionResult> GetUserAchievements()
    {
        var query = new GetUserAchievementsQuery { UserId = GetUserIdFromContext() };
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Data);
    }
}
