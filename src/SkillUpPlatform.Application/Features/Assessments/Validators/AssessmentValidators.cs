using FluentValidation;
using SkillUpPlatform.Application.Features.Assessments.Commands;
using SkillUpPlatform.Domain.Entities;

namespace SkillUpPlatform.Application.Features.Assessments.Validators;

public class CreateAssessmentCommandValidator : AbstractValidator<CreateAssessmentCommand>
{
    public CreateAssessmentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Assessment title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Assessment description is required")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.AssessmentType)
            .IsInEnum().WithMessage("Invalid assessment type");

        RuleFor(x => x.TimeLimit)
            .GreaterThan(0).WithMessage("Time limit must be greater than 0")
            .LessThanOrEqualTo(240).WithMessage("Time limit cannot exceed 240 minutes");

        RuleFor(x => x.PassingScore)
            .GreaterThan(0).WithMessage("Passing score must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Passing score cannot exceed 100");

        RuleFor(x => x.Questions)
            .NotEmpty().WithMessage("Assessment must have at least one question")
            .Must(questions => questions.Count <= 50).WithMessage("Assessment cannot have more than 50 questions");

        RuleForEach(x => x.Questions).SetValidator(new CreateQuestionDtoValidator());
    }
}

public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionDtoValidator()
    {
        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required")
            .MaximumLength(500).WithMessage("Question text cannot exceed 500 characters");

        RuleFor(x => x.QuestionType)
            .IsInEnum().WithMessage("Invalid question type");

        RuleFor(x => x.CorrectAnswer)
            .NotEmpty().WithMessage("Correct answer is required");

        RuleFor(x => x.Points)
            .GreaterThan(0).WithMessage("Points must be greater than 0")
            .LessThanOrEqualTo(10).WithMessage("Points cannot exceed 10");

        When(x => x.QuestionType == QuestionType.MultipleChoice, () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage("Multiple choice questions must have options")
                .Must(options => options.Count >= 2).WithMessage("Multiple choice questions must have at least 2 options")
                .Must(options => options.Count <= 6).WithMessage("Multiple choice questions cannot have more than 6 options");
        });
    }
}

public class SubmitAssessmentCommandValidator : AbstractValidator<SubmitAssessmentCommand>
{
    public SubmitAssessmentCommandValidator()
    {
        RuleFor(x => x.AssessmentId)
            .GreaterThan(0).WithMessage("Valid assessment ID is required");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Valid user ID is required");

        RuleFor(x => x.TimeSpentMinutes)
            .GreaterThan(0).WithMessage("Time spent must be greater than 0")
            .LessThanOrEqualTo(240).WithMessage("Time spent cannot exceed 240 minutes");

        RuleFor(x => x.Answers)
            .NotEmpty().WithMessage("Assessment must have at least one answer");

        RuleForEach(x => x.Answers).SetValidator(new SubmitAnswerDtoValidator());
    }
}

public class SubmitAnswerDtoValidator : AbstractValidator<SubmitAnswerDto>
{
    public SubmitAnswerDtoValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0).WithMessage("Valid question ID is required");

        RuleFor(x => x.Answer)
            .NotEmpty().WithMessage("Answer is required");
    }
}
