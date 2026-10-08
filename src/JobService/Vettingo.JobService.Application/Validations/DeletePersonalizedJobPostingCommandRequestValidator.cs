using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.DeletePersonalizedJobPosting;

namespace Vettingo.JobService.Application.Validations;

public sealed class DeletePersonalizedJobPostingCommandRequestValidator : AbstractValidator<DeletePersonalizedJobPostingCommandRequest>
{
    public DeletePersonalizedJobPostingCommandRequestValidator()
    {
        RuleFor(x => x.PersonalizedJobPostingId).NotEmpty();
    }
}
