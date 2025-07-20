using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;

namespace SkillUpPlatform.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendNotificationAsync(int userId, string title, string message, NotificationType type, string? actionUrl = null, string? actionText = null)
    {
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                ActionUrl = actionUrl,
                ActionText = actionText,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            _logger.LogInformation($"Notification sent to user {userId}: {title}");
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send notification to user {userId}");
            return false;
        }
    }

    public async Task<bool> SendBulkNotificationAsync(List<int> userIds, string title, string message, NotificationType type, string? actionUrl = null, string? actionText = null)
    {
        try
        {
            var notifications = userIds.Select(userId => new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                ActionUrl = actionUrl,
                ActionText = actionText,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _notificationRepository.AddRangeAsync(notifications);
            _logger.LogInformation($"Bulk notification sent to {userIds.Count} users: {title}");
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send bulk notification to {userIds.Count} users");
            return false;
        }
    }

    public async Task<bool> SendEmailNotificationAsync(string email, string title, string message)
    {
        try
        {
            return await _emailService.SendNotificationEmailAsync(email, title, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email notification to {email}");
            return false;
        }
    }

    public async Task<bool> SendLearningReminderAsync(int userId, string learningPathTitle)
    {
        var title = "Learning Reminder";
        var message = $"Don't forget to continue your learning journey in '{learningPathTitle}'!";
        
        return await SendNotificationAsync(userId, title, message, NotificationType.Learning);
    }

    public async Task<bool> SendAssessmentReminderAsync(int userId, string assessmentTitle)
    {
        var title = "Assessment Reminder";
        var message = $"You have a pending assessment: '{assessmentTitle}'";
        
        return await SendNotificationAsync(userId, title, message, NotificationType.Assessment);
    }

    public async Task<bool> SendAchievementNotificationAsync(int userId, string achievementName)
    {
        var title = "Achievement Unlocked!";
        var message = $"Congratulations! You've earned the '{achievementName}' achievement!";
        
        return await SendNotificationAsync(userId, title, message, NotificationType.Achievement);
    }

    public async Task<bool> SendSystemNotificationAsync(int userId, string title, string message)
    {
        return await SendNotificationAsync(userId, title, message, NotificationType.System);
    }

    public async Task<bool> SendWelcomeNotificationAsync(int userId)
    {
        var title = "Welcome to SkillUp Platform!";
        var message = "Welcome to our learning community! Start exploring learning paths and assessments to begin your journey.";
        
        return await SendNotificationAsync(userId, title, message, NotificationType.System, "/dashboard", "Go to Dashboard");
    }

    public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
    {
        try
        {
            var notification = await _notificationRepository.SingleOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _notificationRepository.Update(notification);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to mark notification {notificationId} as read for user {userId}");
            return false;
        }
    }

    public async Task<bool> MarkAllAsReadAsync(int userId)
    {
        try
        {
            var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
            
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                _notificationRepository.Update(notification);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to mark all notifications as read for user {userId}");
            return false;
        }
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        try
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get unread count for user {userId}");
            return 0;
        }
    }
}
