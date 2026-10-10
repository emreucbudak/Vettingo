using FluentValidation;
using Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetByDate;

namespace Vettingo.InterviewService.Application.Validations;

public sealed class GetInterviewsByDateQueryRequestValidator : AbstractValidator<GetInterviewsByDateQueryRequest>
{
    public GetInterviewsByDateQueryRequestValidator()
    {
        RuleFor(request => request.CompanyId).NotEmpty();
        RuleFor(request => request.Date).NotEmpty();
    }
}
