using FlashMediator;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.MarkInterviewStepPassed;

public sealed class MarkInterviewStepPassedCommandHandler(IJobApplicationRepository repository)
    : IRequestHandler<MarkInterviewStepPassedCommandRequest>
{
    public async Task Handle(MarkInterviewStepPassedCommandRequest request, CancellationToken cancellationToken)
    {
        var application = await repository.GetByCandidateAndJobPostingIdAsync(
            request.UserId, request.JobId, cancellationToken)
            ?? throw new NotFoundException("Başvuru bulunamadı.");

        if (application.IsCurrentStepPassed)
        {
            return;
        }

        application.MarkCurrentStepPassed();
        await repository.SaveChangesAsync();
    }
}
