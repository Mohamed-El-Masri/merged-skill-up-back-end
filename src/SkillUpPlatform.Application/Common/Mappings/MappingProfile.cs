using AutoMapper;
using SkillUpPlatform.Application.Features.Admin.Queries;
using SkillUpPlatform.Application.Features.Assessments.DTOs;
using SkillUpPlatform.Application.Features.Contentt.DTOs;
using SkillUpPlatform.Application.Features.LearningPaths.DTOs;
using SkillUpPlatform.Application.Features.Resources.Commands;
using SkillUpPlatform.Application.Features.Resources.DTOs;
using SkillUpPlatform.Application.Features.Users.DTOs;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Domain.Entities;
using System.Text.Json;

namespace SkillUpPlatform.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<User, AdminUserDto>()
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLoginAt ?? src.CreatedAt));
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

        CreateMap<UserLearningPath, Common.Models.UserLearningPathDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.LearningPath.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.LearningPath.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.LearningPath.ImageUrl));

        // Content mappings
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
        //CreateMap<ErrorLog, ErrorLogDto>();
        CreateMap<ErrorLog, ErrorLogDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())) // ���� int �� �����
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Severity))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
            .ForMember(dest => dest.Exception, opt => opt.MapFrom(src => src.StackTrace ?? string.Empty))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.Source, opt => opt.MapFrom(src => "SkillUpPlatform")) // ���� ����� �� ������ ����
            .ForMember(dest => dest.UserId, opt => opt.Ignore()); // �� �� ����� ����

        CreateMap<AuditLog, AuditLogDto>();
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
