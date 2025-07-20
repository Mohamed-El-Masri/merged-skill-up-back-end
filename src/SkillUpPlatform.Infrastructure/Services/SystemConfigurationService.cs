using Microsoft.Extensions.Configuration;
using SkillUpPlatform.Application.Features.Admin.Commands;
using SkillUpPlatform.Application.Features.Admin.Queries;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Domain.Entities;

public class SystemConfigurationService : ISystemConfigurationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, string> _validConfigKeys = new()
    {
        { "AppName", "Application name" },
        { "Environment", "Current environment" },
        { "Version", "Application version" },
        { "DatabaseProvider", "Database provider" },
        { "JwtExpirationMinutes", "JWT token expiration time" },
        { "MaxFileSizeMB", "Maximum file size in MB" },
        { "SmtpHost", "SMTP server host" },
        { "SmtpPort", "SMTP server port" },
        { "FrontendUrl", "Frontend application URL" },
        { "ApiUrl", "API base URL" },
        { "DefaultPageSize", "Default pagination page size" },
        { "MaxPageSize", "Maximum pagination page size" },
        { "EnableSwagger", "Enable Swagger documentation" },
        { "EnableCors", "Enable CORS" },
        { "LogLevel", "Application log level" },
        { "user_role", "User role configuration" } // Allow the one you tested
    };

    public SystemConfigurationService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task UpdateConfigAsync(UpdateSystemConfigCommand config)
    {
        // Validate the configuration key
        if (!_validConfigKeys.ContainsKey(config.Key))
        {
            throw new ArgumentException($"Invalid configuration key: {config.Key}. Valid keys are: {string.Join(", ", _validConfigKeys.Keys)}");
        }

        // Check if setting exists in database
        var existingSetting = await _unitOfWork.SystemSettings.FindAsync(s => s.Key == config.Key);
        
        if (existingSetting.Any())
        {
            // Update existing setting
            var setting = existingSetting.First();
            setting.Value = config.Value;
            setting.Description = config.Description;
            setting.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.SystemSettings.Update(setting);
        }
        else
        {
            // Create new setting
            var newSetting = new SystemSettings
            {
                Key = config.Key,
                Value = config.Value,
                Description = config.Description,
                Category = "System",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.SystemSettings.AddAsync(newSetting);
        }

        await _unitOfWork.SaveChangesAsync();
        Console.WriteLine($"Updated config: {config.Key} = {config.Value} ({config.Description})");
    }

    public async Task<SystemConfigDto> GetConfigAsync()
    {
        // Get settings from database
        var dbSettings = await _unitOfWork.SystemSettings.GetAllAsync();
        var settingsDict = dbSettings.ToDictionary(s => s.Key, s => s.Value);

        // Merge with appsettings.json for complete configuration
        var appSettings = new Dictionary<string, string>
        {
            { "AppName", _configuration["AppName"] ?? "SkillUp Platform" },
            { "Environment", _configuration["Environment"] ?? "Development" },
            { "Version", _configuration["Version"] ?? "1.0.0" },
            { "DatabaseProvider", _configuration["ConnectionStrings:DefaultConnection"]?.Contains("SQL Server") == true ? "SQL Server" : "Unknown" },
            { "JwtExpirationMinutes", _configuration["Jwt:ExpirationMinutes"] ?? "60" },
            { "MaxFileSizeMB", _configuration["FileUpload:MaxFileSizeMB"] ?? "10" },
            { "SmtpHost", _configuration["Email:SmtpHost"] ?? "smtp.gmail.com" },
            { "SmtpPort", _configuration["Email:SmtpPort"] ?? "587" },
            { "FrontendUrl", _configuration["Frontend:Url"] ?? "http://localhost:4200" },
            { "ApiUrl", _configuration["Api:Url"] ?? "https://localhost:5001" },
            { "DefaultPageSize", _configuration["Pagination:DefaultPageSize"] ?? "20" },
            { "MaxPageSize", _configuration["Pagination:MaxPageSize"] ?? "100" },
            { "EnableSwagger", _configuration["Swagger:Enabled"] ?? "true" },
            { "EnableCors", _configuration["Cors:Enabled"] ?? "true" },
            { "LogLevel", _configuration["Logging:LogLevel:Default"] ?? "Information" }
        };

        // Merge database settings with appsettings (database settings take precedence)
        foreach (var setting in settingsDict)
        {
            appSettings[setting.Key] = setting.Value;
        }

        var dto = new SystemConfigDto
        {
            Settings = appSettings,
            LastUpdated = DateTime.UtcNow
        };

        return dto;
    }
}
