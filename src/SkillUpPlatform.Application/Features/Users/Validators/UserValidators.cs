using FluentValidation;
using SkillUpPlatform.Application.Features.Users.Commands;

namespace SkillUpPlatform.Application.Features.Users.Validators;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .MaximumLength(50).WithMessage("Password must not exceed 50 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Specialization)
            .MaximumLength(100).WithMessage("Specialization must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Specialization));

        RuleFor(x => x.StudyYear)
            .GreaterThan(0).WithMessage("Study year must be greater than 0")
            .LessThanOrEqualTo(10).WithMessage("Study year must not exceed 10")
            .When(x => x.StudyYear.HasValue);

        RuleFor(x => x.CareerGoals)
            .MaximumLength(1000).WithMessage("Career goals must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.CareerGoals));
    }
}

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.Bio)
            .MaximumLength(2000).WithMessage("Bio must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.LinkedInUrl)
            .Must(BeValidUrl).WithMessage("Invalid LinkedIn URL format")
            .When(x => !string.IsNullOrEmpty(x.LinkedInUrl));

        RuleFor(x => x.GitHubUrl)
            .Must(BeValidUrl).WithMessage("Invalid GitHub URL format")
            .When(x => !string.IsNullOrEmpty(x.GitHubUrl));

        RuleFor(x => x.PortfolioUrl)
            .Must(BeValidUrl).WithMessage("Invalid Portfolio URL format")
            .When(x => !string.IsNullOrEmpty(x.PortfolioUrl));
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
