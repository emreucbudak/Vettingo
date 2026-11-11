using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.UnitOfWork;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;

public class CreatePersonalizedJobPostingCommandHandler(
    IJobUnitOfWork unitOfWork,
    ILogger<CreatePersonalizedJobPostingCommandHandler> logger) : IRequestHandler<CreatePersonalizedJobPostingCommandRequest>
{
    public async Task Handle(CreatePersonalizedJobPostingCommandRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreatePersonalizedJobPostingCommandHandler));
        var posting = new Domain.Entities.PersonalizedJobPostings
        {
            UserId = request.UserId,
            JobPostingId = request.JobPostingId
        };
        await unitOfWork.PersonalizedJobPostings.AddAsync(posting, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
