using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Infrastructure.Data.Configurations;

public class AssessmentResultConfiguration : IEntityTypeConfiguration<AssessmentResult>
{
    public void Configure(EntityTypeBuilder<AssessmentResult> builder)
    {
        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.Feedback)
            .HasMaxLength(2000);

        builder.HasMany(ar => ar.UserAnswers)
            .WithOne(ua => ua.AssessmentResult)
            .HasForeignKey(ua => ua.AssessmentResultId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
{
    public void Configure(EntityTypeBuilder<UserAnswer> builder)
    {
        builder.HasKey(ua => ua.Id);

        builder.Property(ua => ua.UserAnswerText)
            .IsRequired()
            .HasMaxLength(2000);
    }
}

public class UserLearningPathConfiguration : IEntityTypeConfiguration<UserLearningPath>
{
    public void Configure(EntityTypeBuilder<UserLearningPath> builder)
    {
        builder.HasKey(ulp => ulp.Id);

        builder.HasIndex(ulp => new { ulp.UserId, ulp.LearningPathId })
            .IsUnique();
    }
}

public class UserProgressConfiguration : IEntityTypeConfiguration<UserProgress>
{
    public void Configure(EntityTypeBuilder<UserProgress> builder)
    {
        builder.HasKey(up => up.Id);

        builder.HasIndex(up => new { up.UserId, up.ContentId })
            .IsUnique();
    }
}

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(r => r.FileUrl)
            .HasMaxLength(500);

        builder.Property(r => r.TemplateContent)
            .HasColumnType("nvarchar(max)");

        builder.Property(r => r.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Tags)
            .HasColumnType("nvarchar(max)");
    }
}
