using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.Admin.Commands;

public class CreateAdminUserCommand : IRequest<Result<AdminUserDto>>
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class UpdateSystemConfigCommand : IRequest<Result>
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateUserCommand : IRequest<Result<AdminUserDto>>
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class SuspendUserCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime? SuspensionEndDate { get; set; }
}

public class UpdateUserRoleCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public string NewRole { get; set; } = string.Empty;
}

public class AdminResetPasswordCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}

public class SendMessageToUserCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class DeleteUserCommand : IRequest<Result>
{
    public int UserId { get; set; }
}

public class ActivateUserCommand : IRequest<Result>
{
    public int UserId { get; set; }
}

public class GetUserActivityQuery : IRequest<Result<List<UserActivityDto>>>
{
    public int UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class ExportUsersQuery : IRequest<Result<Files.Queries.FileDownloadDto>>
{
    public string? Format { get; set; } = "csv";
    public string? FilterByRole { get; set; }
    public bool? IsActive { get; set; }
}
