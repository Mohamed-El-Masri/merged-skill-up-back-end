using AutoMapper;
using MediatR;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Resources.Commands;
using SkillUpPlatform.Application.Features.Resources.DTOs;
using SkillUpPlatform.Application.Features.Resources.Queries;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;

namespace SkillUpPlatform.Application.Features.Resources.Handlers;

public class ResourceHandlers :
    IRequestHandler<GetCVTemplatesQuery, Result<List<ResourceDto>>>,
    IRequestHandler<GetCoverLetterTemplatesQuery, Result<List<ResourceDto>>>,
    IRequestHandler<GetInterviewQuestionsQuery, Result<List<ResourceDto>>>,
    IRequestHandler<GetInterviewQuestionsByCategoryQuery, Result<List<ResourceDto>>>,
    IRequestHandler<CreateResourceCommand, Result<int>>,
    IRequestHandler<UpdateResourceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ResourceHandlers(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<ResourceDto>>> Handle(GetCVTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _unitOfWork.Resources.GetResourcesByTypeAsync(resourceType: ResourceType.CVTemplate);
        var mapped = _mapper.Map<List<ResourceDto>>(templates);
        return Result<List<ResourceDto>>.Success(mapped);
    }

    public async Task<Result<List<ResourceDto>>> Handle(GetCoverLetterTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _unitOfWork.Resources.GetResourcesByTypeAsync(resourceType: ResourceType.CoverLetterTemplate);
        var mapped = _mapper.Map<List<ResourceDto>>(templates);
        return Result<List<ResourceDto>>.Success(mapped);
    }

    public async Task<Result<List<ResourceDto>>> Handle(GetInterviewQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await _unitOfWork.Resources.GetResourcesByTypeAsync(resourceType: ResourceType.InterviewQuestions);
        var mapped = _mapper.Map<List<ResourceDto>>(questions);
        return Result<List<ResourceDto>>.Success(mapped);
    }

    public async Task<Result<List<ResourceDto>>> Handle(GetInterviewQuestionsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var questions = await _unitOfWork.Resources.GetByTypeAndCategoryAsync(resourceType: ResourceType.InterviewQuestions, request.Category);
        var mapped = _mapper.Map<List<ResourceDto>>(questions);
        return Result<List<ResourceDto>>.Success(mapped);
    }

    public async Task<Result<int>> Handle(CreateResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = _mapper.Map<Resource>(request);
        await _unitOfWork.Resources.AddAsync(resource);
        await _unitOfWork.SaveChangesAsync();

        return Result<int>.Success(resource.Id);
    }

    public async Task<Result> Handle(UpdateResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await _unitOfWork.Resources.GetByIdAsync(request.Id);
        if (resource == null)
            return Result.Failure("Resource not found");

        resource.Title = request.Title;
        resource.Description = request.Description;
        //resource.ResourceType = request.;
        resource.FileUrl = request.FileUrl;
        resource.Category = request.Category;

        _unitOfWork.Resources.Update(resource);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
