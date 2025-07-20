using MediatR;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.Users.Commands;

public class CreateUserCommand : IRequest<Result<int>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Specialization { get; set; }
    public int? StudyYear { get; set; }
    public string? CareerGoals { get; set; }
}

public class LoginUserCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UpdateUserProfileCommand : IRequest<Result<bool>>
{
    public int UserId { get; set; }
    public string? Bio { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public List<string>? Skills { get; set; }
    public List<string>? Interests { get; set; }
    public List<string>? Certifications { get; set; }
}

public class UpdateUserNotificationSettingsCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool LearningReminders { get; set; }
    public bool AchievementNotifications { get; set; }
    public bool NewsletterSubscription { get; set; }
    public bool WeeklyProgressReport { get; set; }
}

public class DeleteUserAccountCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UploadProfilePictureCommand : IRequest<Result<string>>
{
    public Guid UserId { get; set; }
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}
