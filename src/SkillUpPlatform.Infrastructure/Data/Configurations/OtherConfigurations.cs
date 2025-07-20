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

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FileShareConfiguration : IEntityTypeConfiguration<SkillUpPlatform.Domain.Entities.FileShare>
{
    public void Configure(EntityTypeBuilder<SkillUpPlatform.Domain.Entities.FileShare> builder)
    {
        builder.HasKey(fs => fs.Id);

        builder.HasOne(fs => fs.FileUpload)
            .WithMany(fu => fu.FileShares)
            .HasForeignKey(fs => fs.FileUploadId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(fs => fs.SharedWithUser)
            .WithMany()
            .HasForeignKey(fs => fs.SharedWithUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(fs => fs.SharedByUser)
            .WithMany()
            .HasForeignKey(fs => fs.SharedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(fs => fs.AccessLevel)
            .HasMaxLength(50)
            .HasDefaultValue("Read");
    }
}
