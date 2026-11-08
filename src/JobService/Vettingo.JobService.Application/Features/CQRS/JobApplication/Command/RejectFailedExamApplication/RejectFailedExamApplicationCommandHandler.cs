using FlashMediator;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.RejectFailedExamApplication;

public sealed class RejectFailedExamApplicationCommandHandler(IJobApplicationRepository repository)
    : IRequestHandler<RejectFailedExamApplicationCommandRequest>
{
    public async Task Handle(RejectFailedExamApplicationCommandRequest request, CancellationToken cancellationToken)
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
