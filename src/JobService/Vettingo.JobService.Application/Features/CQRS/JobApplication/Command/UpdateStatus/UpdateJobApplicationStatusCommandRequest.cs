using FlashMediator;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.UpdateStatus
{
    public record UpdateJobApplicationStatusCommandRequest : IRequest
    {
        public Guid ApplicationId { get; init; }
        public ApplicationStatus Status { get; init; }
    }
}
