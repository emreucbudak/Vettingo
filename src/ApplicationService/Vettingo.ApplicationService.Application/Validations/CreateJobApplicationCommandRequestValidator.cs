using FluentValidation;
using Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Command.Create;

namespace Vettingo.ApplicationService.Application.Validations
{
    public sealed class CreateJobApplicationCommandRequestValidator : AbstractValidator<CreateJobApplicationCommandRequest>
    {
        public CreateJobApplicationCommandRequestValidator()
        {
            RuleFor(request => request.CandidateId).NotEmpty();
            RuleFor(request => request.JobPostingId).NotEmpty();
            RuleFor(request => request.AppliedAt).NotEqual(default(DateTime)).When(request => request.AppliedAt.HasValue);
            RuleFor(request => request.Status).IsInEnum();
        }
    }
}
