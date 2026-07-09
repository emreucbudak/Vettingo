using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.CreateInterviewQuestion;

public sealed class CreateInterviewQuestionCommandRequestValidator : AbstractValidator<CreateInterviewQuestionCommandRequest>
{
    public CreateInterviewQuestionCommandRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().When(x => x.CompanyId.HasValue);
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(2000);
    }
}

