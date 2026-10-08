using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetLatest;

namespace Vettingo.JobService.Application.Validations;

public sealed class GetLatestPersonalizedJobPostingsQueryRequestValidator : AbstractValidator<GetLatestPersonalizedJobPostingsQueryRequest>
{
    public GetLatestPersonalizedJobPostingsQueryRequestValidator()
    {
        RuleFor(request => request.UserId).NotEmpty();
    }
}
