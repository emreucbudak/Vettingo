using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetById;

public sealed class GetInterviewQuestionByIdQueryRequestValidator : AbstractValidator<GetInterviewQuestionByIdQueryRequest>
{
    public GetInterviewQuestionByIdQueryRequestValidator()
    {
        RuleFor(x => x.InterviewQuestionId).NotEmpty();
    }
}

