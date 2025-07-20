using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.Auth.Queries;

public class GetUserSessionsQuery : IRequest<Result<List<UserSessionDto>>>
{
    public int UserId { get; set; }
}

public class ValidateTokenQuery : IRequest<Result<bool>>
{
    public string Token { get; set; } = string.Empty;
}

public class GetPasswordResetTokenQuery : IRequest<Result<bool>>
{
    public string Token { get; set; } = string.Empty;
}

public class UserSessionDto
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public DateTime? LogoutTime { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsCurrent { get; set; }
}
