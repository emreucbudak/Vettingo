using FlashMediator;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Repository;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Command.Update;

public class UpdateInterviewResultCommandHandler(IInterviewResultRepository repository)
    : IRequestHandler<UpdateInterviewResultCommandRequest>
{
    public async Task Handle(UpdateInterviewResultCommandRequest request, CancellationToken cancellationToken)
    {
        var interviewResult = await repository.GetByIdAsync(request.InterviewResultId, cancellationToken)
            ?? throw new NotFoundException("Mülakat sonucu bulunamadı.");

        if (request.InterviewScore is null && request.Passed is null)
        {
            return;
        }

        interviewResult.Update(request.InterviewScore, request.Passed);
        repository.Update(interviewResult);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
