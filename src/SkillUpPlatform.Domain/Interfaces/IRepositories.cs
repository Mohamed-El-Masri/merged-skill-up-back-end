using SkillUpPlatform.Domain.Entities;
using System.Collections;
using System.Linq.Expressions;
using FileShare = SkillUpPlatform.Domain.Entities.FileShare;

namespace SkillUpPlatform.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetUserWithProfileAsync(int userId);
    Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
}

public interface ILearningPathRepository : IGenericRepository<LearningPath>
{
    Task<IEnumerable<LearningPath>> GetActiveLearningPathsAsync();
    Task<LearningPath?> GetLearningPathWithContentsAsync(int learningPathId);
    Task<IEnumerable<LearningPath>> GetLearningPathsByCategoryAsync(string category);
    Task<IEnumerable<LearningPath>> GetRecommendedLearningPathsAsync(int userId);
}

public interface IContentRepository : IGenericRepository<Content>
{
    Task<IEnumerable<Content>> GetContentByLearningPathAsync(int learningPathId);
    Task<Content?> GetNextContentAsync(int currentContentId, int learningPathId);
    Task<Content?> GetPreviousContentAsync(int currentContentId, int learningPathId);
}

public interface IAssessmentRepository : IGenericRepository<Assessment>
{
    Task<Assessment?> GetAssessmentWithQuestionsAsync(int assessmentId);
    Task<Assessment?> GetByIdWithQuestionsAsync(int assessmentId);
    Task<IEnumerable<Assessment>> GetAssessmentsByTypeAsync(AssessmentType assessmentType);
    Task<IEnumerable<Assessment>> GetAssessmentsByLearningPathAsync(int learningPathId);
    Task<IEnumerable<Assessment>> GetFilteredAssessmentsAsync(string? category, int? learningPathId);
}

public interface IResourceRepository : IGenericRepository<Resource>
{
    Task<IEnumerable<Resource>> GetResourcesByTypeAsync(ResourceType resourceType);
    Task<IEnumerable<Resource>> GetResourcesByCategoryAsync(string category);
    Task<IEnumerable<Resource>> GetActiveResourcesAsync();
    Task<List<Resource>> GetByTypeAndCategoryAsync(ResourceType resourceType, string category);

}

public interface IUserProgressRepository : IGenericRepository<UserProgress>
{
    Task<IEnumerable<UserProgress>> GetUserProgressByContentAsync(int userId, int contentId);
    Task<IEnumerable<UserProgress>> GetUserProgressByLearningPathAsync(int userId, int learningPathId);
    Task<double> GetLearningPathProgressPercentageAsync(int userId, int learningPathId);
}

public interface IAssessmentResultRepository : IGenericRepository<AssessmentResult>
{
    Task<IEnumerable<AssessmentResult>> GetUserAssessmentResultsAsync(int userId);
    Task<AssessmentResult?> GetAssessmentResultWithAnswersAsync(int assessmentResultId);
    Task<IEnumerable<AssessmentResult>> GetAssessmentResultsByAssessmentAsync(int assessmentId);
    Task<IEnumerable<AssessmentResult>> GetByUserIdAsync(int userId);
    Task<IEnumerable<AssessmentResult>> GetByUserIdAndAssessmentIdAsync(int userId, int assessmentId );
}

public interface IQuestionRepository : IGenericRepository<Question>
{
    Task<IEnumerable<Question>> GetByAssessmentIdAsync(int assessmentId);
    Task<Question?> GetQuestionWithOptionsAsync(int questionId);
}

public interface IUserAnswerRepository : IGenericRepository<UserAnswer>
{
    Task<IEnumerable<UserAnswer>> GetByAssessmentResultIdAsync(int assessmentResultId);
    Task<IEnumerable<UserAnswer>> GetByUserIdAndAssessmentIdAsync(int userId, int assessmentId);
}

public interface IUserLearningPathRepository : IGenericRepository<UserLearningPath>
{
    Task<UserLearningPath?> GetByUserAndLearningPathAsync(int userId, int learningPathId);
    Task<IEnumerable<UserLearningPath>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserLearningPath>> GetByLearningPathIdAsync(int learningPathId);
}

// New Repository Interfaces
public interface IFileUploadRepository : IGenericRepository<FileUpload>
{
    Task<List<FileUpload>> GetByUserIdAsync(int userId);
    Task<List<FileUpload>> GetPublicFilesAsync();
    Task<List<FileUpload>> GetSharedWithUserAsync(int userId);
}

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task<List<Notification>> GetUnreadByUserIdAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
}

public interface IAchievementRepository : IGenericRepository<Achievement>
{
    Task<List<Achievement>> GetActiveAchievementsAsync();
    Task<List<UserAchievement>> GetUserAchievementsAsync(int userId);
}

public interface IUserGoalRepository : IGenericRepository<UserGoal>
{
    Task<List<UserGoal>> GetByUserIdAsync(int userId);
    Task<List<UserGoal>> GetActiveGoalsAsync(int userId);
}

public interface IAuditLogRepository : IGenericRepository<AuditLog>
{
    Task<List<AuditLog>> GetByUserIdAsync(int userId);
    Task<List<AuditLog>> GetByEntityAsync(string entityType, int entityId);
}

public interface IUserActivityRepository : IGenericRepository<UserActivity>
{
    Task<List<UserActivity>> GetByUserIdAsync(int userId);
    Task<List<UserActivity>> GetRecentActivityAsync(int userId, int count);
}

public interface ISystemHealthRepository : IGenericRepository<SystemHealth>
{
    Task<List<SystemHealth>> GetLatestHealthChecksAsync();
    Task<SystemHealth?> GetByComponentAsync(string component);
}

public interface IPasswordResetTokenRepository : IGenericRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetValidTokenAsync(string token);
    Task<List<PasswordResetToken>> GetByUserIdAsync(int userId);
}

public interface IEmailVerificationTokenRepository : IGenericRepository<EmailVerificationToken>
{
    Task<EmailVerificationToken?> GetValidTokenAsync(string token);
    Task<List<EmailVerificationToken>> GetByUserIdAsync(int userId);
}

public interface IUserSessionRepository : IGenericRepository<UserSession>
{
    Task<List<UserSession>> GetActiveSessionsAsync(int userId);
    Task<UserSession?> GetBySessionIdAsync(string sessionId);
}
public interface IFileShareRepository : IGenericRepository<FileShare> 
{ 

}

