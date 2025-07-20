using Microsoft.EntityFrameworkCore;
//using SkillUpPlatform.Application.DTOs;
using SkillUpPlatform.Application.Features.Admin.Queries;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Infrastructure.Data;
public class StatisticsService : IStatisticsService
{
    private readonly ApplicationDbContext _context;

    public StatisticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SystemStatisticsDto> GetStatisticsAsync()
    {
        var now = DateTime.UtcNow;
        var startOfToday = now.Date;
        var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        // Get all users to debug the issue
        var allUsers = await _context.Users.ToListAsync();
        var totalUsers = allUsers.Count;
        var newUsersToday = allUsers.Count(u => u.CreatedAt.Date == startOfToday);
        var newUsersThisWeek = allUsers.Count(u => u.CreatedAt >= startOfWeek);
        var newUsersThisMonth = allUsers.Count(u => u.CreatedAt >= startOfMonth);

        // Get all learning paths to debug the issue
        var allLearningPaths = await _context.LearningPaths.ToListAsync();
        var totalLearningPaths = allLearningPaths.Count;

        return new SystemStatisticsDto
        {
            Users = new UserStatistics
            {
                TotalUsers = totalUsers,
                NewUsersToday = newUsersToday,
                NewUsersThisWeek = newUsersThisWeek,
                NewUsersThisMonth = newUsersThisMonth
            },

            Content = new ContentStatistics
            {
                TotalLearningPaths = totalLearningPaths,
                TotalResources = await _context.Resources.CountAsync(),
                TotalAssessments = await _context.Assessments.CountAsync(),
                PublishedContent = await _context.Contents.CountAsync(c => c.IsPublished),
                DraftContent = await _context.Contents.CountAsync(c => !c.IsPublished)
            },

            Engagement = new EngagementStatistics
            {
                AverageSessionDuration = await _context.UserSessions.AnyAsync()
                    ? await _context.UserSessions.AverageAsync(s => s.DurationInSeconds)
                    : 0,

                TotalSessions = await _context.UserSessions.CountAsync(),
                CompletionRate = await _context.AssessmentResults.AnyAsync()
                    ? await _context.AssessmentResults.AverageAsync(r => r.Score)
                    : 0,

                TotalInteractions = await _context.UserActivities.CountAsync()
            },

            Performance = new PerformanceStatistics
            {
                // القيم دي Placeholder — عدليها لاحقًا لما يبقى فيه data فعلية
                AverageResponseTime = 150, // بالمللي ثانية مثلاً
                SystemUptime = 99.95,      // نسبة uptime للنظام
                TotalRequests = 2500,      // عدديها فعليًا لو عندك logs
                ErrorRate = 0           // نسبة الخطأ في الطلبات مثلاً
            }
        };
    }
}
