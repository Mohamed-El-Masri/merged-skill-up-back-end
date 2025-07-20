using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using BCrypt.Net;

namespace SkillUpPlatform.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email.ToLower());
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResult>.Failure("Invalid credentials");
        }

        // تحقق من أن الدور Student فقط
        if (user.Role != UserRole.Student)
        {
            return Result<AuthResult>.Failure("Not a student");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        // Generate JWT token
        var token = _tokenService.GenerateJwtToken(user.Id, user.Email, user.Role.ToString());

        return Result<AuthResult>.Success(new AuthResult {
            Success = true,
            Token = token,
            Message = "Login successful",
            User = null, // يمكنك لاحقاً إرجاع UserDto
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }
} 