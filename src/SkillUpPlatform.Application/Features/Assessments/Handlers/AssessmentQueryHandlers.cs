using MediatR;
using AutoMapper;
using SkillUpPlatform.Application.Common.Constants;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Assessments.Queries;
using SkillUpPlatform.Application.Features.Assessments.DTOs;
using SkillUpPlatform.Domain.Interfaces;

namespace SkillUpPlatform.Application.Features.Assessments.Handlers;

public class GetAssessmentByIdQueryHandler : IRequestHandler<GetAssessmentByIdQuery, Result<AssessmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAssessmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AssessmentDto>> Handle(GetAssessmentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var assessment = await _unitOfWork.Assessments.GetByIdWithQuestionsAsync(request.AssessmentId);
            
            if (assessment == null)
            {
                return Result<AssessmentDto>.Failure(ErrorMessages.AssessmentNotFound);
            }

            var assessmentDto = _mapper.Map<AssessmentDto>(assessment);
            return Result<AssessmentDto>.Success(assessmentDto);
        }
        catch (Exception ex)
        {
            return Result<AssessmentDto>.Failure($"Failed to get assessment: {ex.Message}");
        }
    }
}

public class GetAssessmentsQueryHandler : IRequestHandler<GetAssessmentsQuery, Result<List<AssessmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAssessmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<AssessmentDto>>> Handle(GetAssessmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var assessments = await _unitOfWork.Assessments.GetFilteredAssessmentsAsync(
                request.Category, 
                request.LearningPathId);

            var assessmentDtos = _mapper.Map<List<AssessmentDto>>(assessments);
            return Result<List<AssessmentDto>>.Success(assessmentDtos);
        }
        catch (Exception ex)
        {
            return Result<List<AssessmentDto>>.Failure($"Failed to get assessments: {ex.Message}");
        }
    }
}

public class GetUserAssessmentResultsQueryHandler : IRequestHandler<GetUserAssessmentResultsQuery, Result<List<AssessmentResultDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserAssessmentResultsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<AssessmentResultDto>>> Handle(GetUserAssessmentResultsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var results = await _unitOfWork.AssessmentResults.GetByUserIdAsync(request.UserId);
            var resultDtos = _mapper.Map<List<AssessmentResultDto>>(results);
            
            return Result<List<AssessmentResultDto>>.Success(resultDtos);
        }
        catch (Exception ex)
        {
            return Result<List<AssessmentResultDto>>.Failure($"Failed to get user assessment results: {ex.Message}");
        }
    }
}
