using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.UpdateStatus;

namespace Vettingo.JobService.Application.Validations
{
    public sealed class UpdateJobApplicationStatusCommandRequestValidator : AbstractValidator<UpdateJobApplicationStatusCommandRequest>
    {
        public UpdateJobApplicationStatusCommandRequestValidator()
        {
            RuleFor(request => request.ApplicationId).NotEmpty();
            RuleFor(request => request.Status).IsInEnum();
        }
    }
}
