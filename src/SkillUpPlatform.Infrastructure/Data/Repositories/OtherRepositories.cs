using Microsoft.EntityFrameworkCore;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Infrastructure.Data;

namespace SkillUpPlatform.Infrastructure.Data.Repositories;

public class ResourceRepository : GenericRepository<Resource>, IResourceRepository
{
    public ResourceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Resource>> GetResourcesByTypeAsync(ResourceType resourceType)
    {
        return await _dbSet
            .Where(r => r.ResourceType == resourceType && r.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Resource>> GetResourcesByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(r => r.Category == category && r.IsActive)
            .ToListAsync();
    }

    public async Task<List<Resource>> GetByTypeAndCategoryAsync(ResourceType resourceType, string category)
    {
        return await _context.Resources
            .Where(r => r.ResourceType == resourceType && r.Category == category && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Resource>> GetActiveResourcesAsync()
    {
        return await _dbSet
            .Where(r => r.IsActive)
            .ToListAsync();
    }
}

public class UserProgressRepository : GenericRepository<UserProgress>, IUserProgressRepository
{
    public UserProgressRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserProgress>> GetUserProgressByContentAsync(int userId, int contentId)
    {
        return await _dbSet
            .Where(up => up.UserId == userId && up.ContentId == contentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserProgress>> GetUserProgressByLearningPathAsync(int userId, int learningPathId)
    {
        return await _dbSet
            .Include(up => up.Content)
            .Where(up => up.UserId == userId && up.Content.LearningPathId == learningPathId)
            .ToListAsync();
    }

    public async Task<double> GetLearningPathProgressPercentageAsync(int userId, int learningPathId)
    {
        var totalContent = await _context.Contents
            .CountAsync(c => c.LearningPathId == learningPathId);

        if (totalContent == 0) return 0;

        var completedContent = await _dbSet
            .Include(up => up.Content)
            .CountAsync(up => up.UserId == userId && 
                           up.Content.LearningPathId == learningPathId && 
                           up.IsCompleted);

        return (double)completedContent / totalContent * 100;
    }

    public async Task<int> CountAllAsync()
    {
        return await _context.UserProgresses.CountAsync();
    }

    public async Task<int> CountCompletedAsync()
    {
        return await _context.UserProgresses.CountAsync(p => p.IsCompleted);
    }

    public async Task<double?> GetAverageEngagementTimeAsync()
    {
        return await _context.UserProgresses
            .Where(p => p.TimeSpentMinutes > 0)
            .AverageAsync(p => (double?)p.TimeSpentMinutes);
    }

}

public class AssessmentResultRepository : GenericRepository<AssessmentResult>, IAssessmentResultRepository
{
    public AssessmentResultRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AssessmentResult>> GetUserAssessmentResultsAsync(int userId)
    {
        return await _dbSet
            .Include(ar => ar.Assessment)
            .Where(ar => ar.UserId == userId)
            .OrderByDescending(ar => ar.CompletedAt)
            .ToListAsync();
    }

    public async Task<AssessmentResult?> GetAssessmentResultWithAnswersAsync(int assessmentResultId)
    {        return await _dbSet
            .Include(ar => ar.UserAnswers)
                .ThenInclude(ua => ua.Question)
            .FirstOrDefaultAsync(ar => ar.Id == assessmentResultId);
    }

    public async Task<IEnumerable<AssessmentResult>> GetAssessmentResultsByAssessmentAsync(int assessmentId)
    {
        return await _dbSet
            .Include(ar => ar.User)
            .Where(ar => ar.AssessmentId == assessmentId)
            .OrderByDescending(ar => ar.CompletedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AssessmentResult>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(ar => ar.Assessment)
            .Where(ar => ar.UserId == userId)
            .OrderByDescending(ar => ar.CompletedAt)
            .ToListAsync();    }
}

public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
{
    public QuestionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Question>> GetByAssessmentIdAsync(int assessmentId)
    {
        return await _dbSet
            .Where(q => q.AssessmentId == assessmentId)
            .ToListAsync();
    }

    public async Task<Question?> GetQuestionWithOptionsAsync(int questionId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(q => q.Id == questionId);
    }
}

public class UserAnswerRepository : GenericRepository<UserAnswer>, IUserAnswerRepository
{
    public UserAnswerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserAnswer>> GetByAssessmentResultIdAsync(int assessmentResultId)
    {
        return await _dbSet
            .Include(ua => ua.Question)
            .Where(ua => ua.AssessmentResultId == assessmentResultId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAnswer>> GetByUserIdAndAssessmentIdAsync(int userId, int assessmentId)
    {
        return await _dbSet
            .Include(ua => ua.Question)
            .Where(ua => ua.UserId == userId && ua.Question.AssessmentId == assessmentId)
            .ToListAsync();
    }
}

public class UserLearningPathRepository : GenericRepository<UserLearningPath>, IUserLearningPathRepository
{
    public UserLearningPathRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserLearningPath?> GetByUserAndLearningPathAsync(int userId, int learningPathId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ulp => ulp.UserId == userId && ulp.LearningPathId == learningPathId);
    }

    public async Task<IEnumerable<UserLearningPath>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(ulp => ulp.LearningPath)
            .Where(ulp => ulp.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserLearningPath>> GetByLearningPathIdAsync(int learningPathId)
    {
        return await _dbSet
            .Include(ulp => ulp.User)
            .Where(ulp => ulp.LearningPathId == learningPathId)
            .ToListAsync();
    }
}
