using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Query.GetAll;

public sealed class GetInterviewAnswersQueryRequestValidator : AbstractValidator<GetInterviewAnswersQueryRequest>
{
    public GetInterviewAnswersQueryRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().When(x => x.UserId.HasValue);
        RuleFor(x => x.InterviewExamId).NotEmpty().When(x => x.InterviewExamId.HasValue);
    }
}

