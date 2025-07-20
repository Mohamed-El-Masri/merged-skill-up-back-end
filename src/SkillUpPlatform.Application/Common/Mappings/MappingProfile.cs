using AutoMapper;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Assessments.DTOs;
using SkillUpPlatform.Application.Features.Auth.Commands;
using SkillUpPlatform.Application.Features.Contentt.Commands;
using SkillUpPlatform.Application.Features.Contentt.DTOs;
using SkillUpPlatform.Application.Features.Dashboard.Queries;
using SkillUpPlatform.Application.Features.Files.Commands;
using SkillUpPlatform.Application.Features.LearningPaths.DTOs;
using SkillUpPlatform.Application.Features.Progress.Queries;
using SkillUpPlatform.Application.Features.Resources.Commands;
using SkillUpPlatform.Application.Features.Resources.DTOs;
using SkillUpPlatform.Application.Features.Users.DTOs;
using SkillUpPlatform.Domain.Entities;
using System.Text.Json;
using FileShare = SkillUpPlatform.Domain.Entities.FileShare;
using UserDto = SkillUpPlatform.Application.Features.Users.DTOs.UserDto;
using UserLearningPathDto = SkillUpPlatform.Application.Features.LearningPaths.DTOs.UserLearningPathDto;

namespace SkillUpPlatform.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<User, AuthResult>();

        // Learning Path mappings
        CreateMap<LearningPath, LearningPathDto>()
            .ForMember(dest => dest.Prerequisites, opt => opt.MapFrom(src => 
                DeserializeStringList(src.Prerequisites)))
            .ForMember(dest => dest.LearningObjectives, opt => opt.MapFrom(src => 
                DeserializeStringList(src.LearningObjectives)));

        CreateMap<LearningPath, LearningPathDetailDto>()
            .ForMember(dest => dest.Prerequisites, opt => opt.MapFrom(src => 
                DeserializeStringList(src.Prerequisites)))
            .ForMember(dest => dest.LearningObjectives, opt => opt.MapFrom(src => 
                DeserializeStringList(src.LearningObjectives)))
            .ForMember(dest => dest.Contents, opt => opt.MapFrom(src => src.Contents))
            .ForMember(dest => dest.Assessments, opt => opt.MapFrom(src => src.Assessments));

        CreateMap<UserLearningPath, UserLearningPathDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.LearningPath.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.LearningPath.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.LearningPath.ImageUrl));

        // Content mappings
        CreateMap<CreateContentCommand, Content>();
        CreateMap<Content, ContentDto>();
        CreateMap<Content, ContentDetailDto>()
            .ForMember(dest => dest.LearningPathTitle, opt => opt.MapFrom(src => src.LearningPath.Title));
        
        CreateMap<Content, ContentSummaryDto>();

        // Assessment mappings
        CreateMap<Assessment, AssessmentDto>();
        CreateMap<Assessment, AssessmentSummaryDto>();
          CreateMap<Question, QuestionDto>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => 
                DeserializeStringList(src.Options)));

        CreateMap<AssessmentResult, AssessmentResultDto>()
            .ForMember(dest => dest.AssessmentTitle, opt => opt.MapFrom(src => src.Assessment.Title));

        // Resource mappings
        CreateMap<Resource, ResourceDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                DeserializeStringList(src.Tags)));

        CreateMap<Resource, ResourceDetailDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                DeserializeStringList(src.Tags)));        // User Progress mappings
        CreateMap<UserProgress, UserProgressDto>(); 
        CreateMap<CreateResourceCommand, Resource>();

        //Dashboard
        // UserLearningPath to DashboardLearningPathProgressDto
        CreateMap<UserLearningPath, DashboardLearningPathProgressDto>()
            .ForMember(dest => dest.LearningPathId, opt => opt.MapFrom(src => src.LearningPathId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.LearningPath.Title))
            .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => src.ProgressPercentage))
            .ForMember(dest => dest.TimeSpent, opt => opt.Ignore());

        // UserAchievement to DashboardAchievementDto
        CreateMap<UserAchievement, DashboardAchievementDto>();
            /*.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AchievementId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Achievement.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Achievement.Description))
            .ForMember(dest => dest.BadgeUrl, opt => opt.MapFrom(src => src.Achievement.BadgeUrl))
            .ForMember(dest => dest.DateEarned, opt => opt.MapFrom(src => src.EarnedAt))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Achievement.Category));*/

        // LearningPath to PersonalizedRecommendationDto
        CreateMap<LearningPath, PersonalizedRecommendationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "LearningPath"))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.RecommendationScore, opt => opt.Ignore())
            .ForMember(dest => dest.Reason, opt => opt.Ignore());

        // Assessment to UpcomingDeadlineDto
        CreateMap<Assessment, UpcomingDeadlineDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Assessment"))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.DueDate, opt => opt.Ignore())
            .ForMember(dest => dest.DaysRemaining, opt => opt.Ignore())
            .ForMember(dest => dest.Priority, opt => opt.Ignore())
            .ForMember(dest => dest.IsOverdue, opt => opt.Ignore());

        // Assessment to CalendarEventDto
        CreateMap<Assessment, CalendarEventDto>();
        /*.ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
        .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
        .ForMember(dest => dest.Date, opt => opt.Ignore())
        .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Assessment"))
        .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Category switch
        {
            "Technical" => "blue",
            "Soft Skills" => "green",
            _ => "purple"
        }));*/

        // UserActivity to RecentActivityDto
        CreateMap<UserActivity, RecentActivityDto>();
            /*.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ActivityType))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ActivityType switch
            {
                "ContentCompleted" => "Content Completed",
                "AssessmentCompleted" => "Assessment Completed",
                _ => src.ActivityType
            }))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.ActivityType switch
            {
                "ContentCompleted" => "check-circle",
                "AssessmentCompleted" => "award",
                _ => "activity"
            }))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.ActivityType switch
            {
                "ContentCompleted" => "green",
                "AssessmentCompleted" => "blue",
                _ => "gray"
            }))
            .ForMember(dest => dest.ActionUrl, opt => opt.Ignore());*/

        // UserActivity to ActivityDto
        CreateMap<UserActivity, ActivityDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ActivityType))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.RelatedEntity, opt => opt.MapFrom(src => src.AdditionalData.ContainsKey("EntityType") ? src.AdditionalData["EntityType"].ToString() : null))
            .ForMember(dest => dest.RelatedEntityId, opt => opt.MapFrom(src => src.AdditionalData.ContainsKey("EntityId") ? Convert.ToInt32(src.AdditionalData["EntityId"]) : (int?)null));

        // UserProgress to DailyStatisticDto
        CreateMap<UserProgress, DailyStatisticDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CompletedAt!.Value.Date))
            .ForMember(dest => dest.TimeSpent, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.TimeSpentMinutes)))
            .ForMember(dest => dest.CompletedContents, opt => opt.MapFrom(src => src.IsCompleted ? 1 : 0))
            .ForMember(dest => dest.Sessions, opt => opt.MapFrom(src => 1));

        // UserAchievement to ProgressQueries_UserAchievementDto
        CreateMap<UserAchievement, ProgressQueries_UserAchievementDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AchievementId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Achievement.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Achievement.Description))
            .ForMember(dest => dest.EarnedAt, opt => opt.MapFrom(src => src.EarnedAt));

        //File
/*        CreateMap<FileUpload, FileUploadDto>();
        CreateMap<FileUpload, FileDetailsDto>();
        CreateMap<FileUpload, FileInfoDto>();*/

        // Map FileUpload to FileUploadDto
        CreateMap<FileUpload, FileUploadDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.OriginalFileName, opt => opt.MapFrom(src => src.OriginalFileName))
            .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
            .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileSize))
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath))
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.UploadedAt))
            .ForMember(dest => dest.UploadedBy, opt => opt.MapFrom(src => src.UploadedBy))
            .ForMember(dest => dest.SharedWith, opt => opt.MapFrom(src => src.FileShares));

        // Map FileShare to FileShareDto
        CreateMap<FileShare, FileShareDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())) // Generate new Guid for DTO
            .ForMember(dest => dest.FileId, opt => opt.MapFrom(src => src.FileUploadId))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileUpload.FileName))
            .ForMember(dest => dest.SharedWithUsers, opt => opt.MapFrom(src => new List<string> { src.SharedWithUser.Email }))
            .ForMember(dest => dest.Permission, opt => opt.MapFrom(src => src.AccessLevel))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.SharedAt))
            .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore()) // Not present in FileShare
            .ForMember(dest => dest.ShareUrl, opt => opt.Ignore()); // Can be set separately

        // Map UploadFileCommand to FileUpload
        CreateMap<UploadFileCommand, FileUpload>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.OriginalFileName, opt => opt.MapFrom(src => src.OriginalFileName))
            .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
            .ForMember(dest => dest.FileSize, opt => opt.Ignore()) // Set in handler after file processing
            .ForMember(dest => dest.FilePath, opt => opt.Ignore()) // Set in handler after file processing
            .ForMember(dest => dest.UploadedBy, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UploadedAt, opt => opt.Ignore()) // Set in handler
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.FileShares, opt => opt.Ignore());

        // Map FileUpload back to UploadFileCommand (if needed for updates)
        CreateMap<FileUpload, UploadFileCommand>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.OriginalFileName, opt => opt.MapFrom(src => src.OriginalFileName))
            .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
            .ForMember(dest => dest.FileContent, opt => opt.Ignore())
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.File, opt => opt.Ignore());

        // Map ShareFileCommand to FileShare
        CreateMap<ShareFileCommand, FileShare>()
            .ForMember(dest => dest.FileUploadId, opt => opt.MapFrom(src => src.FileId))
            .ForMember(dest => dest.SharedWithUserId, opt => opt.MapFrom(src => src.SharedWithUserId))
            .ForMember(dest => dest.SharedBy, opt => opt.MapFrom(src => src.SharedByUserId))
            .ForMember(dest => dest.SharedAt, opt => opt.Ignore()) // Set separately in handler
            .ForMember(dest => dest.AccessLevel, opt => opt.MapFrom(src => src.AccessLevel))
            .ForMember(dest => dest.FileUpload, opt => opt.Ignore())
            .ForMember(dest => dest.SharedWithUser, opt => opt.Ignore())
            .ForMember(dest => dest.SharedByUser, opt => opt.Ignore());




    }

    private static List<string> DeserializeStringList(string? jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(jsonString) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}
