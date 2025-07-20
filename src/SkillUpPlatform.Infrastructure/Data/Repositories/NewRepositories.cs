using Microsoft.EntityFrameworkCore;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Infrastructure.Data;
using System.Linq.Expressions;
using FileShare = SkillUpPlatform.Domain.Entities.FileShare;

namespace SkillUpPlatform.Infrastructure.Data.Repositories;

public class FileUploadRepository : GenericRepository<FileUpload>, IFileUploadRepository
{
    public FileUploadRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<FileUpload>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(f => f.UploadedBy == userId)
            .Include(f => f.User)
            .Include(f => f.FileShares)
            .ThenInclude(fs => fs.SharedWithUser)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();
    }

    public async Task<List<FileUpload>> GetPublicFilesAsync()
    {
        return await _dbSet
            .Where(f => f.IsPublic)
            .Include(f => f.User)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();
    }

    public async Task<List<FileUpload>> GetSharedWithUserAsync(int userId)
    {
        return await _dbSet
            .Where(f => f.FileShares.Any(fs => fs.SharedWithUserId == userId))
            .Include(f => f.User)
            .Include(f => f.FileShares)
            .ThenInclude(fs => fs.SharedByUser)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();
    }
}

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Notification>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetUnreadByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _dbSet
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }
}

public class AchievementRepository : GenericRepository<Achievement>, IAchievementRepository
{
    public AchievementRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Achievement>> GetActiveAchievementsAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<List<UserAchievement>> GetUserAchievementsAsync(int userId)
    {
        return await _context.Set<UserAchievement>()
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Achievement)
            .OrderByDescending(ua => ua.EarnedAt)
            .ToListAsync();
    }
}

public class UserGoalRepository : GenericRepository<UserGoal>, IUserGoalRepository
{
    public UserGoalRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<UserGoal>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(ug => ug.UserId == userId)
            .OrderByDescending(ug => ug.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<UserGoal>> GetActiveGoalsAsync(int userId)
    {
        return await _dbSet
            .Where(ug => ug.UserId == userId && ug.Status == GoalStatus.Active)
            .OrderBy(ug => ug.TargetDate)
            .ToListAsync();
    }
}

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<AuditLog>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync();
    }

    public async Task<List<AuditLog>> GetByEntityAsync(string entityType, int entityId)
    {
        return await _dbSet
            .Where(al => al.EntityType == entityType && al.EntityId == entityId)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync();
    }
}

public class UserActivityRepository : GenericRepository<UserActivity>, IUserActivityRepository
{
    public UserActivityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<UserActivity>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(ua => ua.UserId == userId)
            .OrderByDescending(ua => ua.Timestamp)
            .ToListAsync();
    }

    public async Task<List<UserActivity>> GetRecentActivityAsync(int userId, int count)
    {
        return await _dbSet
            .Where(ua => ua.UserId == userId)
            .OrderByDescending(ua => ua.Timestamp)
            .Take(count)
            .ToListAsync();
    }
}

public class SystemHealthRepository : GenericRepository<SystemHealth>, ISystemHealthRepository
{
    public SystemHealthRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<SystemHealth>> GetLatestHealthChecksAsync()
    {
        return await _dbSet
            .GroupBy(sh => sh.Component)
            .Select(g => g.OrderByDescending(sh => sh.CheckedAt).First())
            .ToListAsync();
    }

    public async Task<SystemHealth?> GetByComponentAsync(string component)
    {
        return await _dbSet
            .Where(sh => sh.Component == component)
            .OrderByDescending(sh => sh.CheckedAt)
            .FirstOrDefaultAsync();
    }
}

public class PasswordResetTokenRepository : GenericRepository<PasswordResetToken>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
    {
        return await _dbSet
            .Where(prt => prt.Token == token && !prt.IsUsed && prt.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();
    }

    public async Task<List<PasswordResetToken>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(prt => prt.UserId == userId)
            .OrderByDescending(prt => prt.CreatedAt)
            .ToListAsync();
    }
}

public class EmailVerificationTokenRepository : GenericRepository<EmailVerificationToken>, IEmailVerificationTokenRepository
{
    public EmailVerificationTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<EmailVerificationToken?> GetValidTokenAsync(string token)
    {
        return await _dbSet
            .Where(evt => evt.Token == token && !evt.IsUsed && evt.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();
    }

    public async Task<List<EmailVerificationToken>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(evt => evt.UserId == userId)
            .OrderByDescending(evt => evt.CreatedAt)
            .ToListAsync();
    }
}

public class UserSessionRepository : GenericRepository<UserSession>, IUserSessionRepository
{
    public UserSessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<UserSession>> GetActiveSessionsAsync(int userId)
    {
        return await _dbSet
            .Where(us => us.UserId == userId && us.IsActive)
            .OrderByDescending(us => us.LoginTime)
            .ToListAsync();
    }

    public async Task<UserSession?> GetBySessionIdAsync(string sessionId)
    {
        return await _dbSet
            .Where(us => us.SessionId == sessionId)
            .FirstOrDefaultAsync();
    }
}

public class FileShareRepository : GenericRepository<FileShare>, IFileShareRepository
{
    public FileShareRepository(ApplicationDbContext context) : base(context)
    {
    }

}
