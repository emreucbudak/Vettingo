using FlashMediator;
using Vettingo.InterviewService.Application.Repository;
using InterviewResultEntity = Vettingo.InterviewService.Domain.Entities.InterviewResult;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Command.Create;

public class CreateInterviewResultCommandHandler(IInterviewResultRepository repository)
    : IRequestHandler<CreateInterviewResultCommandRequest>
{
    public async Task Handle(CreateInterviewResultCommandRequest request, CancellationToken cancellationToken)
    {
        var interviewResult = new InterviewResultEntity();
        interviewResult.Create(request.UserId, request.InterviewId, request.InterviewScore, request.Passed);

        await repository.AddAsync(interviewResult, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
