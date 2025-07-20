using Microsoft.EntityFrameworkCore.Storage;
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
                        ISystemSettingsRepository systemSettings)
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
        SystemSettings = systemSettings;
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
    public ISystemSettingsRepository SystemSettings { get; }

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
