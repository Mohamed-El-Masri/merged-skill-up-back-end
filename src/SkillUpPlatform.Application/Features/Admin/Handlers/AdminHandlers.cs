// File: Application/Features/Admin/Handlers/AdminHandlers.cs

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Admin.Commands;
using SkillUpPlatform.Application.Features.Admin.Queries;
using SkillUpPlatform.Application.Features.Users.Queries;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GetUsersQuery = SkillUpPlatform.Application.Features.Admin.Queries.GetUsersQuery;
using CommonModels = SkillUpPlatform.Application.Features.Admin.Commands;
using BCrypt.Net; 
using System.Text;

namespace SkillUpPlatform.Application.Features.Admin.Handlers;

public class AdminLoginCommandHandler : IRequestHandler<AdminLoginCommand, AdminAuthResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AdminLoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AdminAuthResult> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || user.Role != UserRole.Admin)
        {
            return new AdminAuthResult { IsSuccess = false, Error = "Invalid credentials" };
        }

        // Assume password already hashed and verified externally for simplicity
        var token = _tokenService.GenerateJwtToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new AdminAuthResult
        {
            IsSuccess = true,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new AdminUserDto
            {
                Id= user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            }
        };
    }
}

public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILearningPathRepository _learningPathRepository;
    private readonly IContentRepository _contentRepository;

    public GetAdminDashboardQueryHandler(IUserRepository userRepository, ILearningPathRepository learningPathRepository, IContentRepository contentRepository)
    {
        _userRepository = userRepository;
        _learningPathRepository = learningPathRepository;
        _contentRepository = contentRepository;
    }

    public async Task<Result<AdminDashboardDto>> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        var totalUsers = (await _userRepository.GetAllAsync()).Count();
        var totalLearningPaths = (await _learningPathRepository.GetAllAsync()).Count();
        var totalContent = (await _contentRepository.GetAllAsync()).Count();
        var activeUsers = (await _userRepository.GetUsersByRoleAsync(UserRole.Student)).Count();

        var dashboard = new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            TotalLearningPaths = totalLearningPaths,
            TotalContent = totalContent,
            ActiveUsers = activeUsers,
            TotalRevenue = 0, // mock value or calculate if revenue logic is implemented
            RecentActivities = new(),
            SystemAlerts = new()
        };

        return Result<AdminDashboardDto>.Success(dashboard);
    }
}

public class GetSystemStatisticsQueryHandler : IRequestHandler<GetSystemStatisticsQuery, Result<SystemStatisticsDto>>
{
    private readonly IStatisticsService _statisticsService;

    public GetSystemStatisticsQueryHandler(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public async Task<Result<SystemStatisticsDto>> Handle(GetSystemStatisticsQuery request, CancellationToken cancellationToken)
    {
        var stats = await _statisticsService.GetStatisticsAsync();
        return Result<SystemStatisticsDto>.Success(stats);
    }
}

public class GetSystemHealthQueryHandler : IRequestHandler<GetSystemHealthQuery, Result<SystemHealthDto>>
{
    private readonly ISystemHealthRepository _healthRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetSystemHealthQueryHandler(ISystemHealthRepository healthRepository, IUnitOfWork unitOfWork)
    {
        _healthRepository = healthRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SystemHealthDto>> Handle(GetSystemHealthQuery request, CancellationToken cancellationToken)
    {
        var healthChecks = new List<HealthCheckDto>();
        var overallStatus = HealthStatus.Healthy;
        var startTime = DateTime.UtcNow;

        // Database Health Check
        try
        {
            var dbStartTime = DateTime.UtcNow;
            var users = await _unitOfWork.Users.GetAllAsync();
            var userCount = users.Count();
            var dbResponseTime = DateTime.UtcNow - dbStartTime;
            
            healthChecks.Add(new HealthCheckDto
            {
                Name = "Database",
                Status = "Healthy",
                Description = $"Database connection is working. Total users: {userCount}",
                ResponseTime = dbResponseTime
            });
        }
        catch (Exception ex)
        {
            healthChecks.Add(new HealthCheckDto
            {
                Name = "Database",
                Status = "Critical",
                Description = $"Database connection failed: {ex.Message}",
                ResponseTime = TimeSpan.Zero
            });
            overallStatus = HealthStatus.Critical;
        }

        // Memory Health Check
        try
        {
            var memoryInfo = GC.GetTotalMemory(false);
            var memoryMB = memoryInfo / (1024 * 1024);
            var isMemoryHealthy = memoryMB < 500; // Less than 500MB is healthy
            
            healthChecks.Add(new HealthCheckDto
            {
                Name = "Memory",
                Status = isMemoryHealthy ? "Healthy" : "Warning",
                Description = $"Memory usage: {memoryMB} MB",
                ResponseTime = TimeSpan.Zero
            });
            
            if (!isMemoryHealthy)
                overallStatus = overallStatus == HealthStatus.Healthy ? HealthStatus.Warning : overallStatus;
        }
        catch (Exception ex)
        {
            healthChecks.Add(new HealthCheckDto
            {
                Name = "Memory",
                Status = "Unknown",
                Description = $"Memory check failed: {ex.Message}",
                ResponseTime = TimeSpan.Zero
            });
        }

        // Application Health Check
        try
        {
            var appResponseTime = DateTime.UtcNow - startTime;
            healthChecks.Add(new HealthCheckDto
            {
                Name = "Application",
                Status = "Healthy",
                Description = "Application is running and responding",
                ResponseTime = appResponseTime
            });
        }
        catch (Exception ex)
        {
            healthChecks.Add(new HealthCheckDto
            {
                Name = "Application",
                Status = "Critical",
                Description = $"Application health check failed: {ex.Message}",
                ResponseTime = TimeSpan.Zero
            });
            overallStatus = HealthStatus.Critical;
        }

        // System Metrics
        var metrics = new Dictionary<string, object>
        {
            { "totalHealthChecks", healthChecks.Count },
            { "healthyChecks", healthChecks.Count(h => h.Status == "Healthy") },
            { "warningChecks", healthChecks.Count(h => h.Status == "Warning") },
            { "criticalChecks", healthChecks.Count(h => h.Status == "Critical") },
            { "uptime", Environment.TickCount / 1000.0 / 60.0 / 60.0 }, // Hours
            { "processId", Environment.ProcessId },
            { "machineName", Environment.MachineName }
        };

        var dto = new SystemHealthDto
        {
            Status = overallStatus,
            LastChecked = DateTime.UtcNow,
            HealthChecks = healthChecks,
            Metrics = metrics
        };

        return Result<SystemHealthDto>.Success(dto);
    }
}

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
{
    private readonly IAuditLogRepository _auditRepo;
    private readonly IMapper _mapper;

    public GetAuditLogsQueryHandler(IAuditLogRepository auditRepo, IMapper mapper)
    {
        _auditRepo = auditRepo;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _auditRepo.GetAllAsync();

        if (request.UserId.HasValue)
            logs = logs.Where(l => l.UserId == request.UserId.Value);

        if (!string.IsNullOrEmpty(request.Action))
            logs = logs.Where(l => l.Action == request.Action);

        if (request.StartDate.HasValue)
            logs = logs.Where(l => l.Timestamp >= request.StartDate);

        if (request.EndDate.HasValue)
            logs = logs.Where(l => l.Timestamp <= request.EndDate);

        var total = logs.Count();
        var items = logs
            .OrderByDescending(l => l.Timestamp)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var result = new PagedResult<AuditLogDto>
        {
            Data = _mapper.Map<List<AuditLogDto>>(items),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<AuditLogDto>>.Success(result);
    }
}

public class GetSystemConfigQueryHandler : IRequestHandler<GetSystemConfigQuery, Result<SystemConfigDto>>
{
    private readonly IConfiguration _configuration;

    public GetSystemConfigQueryHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<Result<SystemConfigDto>> Handle(GetSystemConfigQuery request, CancellationToken cancellationToken)
    {
        var settings = new Dictionary<string, string>
        {
            { "AppName", "SkillUp Platform" },
            { "Environment", _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development" },
            { "Version", "1.0.0" },
            { "DatabaseProvider", "SQL Server" },
            { "JwtExpirationMinutes", _configuration["JwtSettings:ExpirationInMinutes"] ?? "60" },
            { "MaxFileSizeMB", _configuration["FileStorage:MaxFileSizeInBytes"] ?? "10" },
            { "SmtpHost", _configuration["SmtpSettings:Host"] ?? "smtp.gmail.com" },
            { "SmtpPort", _configuration["SmtpSettings:Port"] ?? "587" },
            { "FrontendUrl", _configuration["FrontendUrl"] ?? "http://localhost:4200" },
            { "ApiUrl", _configuration["FileStorage:BaseUrl"] ?? "https://localhost:5001" },
            { "DefaultPageSize", "20" },
            { "MaxPageSize", "100" },
            { "EnableSwagger", "true" },
            { "EnableCors", "true" },
            { "LogLevel", _configuration["Logging:LogLevel:Default"] ?? "Information" }
        };

        var config = new SystemConfigDto
        {
            Settings = settings,
            LastUpdated = DateTime.UtcNow
        };

        return Task.FromResult(Result<SystemConfigDto>.Success(config));
    }
}

public class UpdateSystemConfigCommandHandler : IRequestHandler<UpdateSystemConfigCommand, Result<string>>
{
    private readonly ISystemConfigurationService _configService;

    public UpdateSystemConfigCommandHandler(ISystemConfigurationService configService)
    {
        _configService = configService;
    }

    public async Task<Result<string>> Handle(UpdateSystemConfigCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _configService.UpdateConfigAsync(request);
            return Result<string>.Success($"Configuration '{request.Key}' updated successfully to '{request.Value}'");
        }
        catch (ArgumentException ex)
        {
            return Result<string>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Failed to update configuration: {ex.Message}");
        }
    }
}

public class GetPerformanceMetricsQueryHandler : IRequestHandler<GetPerformanceMetricsQuery, Result<PerformanceMetricsDto>>
{
    private readonly IPerformanceService _performanceService;

    public GetPerformanceMetricsQueryHandler(IPerformanceService performanceService)
    {
        _performanceService = performanceService;
    }

    public async Task<Result<PerformanceMetricsDto>> Handle(GetPerformanceMetricsQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _performanceService.GetMetricsAsync(request.Period);
        return Result<PerformanceMetricsDto>.Success(metrics);
    }
}

public class GetErrorLogsQueryHandler : IRequestHandler<GetErrorLogsQuery, Result<PagedResult<ErrorLogDto>>>
{
    private readonly IErrorLogRepository _errorLogRepo;
    private readonly IMapper _mapper;

    public GetErrorLogsQueryHandler(IErrorLogRepository errorLogRepo, IMapper mapper)
    {
        _errorLogRepo = errorLogRepo;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ErrorLogDto>>> Handle(GetErrorLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _errorLogRepo.GetAllAsync();

        if (!string.IsNullOrEmpty(request.Severity))
            logs = logs.Where(e => e.Severity == request.Severity).ToList();

        if (request.StartDate.HasValue)
            logs = logs.Where(e => e.Timestamp >= request.StartDate).ToList();

        if (request.EndDate.HasValue)
            logs = logs.Where(e => e.Timestamp <= request.EndDate).ToList();

        var total = logs.Count();
        var items = logs
            .OrderByDescending(e => e.Timestamp)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var result = new PagedResult<ErrorLogDto>
        {
            Data = _mapper.Map<List<ErrorLogDto>>(items),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<ErrorLogDto>>.Success(result);
    }
}

public class CreateAdminUserCommandHandler : IRequestHandler<CreateAdminUserCommand, Result<AdminUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAdminUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AdminUserDto>> Handle(CreateAdminUserCommand request, CancellationToken cancellationToken)
    {
        var admin = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = request.Password, // should hash in production
            Role = UserRole.Admin,
            IsEmailVerified = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(admin);
        await _unitOfWork.SaveChangesAsync();

        return Result<AdminUserDto>.Success(new AdminUserDto
        {
            Id = admin.Id,
            Email = admin.Email,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            Role = admin.Role.ToString(),
            CreatedAt = admin.CreatedAt,
            LastLogin = admin.LastLoginAt ?? admin.CreatedAt,
            IsActive = admin.IsActive
        });
    }
}

public class GetPlatformAnalyticsQueryHandler : IRequestHandler<GetPlatformAnalyticsQuery, Result<PlatformAnalyticsDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetPlatformAnalyticsQueryHandler(
        IUserRepository userRepository,
        IUserProgressRepository progressRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _progressRepository = progressRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PlatformAnalyticsDto>> Handle(GetPlatformAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var monthAgo = now.AddMonths(-1);
        var thirtyDaysAgo = now.AddDays(-30);

        // Get basic metrics
        var newUsers = await _userRepository.CountNewUsersSinceAsync(monthAgo);
        var activeUsers = await _userRepository.CountActiveUsersSinceAsync(monthAgo);
        var totalViews = await _progressRepository.CountAllAsync();
        var totalCompletions = await _progressRepository.CountCompletedAsync();
        var averageTime = await _progressRepository.GetAverageEngagementTimeAsync();
        var totalRevenue = await _orderRepository.GetTotalRevenueAsync();
        var monthlyRevenue = await _orderRepository.GetMonthlyRevenueAsync(monthAgo);
        var avgOrderValue = await _orderRepository.GetAverageOrderValueAsync();

        // Get daily growth data for the last 30 days
        var dailyGrowth = new List<DailyGrowthDto>();
        for (int i = 29; i >= 0; i--)
        {
            var date = now.AddDays(-i).Date;
            var dayStart = date;
            var dayEnd = date.AddDays(1);
            
            var dayNewUsers = await _userRepository.CountNewUsersSinceAsync(dayStart);
            var dayActiveUsers = await _userRepository.CountActiveUsersSinceAsync(dayStart);
            
            dailyGrowth.Add(new DailyGrowthDto
            {
                Date = date,
                NewUsers = dayNewUsers,
                ActiveUsers = dayActiveUsers
            });
        }

        // Get category engagement data
        var learningPaths = await _unitOfWork.LearningPaths.FindAsync(lp => lp.IsActive);
        var categoryEngagement = learningPaths
            .GroupBy(lp => lp.Category)
            .Select(g => new CategoryEngagementDto
            {
                Category = g.Key,
                Views = g.Count() * 10, // Mock data - in real app, get from progress
                Completions = g.Count() * 2, // Mock data
                EngagementRate = g.Count() > 0 ? 20.0 : 0 // Mock data
            })
            .ToList();

        // Get monthly revenue trend for the last 12 months
        var monthlyTrend = new List<MonthlyRevenueDto>();
        for (int i = 11; i >= 0; i--)
        {
            var monthStart = now.AddMonths(-i).Date.AddDays(1 - now.AddMonths(-i).Day);
            var monthEnd = monthStart.AddMonths(1);
            var monthRevenue = await _orderRepository.GetMonthlyRevenueAsync(monthStart);
            
            monthlyTrend.Add(new MonthlyRevenueDto
            {
                Year = monthStart.Year,
                Month = monthStart.Month,
                Revenue = monthRevenue,
                Sales = (int)(monthRevenue / Math.Max(avgOrderValue, 1)) // Mock sales count
            });
        }

        // Get top earning learning paths
        var topEarningPaths = learningPaths
            .Take(5)
            .Select(lp => new LearningPathRevenueDto
            {
                Id = lp.Id,
                Title = lp.Title,
                Revenue = lp.Price * 10, // Mock revenue based on price
                Sales = 10, // Mock sales count
                AveragePrice = lp.Price
            })
            .OrderByDescending(lp => lp.Revenue)
            .ToList();

        // Get popular content
        var contents = await _unitOfWork.Contents.FindAsync(c => c.IsPublished);
        var popularContent = contents
            .Take(3)
            .Select(c => new PopularContentDto
            {
                Id = Guid.NewGuid(), // Convert int to Guid
                Title = c.Title,
                Type = c.ContentType.ToString(),
                Views = 100, // Mock data
                Completions = 25, // Mock data
                Rating = 4.5 // Mock data
            })
            .ToList();

        var analytics = new PlatformAnalyticsDto
        {
            UserGrowth = new UserGrowthDto
            {
                NewUsers = newUsers,
                ActiveUsers = activeUsers,
                RetentionRate = newUsers > 0 ? (activeUsers * 100) / newUsers : 0,
                DailyGrowth = dailyGrowth
            },
            ContentEngagement = new AdminContentEngagementDto
            {
                TotalViews = totalViews,
                TotalCompletions = totalCompletions,
                AverageEngagementTime = averageTime ?? 0,
                CategoryEngagement = categoryEngagement
            },
            Revenue = new RevenueAnalyticsDto
            {
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                AverageOrderValue = avgOrderValue,
                MonthlyTrend = monthlyTrend,
                TopEarningPaths = topEarningPaths
            },
            PopularContent = popularContent
        };

        return Result<PlatformAnalyticsDto>.Success(analytics);
    }
}

public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, Result<SkillUpPlatform.Application.Features.Admin.Queries.AdminUserDetailsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SkillUpPlatform.Application.Features.Admin.Queries.AdminUserDetailsDto>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result<SkillUpPlatform.Application.Features.Admin.Queries.AdminUserDetailsDto>.Failure("User not found");
        }

        // Get user's learning paths
        var userLearningPaths = await _unitOfWork.UserLearningPaths.FindAsync(ulp => ulp.UserId == request.UserId);
        var learningPathDtos = new List<Common.Models.UserLearningPathDto>();
        
        foreach (var ulp in userLearningPaths)
        {
            var learningPath = await _unitOfWork.LearningPaths.GetByIdAsync(ulp.LearningPathId);
            if (learningPath != null)
            {
                learningPathDtos.Add(new Common.Models.UserLearningPathDto
                {
                    Id = ulp.Id,
                    Title = learningPath.Title,
                    Description = learningPath.Description,
                    ProgressPercentage = 0, // Calculate based on progress
                    EnrollmentDate = ulp.EnrolledAt,
                    CompletionDate = ulp.CompletedAt,
                    Status = ulp.Status.ToString(),
                    ImageUrl = learningPath.ImageUrl ?? string.Empty
                });
            }
        }

        // Get user's recent activity (mock data for now)
        var recentActivity = new List<Common.Models.UserActivityDto>
        {
            new Common.Models.UserActivityDto
            {
                Id = 1,
                Action = "Login",
                Details = "User logged in",
                Timestamp = user.LastLoginAt ?? user.CreatedAt,
                EntityType = "User",
                EntityId = user.Id
            }
        };

        // Get user statistics (mock data for now)
        var statistics = new Common.Models.UserStatisticsDto
        {
            TotalTimeSpent = TimeSpan.Zero,
            TotalLearningPaths = learningPathDtos.Count,
            CompletedLearningPaths = learningPathDtos.Count(lp => lp.Status == "Completed"),
            TotalAssessments = 0,
            PassedAssessments = 0,
            AverageScore = 0,
            LoginCount = 1,
            LastLogin = user.LastLoginAt,
            CurrentStreak = 0
        };

        var userDetails = new SkillUpPlatform.Application.Features.Admin.Queries.AdminUserDetailsDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            ProfilePictureUrl = user.UserProfile?.ProfilePictureUrl,
            LearningPaths = learningPathDtos,
            RecentActivity = recentActivity,
            Statistics = statistics
        };

        return Result<SkillUpPlatform.Application.Features.Admin.Queries.AdminUserDetailsDto>.Success(userDetails);
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<AdminUserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AdminUserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result<AdminUserDto>.Failure("User not found");
        }

        // Update user properties
        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.IsActive = request.IsActive;

        // Update role if provided and valid
        if (!string.IsNullOrEmpty(request.Role) && Enum.TryParse<UserRole>(request.Role, out var newRole))
        {
            user.Role = newRole;
        }

        // Update the user
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        // Map to DTO and return
        var userDto = _mapper.Map<AdminUserDto>(user);
        return Result<AdminUserDto>.Success(userDto);
    }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PagedResult<AdminUserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<AdminUserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Search))
            users = users.Where(u => u.FirstName.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                                  || u.LastName.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                                  || u.Email.Contains(request.Search, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(request.Role))
            users = users.Where(u => u.Role.ToString() == request.Role).ToList();

        if (request.IsActive.HasValue)
            users = users.Where(u => u.IsActive == request.IsActive.Value).ToList();

        // TODO: Add Status filter if needed

        // Sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var sortDir = request.SortDirection?.ToLower() == "desc";
            users = request.SortBy.ToLower() switch
            {
                "firstname" => sortDir ? users.OrderByDescending(u => u.FirstName).ToList() : users.OrderBy(u => u.FirstName).ToList(),
                "email" => sortDir ? users.OrderByDescending(u => u.Email).ToList() : users.OrderBy(u => u.Email).ToList(),
                "createdat" => sortDir ? users.OrderByDescending(u => u.CreatedAt).ToList() : users.OrderBy(u => u.CreatedAt).ToList(),
                _ => users
            };
        }

        var total = users.Count();
        var paged = users
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dto = _mapper.Map<List<AdminUserDto>>(paged);
        var result = new PagedResult<AdminUserDto>
        {
            Data = dto,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<AdminUserDto>>.Success(result);
    }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        // Prevent deletion of admin users (optional security measure)
        if (user.Role == UserRole.Admin)
        {
            return Result.Failure("Cannot delete admin users");
        }

        // Check if user has any related data that should be handled
        var userLearningPaths = await _unitOfWork.UserLearningPaths.FindAsync(ulp => ulp.UserId == request.UserId);
        var userProgress = await _unitOfWork.UserProgress.FindAsync(up => up.UserId == request.UserId);
        var assessmentResults = await _unitOfWork.AssessmentResults.FindAsync(ar => ar.UserId == request.UserId);

        // Soft delete - mark as inactive
        user.IsActive = false;
        _unitOfWork.Users.Update(user);

        // Note: For hard delete, uncomment the line below
        // await _unitOfWork.Users.DeleteAsync(user);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}

public class SuspendUserCommandHandler : IRequestHandler<SuspendUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public SuspendUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SuspendUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        // Prevent suspension of admin users
        if (user.Role == UserRole.Admin)
        {
            return Result.Failure("Cannot suspend admin users");
        }

        user.IsActive = false;
        // Note: Suspension tracking properties not available in User entity
        // Consider adding these properties to User entity or use a separate Suspension entity

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        // Validate the new role
        if (!Enum.TryParse<UserRole>(request.NewRole, out var newRole))
        {
            return Result.Failure("Invalid role specified");
        }

        // Prevent changing admin roles (optional security measure)
        if (user.Role == UserRole.Admin && newRole != UserRole.Admin)
        {
            return Result.Failure("Cannot change admin user roles");
        }

        user.Role = newRole;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}

public class AdminResetPasswordCommandHandler : IRequestHandler<AdminResetPasswordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminResetPasswordCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        // Hash the new password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordHash = hashedPassword;
        // Note: PasswordResetAt property not available in User entity

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        user.IsActive = true;
        // Note: Suspension tracking properties not available in User entity

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}

public class SendMessageToUserCommandHandler : IRequestHandler<SendMessageToUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public SendMessageToUserCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Result> Handle(SendMessageToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        try
        {
            // Send email to user
            await _emailService.SendEmailAsync(
                user.Email,
                request.Subject,
                request.Message
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to send message: {ex.Message}");
        }
    }
}

public class ExportUsersQueryHandler : IRequestHandler<ExportUsersQuery, Result<FileDownloadDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ExportUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<FileDownloadDto>> Handle(ExportUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        
        try
        {
            var exportData = new FileDownloadDto();
            
            if (request.Format?.ToLower() == "excel")
            {
                // Generate Excel file
                exportData.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                exportData.FileName = $"users_export_{DateTime.UtcNow:yyyyMMdd}.xlsx";
                exportData.Content = GenerateExcelContent(users);
            }
            else
            {
                // Generate CSV file (default)
                exportData.ContentType = "text/csv";
                exportData.FileName = $"users_export_{DateTime.UtcNow:yyyyMMdd}.csv";
                exportData.Content = GenerateCsvContent(users);
            }

            return Result<FileDownloadDto>.Success(exportData);
        }
        catch (Exception ex)
        {
            return Result<FileDownloadDto>.Failure($"Failed to export users: {ex.Message}");
        }
    }

    private byte[] GenerateCsvContent(IEnumerable<User> users)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Id,FirstName,LastName,Email,Role,IsActive,CreatedAt,LastLoginAt");
        
        foreach (var user in users)
        {
            csv.AppendLine($"{user.Id},{user.FirstName},{user.LastName},{user.Email},{user.Role},{user.IsActive},{user.CreatedAt:yyyy-MM-dd HH:mm:ss},{user.LastLoginAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private byte[] GenerateExcelContent(IEnumerable<User> users)
    {
        // For now, return CSV content as Excel (you can implement proper Excel generation later)
        return GenerateCsvContent(users);
    }
}

//public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, Result<Common.Models.AdminUserDetailsDto>>
//{
//    private readonly IUserActivityRepository _activityRepository;
//    private readonly IMapper _mapper;

//    public GetUserDetailsQueryHandler(IUserActivityRepository activityRepository, IMapper mapper)
//    {
//        _activityRepository = activityRepository;
//        _mapper = mapper;
//    }

//    public async Task<Result<Common.Models.AdminUserDetailsDto>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
//    {
//        var allUsers = await _activityRepository.GetAllAsync(); // بترجع IEnumerable

//        var user = allUsers.FirstOrDefault(u => u.UserId == request.UserId); // غيّري اسم الخاصية حسب الداتا

//        if (user == null)
//            return Result<Common.Models.AdminUserDetailsDto>.Failure("User not found");

//        var userDto = _mapper.Map<Common.Models.AdminUserDetailsDto>(user);

//        return Result<Common.Models.AdminUserDetailsDto>.Success(userDto);
//    }

//    Task<Result<Common.Models.AdminUserDetailsDto>> IRequestHandler<GetUserDetailsQuery, Result<Common.Models.AdminUserDetailsDto>>.Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
//    {
//        throw new NotImplementedException();
//    }
//}


//public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
//{
//    private readonly ApplicationDbContext _context;

//    public UpdateUserCommandHandler(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
//    {
//        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

//        if (user == null)
//            return Result.Failure("User not found");

//        user.FirstName = request.FirstName;
//        user.LastName = request.LastName;
//        user.Email = request.Email;
//        user.Role = request.Role;
//        // لو في خصائص إضافية حدثها هنا

//        await _context.SaveChangesAsync(cancellationToken);

//        return Result.Success("User deleted successfully");
//    }
//}

//public class GetUserActivityQueryHandler : IRequestHandler<GetUserActivityQuery, Result<PagedResult<UserQueries_UserActivityDto>>>
//{
//    private readonly IUserActivityRepository _activityRepository;
//    private readonly IMapper _mapper;

//    public GetUserActivityQueryHandler(IUserActivityRepository activityRepository, IMapper mapper)
//    {
//        _activityRepository = activityRepository;
//        _mapper = mapper;
//    }

//    public async Task<Result<PagedResult<UserQueries_UserActivityDto>>> Handle(GetUserActivityQuery request, CancellationToken cancellationToken)
//    {
//        var activities = await _activityRepository.GetByUserIdAsync(request.UserId);

//        var total = activities.Count();
//        var paged = activities
//            .OrderByDescending(a => a.Timestamp)
//            .Skip((request.Page - 1) * request.PageSize)
//            .Take(request.PageSize)
//            .ToList();

//        var dto = _mapper.Map<List<UserQueries_UserActivityDto>>(paged);

//        var result = new PagedResult<UserQueries_UserActivityDto>
//        {
//            Data = dto,
//            TotalCount = total
//        };

//        return Result<PagedResult<UserQueries_UserActivityDto>>.Success(result);
//    }
//}
