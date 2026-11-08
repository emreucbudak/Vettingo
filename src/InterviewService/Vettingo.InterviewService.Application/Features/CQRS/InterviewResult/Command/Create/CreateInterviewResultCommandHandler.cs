using FlashMediator;
using Vettingo.InterviewService.Application.DTOs;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Messaging;
using Vettingo.InterviewService.Application.Repository;
using InterviewResultEntity = Vettingo.InterviewService.Domain.Entities.InterviewResult;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Command.Create;

public class CreateInterviewResultCommandHandler(
    IInterviewResultRepository repository,
    IInterviewRepository interviewRepository,
    IInterviewResultPublisher publisher)
    : IRequestHandler<CreateInterviewResultCommandRequest>
{
    public async Task Handle(CreateInterviewResultCommandRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var interview = await interviewRepository.GetByIdAsync(request.InterviewId, cancellationToken)
            ?? throw new NotFoundException("Mülakat bulunamadı.");

        var interviewResult = new InterviewResultEntity();
        interviewResult.Create(request.UserId, interview.Id, interview.JobId, request.InterviewScore, request.Passed);

        await repository.AddAsync(interviewResult, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        if (!interviewResult.Passed)
        {
            await publisher.PublishAsync(new InterviewResultMessage
            {
                UserId = interviewResult.UserId,
                JobId = interviewResult.JobId
            }, cancellationToken);
        }
    }
}
