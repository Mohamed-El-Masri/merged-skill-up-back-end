using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SkillUpPlatform.Application.Features.Users.Commands;
using SkillUpPlatform.Application.Features.Users.Queries;
using SkillUpPlatform.Application.Features.Auth.Commands;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.API.Controllers;

/// <summary>
/// Authentication endpoints for all user roles
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("👨‍🎓 Student - Authentication & Profile")]
public class AuthController : BaseController
{
    public AuthController(IMediator mediator) : base(mediator)
    {
    }
    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="command">User registration data</param>
    /// <returns>Registration result with JWT token</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Invalid registration data</response>
    /// <response code="409">Email already exists</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<AuthResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="command">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    /// <response code="403">Account not verified or suspended</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<AuthResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Refresh JWT token
    /// </summary>
    /// <param name="command">Refresh token data</param>
    /// <returns>New JWT token</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="401">Invalid refresh token</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<AuthResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Request password reset email
    /// </summary>
    /// <param name="command">Email address for password reset</param>
    /// <returns>Password reset email sent</returns>
    /// <response code="200">Password reset email sent (always returns 200 for security)</response>
    /// <response code="400">Invalid email format</response>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Reset password using reset token
    /// </summary>
    /// <param name="command">Reset token and new password</param>
    /// <returns>Password reset result</returns>
    /// <response code="200">Password reset successfully</response>
    /// <response code="400">Invalid or expired reset token</response>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Verify email address
    /// </summary>
    /// <param name="command">Email verification token</param>
    /// <returns>Email verification result</returns>
    /// <response code="200">Email verified successfully</response>
    /// <response code="400">Invalid or expired verification token</response>
    [HttpPost("verify-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Resend email verification
    /// </summary>
    /// <param name="command">Email address to resend verification</param>
    /// <returns>Verification email sent</returns>
    /// <response code="200">Verification email sent</response>
    /// <response code="400">Invalid email or already verified</response>
    [HttpPost("resend-verification")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Logout user (invalidate token)
    /// </summary>
    /// <returns>Logout result</returns>
    /// <response code="200">Logout successful</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var command = new LogoutCommand();
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="command">Current and new password</param>
    /// <returns>Password change result</returns>
    /// <response code="200">Password changed successfully</response>
    /// <response code="400">Invalid current password</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }

    /// <summary>
    /// Validate reset token
    /// </summary>
    /// <param name="command">Reset token to validate</param>
    /// <returns>Token validation result</returns>
    /// <response code="200">Token is valid</response>
    /// <response code="400">Invalid or expired token</response>
    [HttpPost("validate-reset-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateResetToken([FromBody] ValidateResetTokenCommand command)
    {
        return HandleResult(await _mediator.Send(command));
    }
}
