using FluentValidation;
using Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Command.UpdateStatus;

namespace Vettingo.ApplicationService.Application.Validations
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
