using Microsoft.EntityFrameworkCore.Storage;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Infrastructure.Data;

namespace SkillUpPlatform.Infrastructure.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;   
    public UnitOfWork(ApplicationDbContext context,
                        IUserRepository users,
                        ILearningPathRepository learningPaths,
                        IContentRepository contents,
                        IAssessmentRepository assessments,
                        IResourceRepository resources,
                        IUserProgressRepository userProgress,
                        IAssessmentResultRepository assessmentResults,
                        IQuestionRepository questions,
                        IUserAnswerRepository userAnswers,
                        IUserLearningPathRepository userLearningPaths,
                        IFileUploadRepository fileUploadRepository,
                        INotificationRepository notificationRepository,
                        IAchievementRepository achievementRepository,
                        IUserGoalRepository userGoalRepository,
                        IAuditLogRepository auditLogRepository,
                        IUserActivityRepository userActivityRepository,
                        ISystemHealthRepository systemHealthRepository,
                        IEmailVerificationTokenRepository emailVerificationTokens,
                        IPasswordResetTokenRepository passwordResetTokens,
                        IUserSessionRepository userSessions,
                        IFileShareRepository fileShareRepository)
    {
        _context = context;
        Users = users;
        LearningPaths = learningPaths;
        Contents = contents;
        Assessments = assessments;
        Resources = resources;
        UserProgress = userProgress;
        AssessmentResults = assessmentResults;
        Questions = questions;
        UserAnswers = userAnswers;
        UserLearningPaths = userLearningPaths;
        FileUploadRepository = fileUploadRepository;
        NotificationRepository = notificationRepository;
        AchievementRepository = achievementRepository;
        UserGoalRepository = userGoalRepository;
        AuditLogRepository = auditLogRepository;
        UserActivityRepository = userActivityRepository;
        SystemHealthRepository = systemHealthRepository;
        EmailVerificationTokens = emailVerificationTokens;
        PasswordResetTokens = passwordResetTokens;
        UserSessions = userSessions;
        FileShareRepository = fileShareRepository;
    }

    public IUserRepository Users { get; }
    public ILearningPathRepository LearningPaths { get; }
    public IContentRepository Contents { get; }
    public IAssessmentRepository Assessments { get; }
    public IResourceRepository Resources { get; }
    public IUserProgressRepository UserProgress { get; }
    public IAssessmentResultRepository AssessmentResults { get; }
    public IQuestionRepository Questions { get; }
    public IUserAnswerRepository UserAnswers { get; }
    public IUserLearningPathRepository UserLearningPaths { get; }
    public IFileUploadRepository FileUploadRepository { get; }
    public INotificationRepository NotificationRepository { get; }
    public IAchievementRepository AchievementRepository { get; }
    public IUserGoalRepository UserGoalRepository { get; }
    public IAuditLogRepository AuditLogRepository { get; }
    public IUserActivityRepository UserActivityRepository { get; }
    public ISystemHealthRepository SystemHealthRepository { get; }
    public IEmailVerificationTokenRepository EmailVerificationTokens { get; }
    public IPasswordResetTokenRepository PasswordResetTokens { get; }
    public IUserSessionRepository UserSessions {  get; }
    public IFileShareRepository FileShareRepository { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
