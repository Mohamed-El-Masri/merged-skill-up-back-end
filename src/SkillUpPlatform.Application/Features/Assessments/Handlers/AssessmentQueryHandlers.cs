using MediatR;
using AutoMapper;
using SkillUpPlatform.Application.Common.Constants;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Assessments.Queries;
using SkillUpPlatform.Application.Features.Assessments.DTOs;
using SkillUpPlatform.Domain.Interfaces;
using SkillUpPlatform.Domain.Entities;

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

public class GetAssessmentResultsQueryHandler : IRequestHandler<GetAssessmentResultsQuery, Result<List<AssessmentResultDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAssessmentResultsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Result<List<AssessmentResultDto>>> Handle(GetAssessmentResultsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            bool assessmentExist = await _unitOfWork.Assessments
                .ExistsAsync(request.AssessmentId);

            if (!assessmentExist)
            {
                return Result<List<AssessmentResultDto>>.Failure(ErrorMessages.AssessmentNotFound);
            }

            IEnumerable<AssessmentResult> results;
            if (request.UserId.HasValue)
            {
                results = await _unitOfWork.AssessmentResults
                   .GetByUserIdAndAssessmentIdAsync(request.UserId.Value, request.AssessmentId);
            }
            else
            {
                results = await _unitOfWork.AssessmentResults
                    .GetAssessmentResultsByAssessmentAsync(request.AssessmentId);
            }

            List<AssessmentResultDto> resultDtos = _mapper.Map<List<AssessmentResultDto>>(results);

            Assessment assessment = await _unitOfWork.Assessments.GetByIdAsync(request.AssessmentId);
            foreach (AssessmentResultDto dto in resultDtos)
            {
                dto.AssessmentTitle = assessment.Title;
            }

            return Result<List<AssessmentResultDto>>.Success(resultDtos);
        }
        catch (Exception ex)
        {
            return Result<List<AssessmentResultDto>>.Failure(ex.Message);
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


public class GetAssessmentQuestionsQueryHandler : IRequestHandler<GetAssessmentQuestionsQuery, Result<List<QuestionDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAssessmentQuestionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<QuestionDto>>> Handle(GetAssessmentQuestionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate assessment exists
            var assessmentExists = await _unitOfWork.Assessments.ExistsAsync(request.AssessmentId);
            if (!assessmentExists)
            {
                return Result<List<QuestionDto>>.Failure(ErrorMessages.AssessmentNotFound);
            }

            // Get questions for the assessment
            var questions = await _unitOfWork.Questions.GetByAssessmentIdAsync(request.AssessmentId);

            // Map questions to DTOs
            var questionDtos = _mapper.Map<List<QuestionDto>>(questions);

            return Result<List<QuestionDto>>.Success(questionDtos);
        }
        catch (Exception ex)
        {
            return Result<List<QuestionDto>>.Failure($"Failed to get assessment questions: {ex.Message}");
        }
    }
}

public class GetUserAssessmentsQueryHandler : IRequestHandler<GetUserAssessmentsQuery, Result<List<AssessmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserAssessmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<AssessmentDto>>> Handle(GetUserAssessmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            bool userExists = await _unitOfWork.Users.ExistsAsync(request.UserId);
            if (!userExists)
            {
                return Result<List<AssessmentDto>>.Failure(ErrorMessages.UserNotFound);
            }

            var assessments = await _unitOfWork.Assessments.GetFilteredAssessmentsAsync(
                category: null, 
                learningPathId: null 
            );

            // ????? ????????? ????? ??? ??????
            var activeAssessments = assessments.Where(a => a.IsActive).ToList();

            // ????? ????????? ??? DTO
            var assessmentDtos = _mapper.Map<List<AssessmentDto>>(activeAssessments);

            return Result<List<AssessmentDto>>.Success(assessmentDtos);
        }
        catch (Exception ex)
        {
            return Result<List<AssessmentDto>>.Failure($"Failed to get user assessments: {ex.Message}");
        }
    }
}
