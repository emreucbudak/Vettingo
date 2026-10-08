using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.DeletePersonalizedJobPosting;

public class DeletePersonalizedJobPostingCommandHandler(
    IPersonalizedJobPostingsRepository repository,
    ILogger<DeletePersonalizedJobPostingCommandHandler> logger) : IRequestHandler<DeletePersonalizedJobPostingCommandRequest>
{
    public async Task Handle(DeletePersonalizedJobPostingCommandRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeletePersonalizedJobPostingCommandHandler));
        var posting = await repository.GetByIdAsync(request.PersonalizedJobPostingId, cancellationToken);
        if (posting is null)
        {
            throw new NotFoundException("Kişiselleştirilmiş iş ilanı bulunamadı");
        }

        repository.Delete(posting);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
