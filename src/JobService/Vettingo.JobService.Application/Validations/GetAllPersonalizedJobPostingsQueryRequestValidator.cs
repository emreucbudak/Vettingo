using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;

namespace Vettingo.JobService.Application.Validations;

public sealed class GetAllPersonalizedJobPostingsQueryRequestValidator : AbstractValidator<GetAllPersonalizedJobPostingsQueryRequest>
{
    public GetAllPersonalizedJobPostingsQueryRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
