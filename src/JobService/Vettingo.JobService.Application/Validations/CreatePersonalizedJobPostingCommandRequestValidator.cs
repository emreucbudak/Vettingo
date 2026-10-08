using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;

namespace Vettingo.JobService.Application.Validations;

public sealed class CreatePersonalizedJobPostingCommandRequestValidator : AbstractValidator<CreatePersonalizedJobPostingCommandRequest>
{
    public CreatePersonalizedJobPostingCommandRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CityId).GreaterThan(0);
        RuleFor(x => x.PublishedDate).NotEmpty();
        RuleFor(x => x.Salary).GreaterThanOrEqualTo(0);
    }
}
