namespace SkillUpPlatform.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ILearningPathRepository LearningPaths { get; }
    IContentRepository Contents { get; }
    IAssessmentRepository Assessments { get; }
    IResourceRepository Resources { get; }
    IUserProgressRepository UserProgress { get; }
    IAssessmentResultRepository AssessmentResults { get; }
    IQuestionRepository Questions { get; }
    IUserAnswerRepository UserAnswers { get; }
    IUserLearningPathRepository UserLearningPaths { get; }
    IFileUploadRepository FileUploadRepository { get; }
    INotificationRepository NotificationRepository { get; }
    IAchievementRepository AchievementRepository { get; }
    IUserGoalRepository UserGoalRepository { get; }
    IAuditLogRepository AuditLogRepository { get; }
    IUserActivityRepository UserActivityRepository { get; }
    ISystemHealthRepository SystemHealthRepository { get; }
    IEmailVerificationTokenRepository EmailVerificationTokens { get; }
    IPasswordResetTokenRepository PasswordResetTokens { get; }
    IUserSessionRepository UserSessions { get; }
    IFileShareRepository FileShareRepository { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
