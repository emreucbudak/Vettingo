using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetAll;

public sealed class GetAllInterviewExamsQueryRequestValidator : AbstractValidator<GetAllInterviewExamsQueryRequest>
{
    public GetAllInterviewExamsQueryRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().When(x => x.CompanyId.HasValue);
    }
}

