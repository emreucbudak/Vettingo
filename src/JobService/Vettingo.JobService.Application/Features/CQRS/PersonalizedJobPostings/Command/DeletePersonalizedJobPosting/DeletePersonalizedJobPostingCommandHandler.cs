using FlashMediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.UnitOfWork;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.DeletePersonalizedJobPosting;

public class DeletePersonalizedJobPostingCommandHandler(
    IJobUnitOfWork unitOfWork,
    ILogger<DeletePersonalizedJobPostingCommandHandler> logger) : IRequestHandler<DeletePersonalizedJobPostingCommandRequest>
{
    public async Task Handle(DeletePersonalizedJobPostingCommandRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeletePersonalizedJobPostingCommandHandler));
        var posting = await unitOfWork.PersonalizedJobPostings.FirstOrDefaultAsync(
            posting => posting.Id == request.PersonalizedJobPostingId, cancellationToken);
        if (posting is null)
        {
            throw new NotFoundException("Kişiselleştirilmiş iş ilanı bulunamadı");
        }

        unitOfWork.PersonalizedJobPostings.Remove(posting);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
