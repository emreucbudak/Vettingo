using FluentValidation;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam;

public sealed class CreateExamCommandRequestValidator : AbstractValidator<CreateExamCommandRequest>
{
    public CreateExamCommandRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().When(x => x.CompanyId.HasValue);
        RuleFor(x => x.OwnerType).IsInEnum();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.DurationMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PassingScore).InclusiveBetween(0, 100);
        RuleFor(x => x)
            .Must(x => x.MultipleChoiceQuestions is { Count: > 0 } || x.TrueFalseQuestions is { Count: > 0 })
            .WithMessage("Sınav en az bir çoktan seçmeli veya doğru/yanlış sorusu içermelidir.");

        RuleForEach(x => x.MultipleChoiceQuestions).NotNull().ChildRules(question =>
        {
            question.RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(2000);
            question.RuleFor(x => x.Weight).GreaterThan(0);
            question.RuleFor(x => x.DisplayOrder).GreaterThan(0);
            question.RuleFor(x => x.Options).NotEmpty();
            question.RuleForEach(x => x.Options).NotNull().ChildRules(option =>
            {
                option.RuleFor(x => x.OptionText).NotEmpty().MaximumLength(2000);
                option.RuleFor(x => x.Order).IsInEnum();
            });
        });

        RuleForEach(x => x.TrueFalseQuestions).NotNull().ChildRules(question =>
        {
            question.RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(2000);
            question.RuleFor(x => x.Weight).GreaterThan(0);
            question.RuleFor(x => x.DisplayOrder).GreaterThan(0);
            question.RuleFor(x => x.TrueAnswer).NotEmpty().MaximumLength(2000)
                .When(x => x.TrueAnswer is not null);
            question.RuleFor(x => x.FalseAnswer).NotEmpty().MaximumLength(2000)
                .When(x => x.FalseAnswer is not null);
            question.RuleFor(x => x.Options)
                .Must(options => options is { Count: 2 } &&
                    options.Count(option => option is { Type: TrueFalseOptionType.True }) == 1 &&
                    options.Count(option => option is { Type: TrueFalseOptionType.False }) == 1 &&
                    options.Count(option => option is { IsCorrect: true }) == 1)
                .WithMessage("Bir doğru ve bir yanlış seçeneği gönderilmeli ve yalnızca biri doğru cevap olarak işaretlenmelidir.");
        });
    }
}

