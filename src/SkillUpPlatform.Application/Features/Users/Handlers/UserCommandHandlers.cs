using MediatR;
using SkillUpPlatform.Application.Common.Constants;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Users.Commands;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using BCrypt.Net;

namespace SkillUpPlatform.Application.Features.Users.Handlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email))
        {
            return Result<int>.Failure(ErrorMessages.EmailAlreadyExists);
        }

        // Create new user
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Specialization = request.Specialization,
            StudyYear = request.StudyYear,
            CareerGoals = request.CareerGoals,
            Role = UserRole.Student,
            IsEmailVerified = false
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<int>.Success(user.Id);
    }
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email.ToLower());
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result<string>.Failure(ErrorMessages.InvalidCredentials);
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        // Generate JWT token
        var token = _tokenService.GenerateJwtToken(user.Id, user.Email, user.Role.ToString());

        return Result<string>.Success(token);
    }
}

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserWithProfileAsync(request.UserId);
        
        if (user == null)
        {
            return Result<bool>.Failure(ErrorMessages.UserNotFound);
        }

        // Update or create user profile
        if (user.UserProfile == null)
        {
            user.UserProfile = new UserProfile
            {
                UserId = user.Id
            };
        }

        user.UserProfile.Bio = request.Bio;
        user.UserProfile.LinkedInUrl = request.LinkedInUrl;
        user.UserProfile.GitHubUrl = request.GitHubUrl;
        user.UserProfile.PortfolioUrl = request.PortfolioUrl;
        user.UserProfile.Skills = request.Skills != null ? System.Text.Json.JsonSerializer.Serialize(request.Skills) : null;
        user.UserProfile.Interests = request.Interests != null ? System.Text.Json.JsonSerializer.Serialize(request.Interests) : null;
        user.UserProfile.Certifications = request.Certifications != null ? System.Text.Json.JsonSerializer.Serialize(request.Certifications) : null;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}
