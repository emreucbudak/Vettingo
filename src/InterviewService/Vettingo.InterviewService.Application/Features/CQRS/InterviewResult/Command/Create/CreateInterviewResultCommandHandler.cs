using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Command.Create;

public class CreateInterviewResultCommandHandler : IRequestHandler<CreateInterviewResultCommandRequest>
{
    public Task Handle(CreateInterviewResultCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
