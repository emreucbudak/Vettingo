using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;

public class CreatePersonalizedJobPostingCommandHandler(
    IPersonalizedJobPostingsRepository repository,
    ILogger<CreatePersonalizedJobPostingCommandHandler> logger) : IRequestHandler<CreatePersonalizedJobPostingCommandRequest>
{
    public async Task Handle(CreatePersonalizedJobPostingCommandRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreatePersonalizedJobPostingCommandHandler));
        var posting = new Domain.Entities.PersonalizedJobPostings
        {
            UserId = request.UserId,
            Title = request.Title,
            PublishedDate = request.PublishedDate,
            CityId = request.CityId
        };
        await repository.AddAsync(posting, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
