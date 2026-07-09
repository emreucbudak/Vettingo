using FluentValidation;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.DeleteJobPosting;

public sealed class DeleteJobPostingCommandRequestValidator : AbstractValidator<DeleteJobPostingCommandRequest>
{
    public DeleteJobPostingCommandRequestValidator()
    {
        RuleFor(x => x.JobPostingId).NotEmpty();
    }
}

