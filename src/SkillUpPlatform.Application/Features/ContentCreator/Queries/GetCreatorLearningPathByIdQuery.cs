using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Common.Models;

namespace SkillUpPlatform.Application.Features.ContentCreator.Queries;

public class GetCreatorLearningPathByIdQuery : IRequest<Result<CreatorLearningPathDto>>
{
    public int Id { get; set; }
} 