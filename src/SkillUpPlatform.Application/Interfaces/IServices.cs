using SkillUpPlatform.Application.Features.AI.Commands;
using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Application.Interfaces;

public interface IAIService
{
    Task<string> ChatWithAssistantAsync(string message, string? context = null);
    Task<SkillAnalysisResult> AnalyzeSkillsAsync(List<string> skills, List<int> assessmentResults);
    Task<List<CareerRecommendation>> RecommendCareerAsync(List<string> skills, List<string> interests, string? educationLevel, string? experience);
    Task<string> GenerateCVAsync(int userId, string templateId);
    Task<List<string>> GenerateRecommendationsAsync(int userId, string recommendationType);
    Task<string> GenerateAssessmentFeedbackAsync(string assessmentTitle, int score, int maxScore, bool isPassed);
}

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    Task<bool> SendPasswordResetEmailAsync(string email, string resetToken);
    Task<bool> SendEmailVerificationAsync(string email, string verificationToken);
    Task<bool> SendWelcomeEmailAsync(string email, string firstName);
    Task<bool> SendNotificationEmailAsync(string email, string title, string message);
}

public interface IFileService
{
    Task<string> SaveFileAsync(byte[] fileContent, string fileName, string fileType);
    Task<byte[]> GetFileAsync(string filePath);
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
    Task<long> GetFileSizeAsync(string filePath);
    Task<string> GetFileUrlAsync(string filePath);
    bool IsValidFileType(string fileType);
    bool IsValidFileSize(long fileSize);
    string GenerateFileHash(byte[] fileContent);
}

public interface ITokenService
{
    string GenerateJwtToken(int userId, string email, string role);
    string GenerateRefreshToken();
    string GeneratePasswordResetToken();
    string GenerateEmailVerificationToken();
    bool ValidateToken(string token);
    int GetUserIdFromToken(string token);
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task ClearAsync();
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);
}

public interface INotificationService
{
    Task<bool> SendNotificationAsync(int userId, string title, string message, NotificationType type, string? actionUrl = null, string? actionText = null);
    Task<bool> SendBulkNotificationAsync(List<int> userIds, string title, string message, NotificationType type, string? actionUrl = null, string? actionText = null);
    Task<bool> SendEmailNotificationAsync(string email, string title, string message);
    Task<bool> SendLearningReminderAsync(int userId, string learningPathTitle);
    Task<bool> SendAssessmentReminderAsync(int userId, string assessmentTitle);
    Task<bool> SendAchievementNotificationAsync(int userId, string achievementName);
    Task<bool> SendSystemNotificationAsync(int userId, string title, string message);
    Task<bool> SendWelcomeNotificationAsync(int userId);
    Task<bool> MarkAsReadAsync(int notificationId, int userId);
    Task<bool> MarkAllAsReadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
}
