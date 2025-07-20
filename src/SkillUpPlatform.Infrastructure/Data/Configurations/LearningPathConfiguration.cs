using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(up => up.Id);

        builder.Property(up => up.Bio)
            .HasMaxLength(2000);

        builder.Property(up => up.LinkedInUrl)
            .HasMaxLength(500);

        builder.Property(up => up.GitHubUrl)
            .HasMaxLength(500);

        builder.Property(up => up.PortfolioUrl)
            .HasMaxLength(500);

        builder.Property(up => up.ProfilePictureUrl)
            .HasMaxLength(500);

        builder.Property(up => up.Skills)
            .HasColumnType("nvarchar(max)");

        builder.Property(up => up.Interests)
            .HasColumnType("nvarchar(max)");

        builder.Property(up => up.Certifications)
            .HasColumnType("nvarchar(max)");
    }
}

public class LearningPathConfiguration : IEntityTypeConfiguration<LearningPath>
{
    public void Configure(EntityTypeBuilder<LearningPath> builder)
    {
        builder.HasKey(lp => lp.Id);

        builder.Property(lp => lp.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(lp => lp.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(lp => lp.ImageUrl)
            .HasMaxLength(500);

        builder.Property(lp => lp.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(lp => lp.Prerequisites)
            .HasColumnType("nvarchar(max)");

        builder.Property(lp => lp.LearningObjectives)
            .HasColumnType("nvarchar(max)");

        builder.HasMany(lp => lp.Contents)
            .WithOne(c => c.LearningPath)
            .HasForeignKey(c => c.LearningPathId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(lp => lp.UserLearningPaths)
            .WithOne(ulp => ulp.LearningPath)
            .HasForeignKey(ulp => ulp.LearningPathId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(lp => lp.Assessments)
            .WithOne(a => a.LearningPath)
            .HasForeignKey(a => a.LearningPathId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
