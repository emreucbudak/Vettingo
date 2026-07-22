using FlashMediator;
using Vettingo.ApplicationService.Domain.Enums;

namespace Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Command.UpdateStatus
{
    public record UpdateJobApplicationStatusCommandRequest : IRequest
    {
        public Guid ApplicationId { get; init; }
        public ApplicationStatus Status { get; init; }
    }
}
