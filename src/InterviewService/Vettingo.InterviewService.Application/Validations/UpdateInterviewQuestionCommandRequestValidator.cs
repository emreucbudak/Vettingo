using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.UpdateInterviewQuestion;

public sealed class UpdateInterviewQuestionCommandRequestValidator : AbstractValidator<UpdateInterviewQuestionCommandRequest>
{
    public UpdateInterviewQuestionCommandRequestValidator()
    {
        RuleFor(x => x.InterviewQuestionId).NotEmpty();
        RuleFor(x => x.CompanyId).NotEmpty().When(x => x.CompanyId.HasValue);
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(2000);
    }
}

