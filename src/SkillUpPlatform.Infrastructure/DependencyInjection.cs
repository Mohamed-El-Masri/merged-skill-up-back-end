using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.ContentCreator.Commands;
using SkillUpPlatform.Application.Features.ContentCreator.Handlers;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Infrastructure.Data;
using SkillUpPlatform.Infrastructure.Data.Repositories;
using SkillUpPlatform.Infrastructure.Services;
using SkillUpPlatform.Application.Features.Admin.Queries;
using SkillUpPlatform.Application.Features.Auth.Commands;

namespace SkillUpPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddMediatR(cfg =>
               cfg.RegisterServicesFromAssembly(typeof(CreatorCreateLearningPathCommandHandler).Assembly));
        services.AddMediatR(cfg =>
               cfg.RegisterServicesFromAssembly(typeof(GetCreatorLearningPathByIdQueryHandler).Assembly));
        services.AddMediatR(cfg =>
               cfg.RegisterServicesFromAssembly(typeof(GetUserAnalyticsQueryHandler).Assembly));
        services.AddMediatR(cfg =>
               cfg.RegisterServicesFromAssembly(typeof(GetUserActivityQueryHandler).Assembly));
        services.AddMediatR(cfg =>
               cfg.RegisterServicesFromAssembly(typeof(GetUsersAnalyticsQueryHandler).Assembly));
        services.AddMediatR(cfg =>
               cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));
        services.AddMediatR(cfg =>
               cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));


        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILearningPathRepository, LearningPathRepository>();
        services.AddScoped<IContentRepository, ContentRepository>();
        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IUserProgressRepository, UserProgressRepository>();
        services.AddScoped<IAssessmentResultRepository, AssessmentResultRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IUserAnswerRepository, UserAnswerRepository>();
        services.AddScoped<IUserLearningPathRepository, UserLearningPathRepository>();
        services.AddScoped<IFileUploadRepository, FileUploadRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IUserGoalRepository, UserGoalRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUserActivityRepository, UserActivityRepository>();
        services.AddScoped<ISystemHealthRepository, SystemHealthRepository>();
        services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();        // Services
        services.AddScoped<IAIService, AIService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ISystemConfigurationService, SystemConfigurationService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
      
        // Redis Cache (optional - can be configured later)
        // services.AddStackExchangeRedisCache(options =>
        // {
        //     options.Configuration = configuration.GetConnectionString("Redis");
        // });

        // Memory Cache
        services.AddMemoryCache();

        return services;
    }
}
