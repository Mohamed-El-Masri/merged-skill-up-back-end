using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Infrastructure.Data.Configurations;
using System.Text.Json;

namespace SkillUpPlatform.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<LearningPath> LearningPaths { get; set; }
    public DbSet<Content> Contents { get; set; }
    public DbSet<Assessment> Assessments { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<AssessmentResult> AssessmentResults { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<UserLearningPath> UserLearningPaths { get; set; }
    public DbSet<UserProgress> UserProgresses { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<FileUpload> FileUploads { get; set; }
    public DbSet<Domain.Entities.FileShare> FileShares { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationSettings> NotificationSettings { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }
    public DbSet<SystemHealth> SystemHealths { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<SystemSettings> SystemSettings { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }


    private static readonly ValueComparer<Dictionary<string, object>> DictionaryComparer =
    new ValueComparer<Dictionary<string, object>>(
        (d1, d2) => d1.SequenceEqual(d2),
        d => d.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        d => d.ToDictionary(entry => entry.Key, entry => entry.Value));


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        


        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.TotalAmount)
                  .HasPrecision(18, 2); // 18 digits total, 2 after decimal point
        });


        modelBuilder.Entity<SystemHealth>(entity =>
        {
            entity.Property(e => e.AdditionalInfo)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(DictionaryComparer);
        });


        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.Property(e => e.AdditionalData)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(DictionaryComparer);
        });


    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
