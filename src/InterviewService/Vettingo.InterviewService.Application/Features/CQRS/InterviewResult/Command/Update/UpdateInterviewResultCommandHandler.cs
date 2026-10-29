using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Command.Update;

public class UpdateInterviewResultCommandHandler : IRequestHandler<UpdateInterviewResultCommandRequest>
{
    public Task Handle(UpdateInterviewResultCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
