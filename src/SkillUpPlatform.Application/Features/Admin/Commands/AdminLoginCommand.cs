using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.Admin.Commands;

public class AdminLoginCommand : IRequest<AdminAuthResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AdminAuthResult
{
    public bool IsSuccess { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public AdminUserDto User { get; set; } = new();
    public string Error { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
