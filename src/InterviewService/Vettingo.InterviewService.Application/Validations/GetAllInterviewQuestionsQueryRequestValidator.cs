using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetAll;

public sealed class GetAllInterviewQuestionsQueryRequestValidator : AbstractValidator<GetAllInterviewQuestionsQueryRequest>
{
    public GetAllInterviewQuestionsQueryRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().When(x => x.CompanyId.HasValue);
    }
}

