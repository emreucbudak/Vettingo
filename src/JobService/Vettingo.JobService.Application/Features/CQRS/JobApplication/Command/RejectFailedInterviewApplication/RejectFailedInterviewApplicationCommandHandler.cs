using FlashMediator;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.RejectFailedInterviewApplication;

public sealed class RejectFailedInterviewApplicationCommandHandler(IJobApplicationRepository repository)
    : IRequestHandler<RejectFailedInterviewApplicationCommandRequest>
{
    public async Task Handle(RejectFailedInterviewApplicationCommandRequest request, CancellationToken cancellationToken)
    {
        var application = await repository.GetByCandidateAndJobPostingIdAsync(
            request.UserId, request.JobId, cancellationToken)
            ?? throw new NotFoundException("Başvuru bulunamadı.");

        if (application.Status == ApplicationStatus.Elendi)
        {
            return;
        }

        application.UpdateStatus(ApplicationStatus.Elendi);
        await repository.SaveChangesAsync();
    }
}
