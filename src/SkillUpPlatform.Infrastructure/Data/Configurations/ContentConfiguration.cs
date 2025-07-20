using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Infrastructure.Data.Configurations;

public class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.VideoUrl)
            .HasMaxLength(500);

        builder.Property(c => c.DocumentUrl)
            .HasMaxLength(500);

        builder.Property(c => c.TextContent)
            .HasColumnType("nvarchar(max)");

        builder.HasMany(c => c.UserProgresses)
            .WithOne(up => up.Content)
            .HasForeignKey(up => up.ContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasMany(a => a.Questions)
            .WithOne(q => q.Assessment)
            .HasForeignKey(q => q.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.AssessmentResults)
            .WithOne(ar => ar.Assessment)
            .HasForeignKey(ar => ar.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.QuestionText)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(q => q.Options)
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.CorrectAnswer)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.Explanation)
            .HasMaxLength(2000);

        builder.HasMany(q => q.UserAnswers)
            .WithOne(ua => ua.Question)
            .HasForeignKey(ua => ua.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
