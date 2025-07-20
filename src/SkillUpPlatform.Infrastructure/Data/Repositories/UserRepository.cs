using Microsoft.EntityFrameworkCore;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Infrastructure.Data;

namespace SkillUpPlatform.Infrastructure.Data.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserWithProfileAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
    {
        return await _dbSet.Where(u => u.Role == role).ToListAsync();
    }
}

public class LearningPathRepository : GenericRepository<LearningPath>, ILearningPathRepository
{
    public LearningPathRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LearningPath>> GetActiveLearningPathsAsync()
    {
        return await _dbSet.Where(lp => lp.IsActive).ToListAsync();
    }

    public async Task<LearningPath?> GetLearningPathWithContentsAsync(int learningPathId)
    {
        return await _dbSet
            .Include(lp => lp.Contents.OrderBy(c => c.DisplayOrder))
            .FirstOrDefaultAsync(lp => lp.Id == learningPathId);
    }

    public async Task<IEnumerable<LearningPath>> GetLearningPathsByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(lp => lp.Category == category && lp.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<LearningPath>> GetRecommendedLearningPathsAsync(int userId)
    {
        // This would typically involve AI/ML logic
        // For now, return active learning paths
        return await GetActiveLearningPathsAsync();
    }
}
