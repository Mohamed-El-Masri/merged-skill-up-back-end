using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SkillUpPlatform.Infrastructure.Data.Repositories;

public class ErrorLogRepository : GenericRepository<ErrorLogRepository>, IErrorLogRepository
{
    private readonly ApplicationDbContext _context;

    public ErrorLogRepository(ApplicationDbContext context):base(context)
    {
        _context = context;
    }

    public async Task<ErrorLog?> GetByIdAsync(int id)
    {
        return await _context.ErrorLogs.FindAsync(id);
    }

    public async Task<List<ErrorLog>> GetAllAsync()
    {
        return await _context.ErrorLogs
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();
    }

    public async Task<List<ErrorLog>> GetBySeverityAsync(string severity)
    {
        return await _context.ErrorLogs
            .Where(e => e.Severity == severity)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();
    }

    public async Task<List<ErrorLog>> GetByDateRangeAsync(DateTime start, DateTime end)
    {
        return await _context.ErrorLogs
            .Where(e => e.Timestamp >= start && e.Timestamp <= end)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();
    }

    public async Task AddAsync(ErrorLog entity)
    {
        await _context.ErrorLogs.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<ErrorLog> entities)
    {
        await _context.ErrorLogs.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.ErrorLogs.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<ErrorLog>> FindAsync(Expression<Func<ErrorLog, bool>> expression)
    {
        return await _context.ErrorLogs.Where(expression).ToListAsync();
    }

    public async Task<ErrorLog?> SingleOrDefaultAsync(Expression<Func<ErrorLog, bool>> expression)
    {
        return await _context.ErrorLogs.SingleOrDefaultAsync(expression);
    }

    public void Remove(ErrorLog entity)
    {
        _context.ErrorLogs.Remove(entity);
        _context.SaveChanges();
    }

    public void RemoveRange(IEnumerable<ErrorLog> entities)
    {
        _context.ErrorLogs.RemoveRange(entities);
        _context.SaveChanges();
    }

    public void Update(ErrorLog entity)
    {
        _context.ErrorLogs.Update(entity);
        _context.SaveChanges();
    }

    public void UpdateRange(IEnumerable<ErrorLog> entities)
    {
        _context.ErrorLogs.UpdateRange(entities);
        _context.SaveChanges();
    }

    async Task<IEnumerable<ErrorLog>> IGenericRepository<ErrorLog>.GetAllAsync()
    {
        return await _context.ErrorLogs.ToListAsync();
    }
}
