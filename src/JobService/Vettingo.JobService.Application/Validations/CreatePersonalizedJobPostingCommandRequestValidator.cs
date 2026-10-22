using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;

namespace Vettingo.JobService.Application.Validations;

public sealed class CreatePersonalizedJobPostingCommandRequestValidator : AbstractValidator<CreatePersonalizedJobPostingCommandRequest>
{
    public CreatePersonalizedJobPostingCommandRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.JobPostingId).NotEmpty();
    }
}
