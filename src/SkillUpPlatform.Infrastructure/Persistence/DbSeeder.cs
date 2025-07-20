using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillUpPlatform.Infrastructure.Persistence
{
    public static class DbSeeder
    {

        //public static async Task SeedAsync(ApplicationDbContext context)
        //{
        //    if (!await context.Users.AnyAsync())
        //    {
        //        var passwordHasher = new PasswordHasher<User>();

        //        var admin = new User
        //        {
        //            Email = "admin@skillup.com",
        //            FirstName = "SAdmin",
        //            Role = UserRole.Admin,
        //            IsEmailVerified= true
        //        };

        //        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin@123"); // اختاري باسورد قوي

        //        context.Users.Add(admin);
        //        await context.SaveChangesAsync();
        //    }

        //}


        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var passwordHasher = new PasswordHasher<User>();

            // Check if Admin exists
            if (!await context.Users.AnyAsync(u => u.Email == "admin@skillup.com"))
            {
                var admin = new User
                {
                    Email = "admin@skillup.com",
                    FirstName = "SAdmin",
                    Role = UserRole.Admin,
                    IsEmailVerified = true
                };
                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

                context.Users.Add(admin);
            }

            // Check if Content Creator exists
            if (!await context.Users.AnyAsync(u => u.Email == "creator@skillup.com"))
            {
                var contentCreator = new User
                {
                    Email = "creator@skillup.com",
                    FirstName = "CreatorUser",
                    Role = UserRole.ContentCreator,
                    IsEmailVerified = true
                };
                contentCreator.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Creator@123");

                context.Users.Add(contentCreator);
            }

            // Check if Student exists
            if (!await context.Users.AnyAsync(u => u.Email == "student@skillup.com"))
            {
                var student = new User
                {
                    Email = "student@skillup.com",
                    FirstName = "StudentUser",
                    Role = UserRole.Student,
                    IsEmailVerified = true
                };
                student.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123");

                context.Users.Add(student);
            }

            await context.SaveChangesAsync();

            // Get the content creator user for learning paths
            var creatorUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "creator@skillup.com");

            // Seed sample audit logs if none exist
            if (!await context.AuditLogs.AnyAsync())
            {
                var sampleAuditLogs = new List<AuditLog>
                {
                    new AuditLog
                    {
                        UserId = 1, // Assuming admin user ID is 1
                        Action = "Login",
                        EntityType = "User",
                        EntityId = 1,
                        OldValues = null,
                        NewValues = null,
                        IpAddress = "192.168.1.1",
                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                        Timestamp = DateTime.UtcNow.AddHours(-2)
                    },
                    new AuditLog
                    {
                        UserId = 1,
                        Action = "Update",
                        EntityType = "User",
                        EntityId = 2,
                        OldValues = "{\"FirstName\":\"Old Name\"}",
                        NewValues = "{\"FirstName\":\"New Name\"}",
                        IpAddress = "192.168.1.1",
                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                        Timestamp = DateTime.UtcNow.AddHours(-1)
                    },
                    new AuditLog
                    {
                        UserId = creatorUser?.Id ?? 2, // Use actual creator user ID
                        Action = "Create",
                        EntityType = "LearningPath",
                        EntityId = 1,
                        OldValues = null,
                        NewValues = "{\"Title\":\"Sample Learning Path\",\"Description\":\"A sample learning path\"}",
                        IpAddress = "192.168.1.2",
                        UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
                        Timestamp = DateTime.UtcNow.AddMinutes(-30)
                    }
                };

                context.AuditLogs.AddRange(sampleAuditLogs);
                await context.SaveChangesAsync();
            }

            // Seed sample learning paths if none exist
            if (!await context.LearningPaths.AnyAsync())
            {
                if (creatorUser != null)
                {
                    var sampleLearningPaths = new List<LearningPath>
                    {
                        new LearningPath
                        {
                            Title = "Introduction to Web Development",
                            Description = "Learn the basics of HTML, CSS, and JavaScript",
                            Category = "Web Development",
                            DifficultyLevel = DifficultyLevel.Beginner,
                            EstimatedDuration = 40,
                            Price = 49.99m,
                            IsPublished = true,
                            Tags = "web,html,css,javascript",
                            Prerequisites = "Basic computer skills",
                            LearningObjectives = "Build responsive websites,Understand modern web technologies",
                            IsActive = true,
                            CreatorId = creatorUser.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-5)
                        },
                        new LearningPath
                        {
                            Title = "Advanced JavaScript Programming",
                            Description = "Master JavaScript ES6+ features and modern development practices",
                            Category = "Programming",
                            DifficultyLevel = DifficultyLevel.Intermediate,
                            EstimatedDuration = 60,
                            Price = 79.99m,
                            IsPublished = true,
                            Tags = "javascript,es6,programming",
                            Prerequisites = "Basic JavaScript knowledge",
                            LearningObjectives = "Master ES6+ features,Understand async programming",
                            IsActive = true,
                            CreatorId = creatorUser.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-3)
                        },
                        new LearningPath
                        {
                            Title = "React.js Fundamentals",
                            Description = "Learn React.js from scratch to build modern web applications",
                            Category = "Frontend Development",
                            DifficultyLevel = DifficultyLevel.Intermediate,
                            EstimatedDuration = 50,
                            Price = 69.99m,
                            IsPublished = true,
                            Tags = "react,frontend,javascript",
                            Prerequisites = "JavaScript fundamentals",
                            LearningObjectives = "Build React applications,Understand component lifecycle",
                            IsActive = true,
                            CreatorId = creatorUser.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-1)
                        }
                    };

                    context.LearningPaths.AddRange(sampleLearningPaths);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}



