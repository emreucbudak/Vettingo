using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.AdvanceJobStep;

public class AdvanceJobStepCommandHandler : IRequestHandler<AdvanceJobStepCommandRequest>
{
    public Task Handle(AdvanceJobStepCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
