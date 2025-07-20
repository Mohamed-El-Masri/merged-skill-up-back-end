using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using BCrypt.Net;

namespace SkillUpPlatform.Application.Features.Auth.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public RegisterUserCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResult>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // تحقق من عدم وجود المستخدم مسبقاً
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email.ToLower());
        if (existingUser != null)
            return Result<AuthResult>.Failure("Email already exists");

        // أنشئ المستخدم الجديد
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Role = Enum.TryParse<UserRole>(request.Role, out var role) ? role : UserRole.Student,
            IsEmailVerified = true,
            IsActive = true
        };
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // أنشئ التوكن
        var token = _tokenService.GenerateJwtToken(user.Id, user.Email, user.Role.ToString());

        return Result<AuthResult>.Success(new AuthResult {
            Success = true,
            Token = token,
            Message = "Registration successful",
            User = null, // يمكنك لاحقاً إرجاع UserDto
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }
} 