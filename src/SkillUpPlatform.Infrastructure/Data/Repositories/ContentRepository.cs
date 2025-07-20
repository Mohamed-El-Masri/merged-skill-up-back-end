using Microsoft.EntityFrameworkCore;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Infrastructure.Data;

namespace SkillUpPlatform.Infrastructure.Data.Repositories;

public class ContentRepository : GenericRepository<Content>, IContentRepository
{
    public ContentRepository(ApplicationDbContext context) : base(context)
    {
    }
   
    public async Task<IEnumerable<Content>> GetContentByLearningPathAsync(int learningPathId)
    {
        return await _dbSet
            .Where(c => c.LearningPathId == learningPathId)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Content?> GetNextContentAsync(int currentContentId, int learningPathId)
    {
        var currentContent = await _dbSet.FindAsync(currentContentId);
        if (currentContent == null) return null;

        return await _dbSet
            .Where(c => c.LearningPathId == learningPathId && c.DisplayOrder > currentContent.DisplayOrder)
            .OrderBy(c => c.DisplayOrder)
            .FirstOrDefaultAsync();
    }

    public async Task<Content?> GetPreviousContentAsync(int currentContentId, int learningPathId)
    {
        var currentContent = await _dbSet.FindAsync(currentContentId);
        if (currentContent == null) return null;

        return await _dbSet
            .Where(c => c.LearningPathId == learningPathId && c.DisplayOrder < currentContent.DisplayOrder)
            .OrderByDescending(c => c.DisplayOrder)
            .FirstOrDefaultAsync();
    }
}

public class AssessmentRepository : GenericRepository<Assessment>, IAssessmentRepository
{
    public AssessmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Assessment?> GetAssessmentWithQuestionsAsync(int assessmentId)
    {
        return await _dbSet
            .Include(a => a.Questions.OrderBy(q => q.DisplayOrder))
            .FirstOrDefaultAsync(a => a.Id == assessmentId);
    }

    public async Task<IEnumerable<Assessment>> GetAssessmentsByTypeAsync(AssessmentType assessmentType)
    {
        return await _dbSet
            .Where(a => a.AssessmentType == assessmentType && a.IsActive)
            .ToListAsync();
    }    public async Task<IEnumerable<Assessment>> GetAssessmentsByLearningPathAsync(int learningPathId)
    {
        return await _dbSet
            .Where(a => a.LearningPathId == learningPathId && a.IsActive)
            .ToListAsync();
    }

    public async Task<Assessment?> GetByIdWithQuestionsAsync(int assessmentId)
    {
        return await GetAssessmentWithQuestionsAsync(assessmentId);
    }

    public async Task<IEnumerable<Assessment>> GetFilteredAssessmentsAsync(string? category, int? learningPathId)
    {
        var query = _dbSet.Where(a => a.IsActive);

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(a => a.Category == category);
        }

        if (learningPathId.HasValue)
        {
            query = query.Where(a => a.LearningPathId == learningPathId.Value);
        }

        return await query.ToListAsync();
    }
}
