using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SkillUpPlatform.Application.Common.Constants;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Auth.Commands;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillUpPlatform.Application.Features.Auth.Handlers
{
    public class AuthHandlers :
        IRequestHandler<RegisterUserCommand, Result<AuthResult>>,
        IRequestHandler<LoginCommand, Result<AuthResult>>,
        IRequestHandler<RefreshTokenCommand, Result<AuthResult>>,
        IRequestHandler<LogoutCommand, Result<bool>>,
        IRequestHandler<ForgotPasswordCommand, Result<bool>>,
        IRequestHandler<ResetPasswordCommand, Result<bool>>,
        IRequestHandler<VerifyEmailCommand, Result<bool>>,
        IRequestHandler<ResendVerificationCommand, Result>,
        IRequestHandler<ChangePasswordCommand, Result<bool>>,
        IRequestHandler<ValidateResetTokenCommand, Result<bool>>

    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthHandlers(IUnitOfWork unitOfWork, 
                            IMapper mapper, 
                            ITokenService tokenService,
                            IEmailService emailService,
                            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<AuthResult>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists
            var EmailExist = await _unitOfWork.Users.ExistsByEmailAsync(request.Email);
            if (EmailExist)
            {
                return Result<AuthResult>.Failure(ErrorMessages.EmailAlreadyExists);
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
                Role = UserRole.Student,
                IsEmailVerified = false
            };

            try
            {
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userMap = _mapper.Map<AuthResult>(user);
                return Result<AuthResult>.Success(userMap);
            }
            catch (Exception ex) 
            {
                return Result<AuthResult>.Failure($"Failed to create register: {ex.Message}");
            }
        }

        public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            User? user = await _unitOfWork.Users.GetByEmailAsync(request.Email.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Result<AuthResult>.Failure(ErrorMessages.InvalidCredentials);
            }

            try
            {
                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                // Generate JWT token

                var token = _tokenService.GenerateJwtToken(user.Id, user.Email, user.Role.ToString());
                var refreshToken = _tokenService.GenerateRefreshToken();

                var userSession = new UserSession
                {
                    UserId = user.Id,
                    SessionId = Guid.NewGuid().ToString(),
                    RefreshToken = refreshToken,
                    LoginTime = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0",
                    UserAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown",
                    IsActive = true
                };
                await _unitOfWork.UserSessions.AddAsync(userSession);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                //_httpContextAccessor.HttpContext?.Items["SessionId"] = refreshToken;


                var result = new AuthResult
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    RefreshToken = refreshToken,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        CreatedAt = DateTime.UtcNow,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsActive = user.IsEmailVerified,
                        LastLoginAt = DateTime.UtcNow,
                        Role = user.Role.ToString(),
                    },
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                };

                return Result<AuthResult>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<AuthResult>.Failure($"Login failed : {ex.Message}");
            }
        }

        // RefreshTokenCommand Handler
        public async Task<Result<AuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Result<AuthResult>.Failure("Refresh token is required");

            var userSessions = await _unitOfWork.UserSessions.FindAsync(s => s.RefreshToken == request.RefreshToken);
            var session = userSessions.FirstOrDefault();

            if (session == null || session.ExpiresAt < DateTime.UtcNow)
                return Result<AuthResult>.Failure("Invalid or expired refresh token");

            var user = await _unitOfWork.Users.GetByIdAsync(session.UserId);
            if (user == null)
                return Result<AuthResult>.Failure("User not found");

            // Generate new tokens
            var newToken = _tokenService.GenerateJwtToken(user.Id, user.Email, user.Role.ToString());
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Update session
            session.RefreshToken = newRefreshToken;
            session.ExpiresAt = DateTime.UtcNow.AddDays(7);
            _unitOfWork.UserSessions.Update(session);
            await _unitOfWork.SaveChangesAsync();

            var result = new AuthResult
            {
                Success = true,
                Token = newToken,
                RefreshToken = newRefreshToken,
                User = new UserDto
                {
                    CreatedAt = DateTime.UtcNow,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.Id,
                    IsActive = user.IsEmailVerified,
                    LastLoginAt = DateTime.UtcNow,
                    Role = user.Role.ToString(),
                },
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                Message = "Token refreshed successfully"
            };

            return Result<AuthResult>.Success(result);
        }

        // LogoutCommand Handler  
        /* public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
         {
             try
             {
                 var session = await _unitOfWork.UserSessions.GetBySessionIdAsync(request.SessionId);
                 if (session == null || session.UserId != request.UserId || !session.IsActive)
                 {
                     return Result<bool>.Failure("Invalid or inactive session.");
                 }

                 session.IsActive = false;
                 session.LogoutTime = DateTime.UtcNow;
                 _unitOfWork.UserSessions.Update(session);
                 await _unitOfWork.SaveChangesAsync();

                 return Result<bool>.Success(true);
             }
             catch (Exception ex)
             {
                 return Result<bool>.Failure($"Logout failed: {ex.Message}");
             }
         }
 */

         public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.UserSessions.GetBySessionIdAsync(request.SessionId);
            if (session == null || session.UserId != request.UserId || !session.IsActive)
            {
                return Result<bool>.Failure("Invalid or inactive session.");
            }

            try
            {
                _unitOfWork.UserSessions.Remove(session);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to logout: {ex.Message}");
            }
        }

        // ForgotPasswordCommand Handler
        public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email.ToLower());
                if (user == null)
                {
                    return Result<bool>.Success(true);
                }

                var resetToken = _tokenService.GeneratePasswordResetToken();
                var passwordReset = new PasswordResetToken
                {
                    UserId = user.Id,
                    Token = resetToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };

                await _unitOfWork.PasswordResetTokens.AddAsync(passwordReset);
                await _unitOfWork.SaveChangesAsync();

                await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to send password reset email: {ex.Message}");
            }
        }

        // ResetPasswordCommand Handler
        public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return Result<bool>.Failure("Passwords do not match.");
            }

            try
            {
                var resetToken = await _unitOfWork.PasswordResetTokens.GetValidTokenAsync(request.Token);
                if (resetToken == null || resetToken.ExpiresAt < DateTime.UtcNow)
                {
                    return Result<bool>.Failure("Invalid or expired reset token.");
                }

                var user = await _unitOfWork.Users.GetByIdAsync(resetToken.UserId);
                if (user == null)
                {
                    return Result<bool>.Failure("User not found.");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                resetToken.IsUsed = true;
                _unitOfWork.Users.Update(user);
                _unitOfWork.PasswordResetTokens.Update(resetToken);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to reset password: {ex.Message}");
            }
        }

        /*public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var resetToken = await _unitOfWork.PasswordResetTokens.GetValidTokenAsync(request.Token);
            if (resetToken == null || resetToken.ExpiresAt < DateTime.UtcNow)
            {
                return Result<bool>.Failure("Invalid or expired reset token");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(resetToken.UserId);
            if (user == null)
            {
                return Result<bool>.Failure("User not found");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return Result<bool>.Failure("Passwords do not match");
            }

            try
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                _unitOfWork.Users.Update(user);
                _unitOfWork.PasswordResetTokens.Remove(resetToken);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to reset password: {ex.Message}");
            }
        }*/




        // VerifyEmailCommand Handler
        public async Task<Result<bool>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var verificationToken = await _unitOfWork.EmailVerificationTokens.GetValidTokenAsync(request.Token);
                if (verificationToken == null || verificationToken.ExpiresAt < DateTime.UtcNow)
                {
                    return Result<bool>.Failure("Invalid or expired verification token.");
                }

                var user = await _unitOfWork.Users.GetByIdAsync(verificationToken.UserId);
                if (user == null)
                {
                    return Result<bool>.Failure("User not found.");
                }

                if (user.IsEmailVerified)
                {
                    return Result<bool>.Failure("Email already verified.");
                }

                user.IsEmailVerified = true;
                verificationToken.IsUsed = true;
                _unitOfWork.Users.Update(user);
                _unitOfWork.EmailVerificationTokens.Update(verificationToken);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to verify email: {ex.Message}");
            }
        }

        // ResendVerificationCommand Handler
        public async Task<Result> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email.ToLower());
                if (user == null)
                {
                    return Result.Failure("User not found.");
                }

                if (user.IsEmailVerified)
                {
                    return Result.Failure("Email already verified.");
                }

                var verificationToken = _tokenService.GenerateEmailVerificationToken();
                var emailVerification = new EmailVerificationToken
                {
                    UserId = user.Id,
                    Token = verificationToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                await _unitOfWork.EmailVerificationTokens.AddAsync(emailVerification);
                await _unitOfWork.SaveChangesAsync();

                await _emailService.SendEmailVerificationAsync(user.Email, verificationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to resend verification email: {ex.Message}");
            }
        }

        // ChangePasswordCommand Handler
        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return Result<bool>.Failure("New passwords do not match.");
            }

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return Result<bool>.Failure("User not found.");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                {
                    return Result<bool>.Failure("Current password is incorrect.");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to change password: {ex.Message}");
            }
        }

        // ValidateResetTokenCommand Handler
        public async Task<Result<bool>> Handle(ValidateResetTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var resetToken = await _unitOfWork.PasswordResetTokens.GetValidTokenAsync(request.Token);
                if (resetToken == null || resetToken.ExpiresAt < DateTime.UtcNow)
                {
                    return Result<bool>.Failure("Invalid or expired reset token.");
                }

                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email.ToLower());
                if (user == null || user.Id != resetToken.UserId)
                {
                    return Result<bool>.Failure("Invalid email or token.");
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to validate reset token: {ex.Message}");
            }
        }
    }
}

