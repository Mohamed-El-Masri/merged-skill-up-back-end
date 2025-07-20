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
    ISystemSettingsRepository SystemSettings { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
