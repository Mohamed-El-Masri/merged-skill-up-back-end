using MediatR;
using AutoMapper;
using SkillUpPlatform.Application.Common.Constants;
using SkillUpPlatform.Application.Common.Models;
using SkillUpPlatform.Application.Features.Assessments.Commands;
using SkillUpPlatform.Application.Interfaces;
using SkillUpPlatform.Domain.Entities;
using SkillUpPlatform.Domain.Interfaces;

namespace SkillUpPlatform.Application.Features.Assessments.Handlers;

public class CreateAssessmentCommandHandler : IRequestHandler<CreateAssessmentCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateAssessmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(CreateAssessmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate learning path exists if specified
            if (request.LearningPathId.HasValue)
            {
                var learningPathExists = await _unitOfWork.LearningPaths.ExistsAsync(request.LearningPathId.Value);
                if (!learningPathExists)
                {
                    return Result<int>.Failure(ErrorMessages.LearningPathNotFound);
                }
            }

            // Create assessment
            var assessment = new Assessment
            {
                Title = request.Title,
                Description = request.Description,
                AssessmentType = request.AssessmentType,
                TimeLimit = request.TimeLimit,
                PassingScore = request.PassingScore,
                LearningPathId = request.LearningPathId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Assessments.AddAsync(assessment);
            await _unitOfWork.SaveChangesAsync();

            // Create questions
            foreach (var questionDto in request.Questions)
            {                var question = new Question
                {
                    AssessmentId = assessment.Id,
                    QuestionText = questionDto.QuestionText,
                    QuestionType = questionDto.QuestionType,
                    Options = System.Text.Json.JsonSerializer.Serialize(questionDto.Options),
                    CorrectAnswer = questionDto.CorrectAnswer,
                    Explanation = questionDto.Explanation,
                    Points = questionDto.Points
                };

                await _unitOfWork.Questions.AddAsync(question);
            }

            await _unitOfWork.SaveChangesAsync();

            return Result<int>.Success(assessment.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Failed to create assessment: {ex.Message}");
        }
    }
}

public class SubmitAssessmentCommandHandler : IRequestHandler<SubmitAssessmentCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAIService _aiService;

    public SubmitAssessmentCommandHandler(IUnitOfWork unitOfWork, IAIService aiService)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
    }

    public async Task<Result<int>> Handle(SubmitAssessmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate assessment exists
            var assessment = await _unitOfWork.Assessments.GetByIdAsync(request.AssessmentId);
            if (assessment == null)
            {
                return Result<int>.Failure(ErrorMessages.AssessmentNotFound);
            }

            // Validate user exists
            var userExists = await _unitOfWork.Users.ExistsAsync(request.UserId);
            if (!userExists)
            {
                return Result<int>.Failure(ErrorMessages.UserNotFound);
            }

            // Get questions for scoring
            var questions = await _unitOfWork.Questions.GetByAssessmentIdAsync(request.AssessmentId);
            
            // Calculate score
            int totalScore = 0;
            int maxScore = questions.Sum(q => q.Points);

            // Create assessment result
            var assessmentResult = new AssessmentResult
            {
                UserId = request.UserId,
                AssessmentId = request.AssessmentId,
                Score = 0, // Will be calculated below
                MaxScore = maxScore,
                TimeSpentMinutes = request.TimeSpentMinutes,
                CompletedAt = DateTime.UtcNow,
                IsPassed = false // Will be determined below
            };

            await _unitOfWork.AssessmentResults.AddAsync(assessmentResult);
            await _unitOfWork.SaveChangesAsync();

            // Process answers and calculate score
            foreach (var answerDto in request.Answers)
            {
                var question = questions.FirstOrDefault(q => q.Id == answerDto.QuestionId);
                if (question == null) continue;

                bool isCorrect = string.Equals(question.CorrectAnswer?.Trim(), answerDto.Answer?.Trim(), StringComparison.OrdinalIgnoreCase);
                
                if (isCorrect)
                {
                    totalScore += question.Points;
                }

                var userAnswer = new UserAnswer
                {
                    UserId = request.UserId,
                    QuestionId = answerDto.QuestionId,
                    AssessmentResultId = assessmentResult.Id,
                    Answer = answerDto.Answer,
                    IsCorrect = isCorrect
                };

                await _unitOfWork.UserAnswers.AddAsync(userAnswer);
            }

            // Update assessment result with final score
            assessmentResult.Score = totalScore;
            assessmentResult.IsPassed = totalScore >= assessment.PassingScore;

            await _unitOfWork.SaveChangesAsync();

            // Generate AI feedback (async, don't wait)
            _ = Task.Run(async () =>
            {
                try
                {
                    var feedback = await _aiService.GenerateAssessmentFeedbackAsync(
                        assessment.Title, 
                        totalScore, 
                        maxScore, 
                        assessmentResult.IsPassed);
                    
                    assessmentResult.AIFeedback = feedback;
                    await _unitOfWork.SaveChangesAsync();
                }
                catch
                {
                    // Log error but don't fail the main operation
                }
            });

            return Result<int>.Success(assessmentResult.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Failed to submit assessment: {ex.Message}");
        }
    }
}
