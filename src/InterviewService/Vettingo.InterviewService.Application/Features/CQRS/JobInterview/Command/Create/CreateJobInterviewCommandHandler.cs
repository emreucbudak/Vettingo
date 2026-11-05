using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.JobInterview.Command.Create;

public class CreateJobInterviewCommandHandler : IRequestHandler<CreateJobInterviewCommandRequest>
{
    public Task Handle(CreateJobInterviewCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
