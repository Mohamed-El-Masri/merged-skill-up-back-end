using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<Result<AuthResult>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public int Role { get; set; } = 1; // Default to Student
}

public class LoginCommand : IRequest<Result<AuthResult>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class LogoutCommand : IRequest<Result<bool>>
{
    public int UserId { get; set; }
    public string SessionId { get; set; } = string.Empty;
}

public class ForgotPasswordCommand : IRequest<Result<bool>>
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordCommand : IRequest<Result<bool>>
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class VerifyEmailCommand : IRequest<Result<bool>>
{
    public string Token { get; set; } = string.Empty;
}

public class ResendEmailVerificationCommand : IRequest<Result<bool>>
{
    public int UserId { get; set; }
}

public class ChangePasswordCommand : IRequest<Result<bool>>
{
    public int UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class RefreshTokenCommand : IRequest<Result<AuthResult>>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RevokeTokenCommand : IRequest<Result<bool>>
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}

public class RegisterUserCommand : IRequest<Result<AuthResult>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public Common.Models.UserDto? User { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class ResendVerificationCommand : IRequest<Result>
{
    public string Email { get; set; } = string.Empty;
}

public class ValidateResetTokenCommand : IRequest<Result<bool>>
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
