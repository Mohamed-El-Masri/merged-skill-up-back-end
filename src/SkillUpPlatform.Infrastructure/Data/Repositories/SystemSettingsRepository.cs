using Microsoft.EntityFrameworkCore;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Infrastructure.Data;

namespace SkillUpPlatform.Infrastructure.Data.Repositories;

public class SystemSettingsRepository : GenericRepository<SystemSettings>, ISystemSettingsRepository
{
    public SystemSettingsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SystemSettings?> GetByKeyAsync(string key)
    {
        return await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
    }

    public async Task<List<SystemSettings>> GetByCategoryAsync(string category)
    {
        return await _context.SystemSettings.Where(s => s.Category == category).ToListAsync();
    }
} 